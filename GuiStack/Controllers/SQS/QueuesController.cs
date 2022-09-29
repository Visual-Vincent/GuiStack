/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.SQS;
using GuiStack.Extensions;
using GuiStack.Models;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GuiStack.Controllers.SQS
{
    [ApiController]
    [Route("api/" + nameof(SQS) + "/[controller]")]
    public class QueuesController : Controller
    {
        private ISQSRepository sqsRepository;
        
        public QueuesController(ISQSRepository sqsRepository)
        {
            this.sqsRepository = sqsRepository;
        }

        private ActionResult HandleException(Exception ex)
        {
            if(ex == null)
                throw new ArgumentNullException(nameof(ex));

            if(ex is AmazonSQSException sqsEx)
            {
                if(sqsEx.StatusCode == HttpStatusCode.NotFound)
                    return StatusCode((int)sqsEx.StatusCode, new { error = sqsEx.Message });

                Console.Error.WriteLine(sqsEx);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }

            Console.Error.WriteLine(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpPut]
        [Consumes("application/json")]
        public async Task<ActionResult> CreateQueue([FromBody] SQSCreateQueueModel model)
        {
            if(string.IsNullOrWhiteSpace(model.QueueName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                await sqsRepository.CreateQueueAsync(model);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{queueUrl}")]
        public async Task<ActionResult> DeleteQueue([FromRoute] string queueUrl)
        {
            if(string.IsNullOrWhiteSpace(queueUrl))
                return StatusCode((int)HttpStatusCode.BadRequest);

            queueUrl = queueUrl.DecodeRouteParameter();

            try
            {
                await sqsRepository.DeleteQueueAsync(queueUrl);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult> GetQueues()
        {
            try
            {
                var queues = (await sqsRepository.GetQueuesAsync())
                    .OrderBy(q => q.Name);

                return Json(queues);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{queueName}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> SendMessage([FromRoute] string queueName, [FromBody] SQSSendMessageModel message)
        {
            if(message == null)
                return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                string body = message.Body;

                if(message.IsProtobuf)
                {
                    byte[] protoData = Convert.FromBase64String(body);

                    // This re-encoding to Base64 is intentional, as I want to rely on .NET's Base64 implementation to ensure that
                    // in the event that encoding isn't properly performed by the client, we don't send incorrectly encoded messages.
                    // (Insead, the decoding above will likely throw an error)

                    if(message.Base64Encode)
                        body = Convert.ToBase64String(protoData);
                    else
                        body = protoData.ToRawString();
                }
                else if(message.Base64Encode)
                {
                    byte[] data = Encoding.UTF8.GetBytes(body);
                    body = Convert.ToBase64String(data);
                }

                var queueUrl = await sqsRepository.GetQueueUrlAsync(queueName);
                var messageId = await sqsRepository.SendMessageAsync(queueUrl, body);

                return Json(new { messageId = messageId });
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
