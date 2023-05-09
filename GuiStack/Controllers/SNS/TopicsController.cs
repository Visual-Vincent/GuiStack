/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2023
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Net;
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

        [HttpPut]
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
    }
}
