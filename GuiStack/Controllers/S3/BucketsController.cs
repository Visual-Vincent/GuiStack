using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using GuiStack.Authentication.AWS;
using Microsoft.AspNetCore.Mvc;

namespace GuiStack.Controllers.S3
{
    [ApiController]
    [Route("api/" + nameof(S3) + "/[controller]")]
    public class BucketsController : Controller
    {
        private S3Authenticator authenticator = new S3Authenticator();

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            using var s3 = authenticator.Authenticate();
            var response = await s3.ListBucketsAsync();

            if(response.HttpStatusCode != HttpStatusCode.OK)
                return StatusCode((int)response.HttpStatusCode);

            return Json(response.Buckets.Select(b => new {
                CreationDate = b.CreationDate,
                Name = b.BucketName
            }));
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
