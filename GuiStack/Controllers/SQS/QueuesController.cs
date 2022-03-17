using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.SQS;
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
                    return StatusCode((int)sqsEx.StatusCode, sqsEx.Message);

                Console.Error.WriteLine(sqsEx);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

            Console.Error.WriteLine(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult> GetQueues()
        {
            try
            {
                var queues = await sqsRepository.GetQueuesAsync();
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
                var queueUrl = await sqsRepository.GetQueueUrlAsync(queueName);
                var messageId = await sqsRepository.SendMessageAsync(queueUrl, message.Body);

                return Json(new { messageId = messageId });
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
