using System;
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
    }
}
