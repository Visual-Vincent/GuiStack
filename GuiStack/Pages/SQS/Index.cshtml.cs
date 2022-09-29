/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Net;
using System.Threading.Tasks;
using GuiStack.Models;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GuiStack.Pages.SQS
{
    public class IndexModel : PageModel
    {
        public ISQSRepository SQSRepository { get; }

        [BindProperty(SupportsGet = true)]
        public string Queue { get; set; }

        public IndexModel(ISQSRepository sqsRepository)
        {
            this.SQSRepository = sqsRepository;
        }

        private IActionResult HandleException(Exception ex)
        {
            if(ex == null)
                throw new ArgumentNullException(nameof(ex));

            Console.Error.WriteLine(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetPeekMessagesPartial(string prefix, int maxAmount, int waitTimeSeconds)
        {
            try
            {
                var queueUrl = await SQSRepository.GetQueueUrlAsync(Queue);
                var messages = await SQSRepository.ReceiveMessagesAsync(queueUrl, maxAmount, waitTimeSeconds);

                return Partial("_MessagesTablePartial", new SQSMessagesModel() {
                    Messages = messages,
                    Prefix = prefix
                });
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<IActionResult> OnGetReceiveMessagesPartial(string prefix, int maxAmount, int waitTimeSeconds)
        {
            try
            {
                var queueUrl = await SQSRepository.GetQueueUrlAsync(Queue);
                var messages = await SQSRepository.ReceiveMessagesAsync(queueUrl, maxAmount, waitTimeSeconds);

                foreach(var message in messages)
                {
                    await SQSRepository.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                }

                return Partial("_MessagesTablePartial", new SQSMessagesModel() {
                    Messages = messages,
                    Prefix = prefix
                });
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
