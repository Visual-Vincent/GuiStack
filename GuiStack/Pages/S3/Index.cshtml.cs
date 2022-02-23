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

        [BindProperty(SupportsGet = true)]
        public string Object { get; set; }

        public IndexModel(IS3Repository s3Repository)
        {
            this.S3Repository = s3Repository;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetDownload()
        {
            if(string.IsNullOrWhiteSpace(Bucket) || string.IsNullOrWhiteSpace(Object))
                return StatusCode((int)HttpStatusCode.NotFound);

            using var obj = await S3Repository.GetObjectAsync(Bucket, Object);
            using var stream = obj.ResponseStream;

            Response.ContentLength = obj.ContentLength;
            Response.ContentType = "application/octet-stream";

            var contentDisposition = new ContentDisposition
            {
                FileName = Object,
                Inline = false
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            int read = 0;
            byte[] buffer = new byte[8192];

            while((read = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await Response.Body.WriteAsync(buffer, 0, read);
            }

            return new EmptyResult();
        }
    }
}
