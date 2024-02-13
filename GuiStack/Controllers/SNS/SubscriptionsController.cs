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
    public class SubscriptionsController : Controller
    {
        private ISNSRepository snsRepository;

        public SubscriptionsController(ISNSRepository snsRepository)
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
        [Produces("application/json")]
        public async Task<ActionResult> CreateSubscription([FromBody] SNSCreateSubscriptionModel model)
        {
            if(string.IsNullOrWhiteSpace(model.TopicArn))
                return StatusCode((int)HttpStatusCode.BadRequest, new { error = "'topicArn' cannot be empty" });

            if(string.IsNullOrWhiteSpace(model.Protocol))
                return StatusCode((int)HttpStatusCode.BadRequest, new { error = "'protocol' cannot be empty" });

            if(string.IsNullOrWhiteSpace(model.Endpoint))
                return StatusCode((int)HttpStatusCode.BadRequest, new { error = "'endpoint' cannot be empty" });

            if(!model.Protocol.Equals("sqs", StringComparison.OrdinalIgnoreCase))
                return StatusCode((int)HttpStatusCode.BadRequest, new { error = $"Protocol '{model.Protocol}' is not supported. Supported protocols are: sqs" });

            try
            {
                await snsRepository.CreateTopicSubscriptionAsync(model);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{subscriptionArn}")]
        public async Task<ActionResult> DeleteSubscription(string subscriptionArn)
        {
            if(string.IsNullOrWhiteSpace(subscriptionArn))
                return StatusCode((int)HttpStatusCode.BadRequest);

            subscriptionArn = subscriptionArn.DecodeRouteParameter();

            try
            {
                await snsRepository.DeleteSubscriptionAsync(subscriptionArn);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
