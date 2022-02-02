using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GuiStack.Pages.S3
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Bucket { get; set; }

        public void OnGet()
        {
        }
    }
}
