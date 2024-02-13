/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using GuiStack.Extensions;
using GuiStack.Models;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GuiStack.Controllers.SNS
{
    [ApiController]
    [Route("api/" + nameof(SNS) + "/[controller]")]
    public class TopicsController : Controller
    {
        private ISNSRepository snsRepository;

        public TopicsController(ISNSRepository snsRepository)
        {
            this.snsRepository = snsRepository;
        }

        private ActionResult HandleException(Exception ex)
        {
            if(ex == null)
                throw new ArgumentNullException(nameof(ex));

            if(ex is AmazonSimpleNotificationServiceException snsEx)
            {
                if(snsEx.StatusCode == HttpStatusCode.NotFound)
                    return StatusCode((int)snsEx.StatusCode, new { error = snsEx.Message });

                Console.Error.WriteLine(snsEx);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }

            Console.Error.WriteLine(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult> CreateTopic([FromBody] SNSCreateTopicModel model)
        {
            if(string.IsNullOrWhiteSpace(model.TopicName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            try
            {
                await snsRepository.CreateTopicAsync(model);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{topicArn}")]
        public async Task<ActionResult> DeleteTopic([FromRoute] string topicArn)
        {
            if(string.IsNullOrWhiteSpace(topicArn))
                return StatusCode((int)HttpStatusCode.BadRequest);

            topicArn = topicArn.DecodeRouteParameter();

            try
            {
                await snsRepository.DeleteTopicAsync(topicArn);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{topicArn}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> SendMessage([FromRoute] string topicArn, [FromBody] SQSSendMessageModel message)
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
                    // (Instead, the decoding above will likely throw an error)

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

                var messageId = await snsRepository.SendMessageAsync(topicArn, body);

                return Json(new { messageId = messageId });
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
