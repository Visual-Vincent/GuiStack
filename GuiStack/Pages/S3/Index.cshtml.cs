using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GuiStack.Pages.S3
{
    public class IndexModel : PageModel
    {
        public IS3Repository S3Repository { get; }

        [BindProperty(SupportsGet = true)]
        public string Bucket { get; set; }

        public IndexModel(IS3Repository s3Repository)
        {
            this.S3Repository = s3Repository;
        }

        public void OnGet()
        {
        }
    }
}
