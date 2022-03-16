using System;
using System.Threading.Tasks;
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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetPeekMessagesPartial(int maxAmount, int waitTimeSeconds)
        {
            var queueUrl = await SQSRepository.GetQueueUrlAsync(Queue);
            var messages = await SQSRepository.ReceiveMessagesAsync(queueUrl, maxAmount, waitTimeSeconds);

            return Partial("_MessagesTablePartial", messages);
        }
    }
}
