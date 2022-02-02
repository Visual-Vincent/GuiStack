using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using GuiStack.Authentication.AWS;
using Microsoft.AspNetCore.Mvc;

namespace GuiStack.Controllers.S3
{
    [ApiController]
    [Route("api/" + nameof(S3) + "/[controller]")]
    public class BucketContentsController : Controller
    {
        private S3Authenticator authenticator = new S3Authenticator();

        [HttpGet]
        public async Task<ActionResult> Get(string bucket)
        {
            try
            {
                using var s3 = authenticator.Authenticate();
                var response = await s3.ListObjectsV2Async(new ListObjectsV2Request() {
                    BucketName = bucket
                });

                if(response.HttpStatusCode != HttpStatusCode.OK)
                    return StatusCode((int)response.HttpStatusCode);

                return Json(response.S3Objects.Select(obj => new {
                    Name = obj.Key,
                    Size = obj.Size,
                    LastModified = obj.LastModified,
                }));
            }
            catch(AmazonS3Exception ex)
            {
                if(ex.StatusCode == HttpStatusCode.NotFound)
                    return StatusCode((int)ex.StatusCode);

                Console.Error.WriteLine(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
