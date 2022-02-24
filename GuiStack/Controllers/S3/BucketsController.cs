using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Amazon.S3;
using GuiStack.Extensions;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GuiStack.Controllers.S3
{
    [ApiController]
    [Route("api/" + nameof(S3) + "/[controller]")]
    public class BucketsController : Controller
    {
        private IS3Repository s3Repository;
        
        public BucketsController(IS3Repository s3Repository)
        {
            this.s3Repository = s3Repository;
        }

        private ActionResult HandleException(Exception ex)
        {
            if(ex == null)
                throw new ArgumentNullException(nameof(ex));

            if(ex is AmazonS3Exception s3ex)
            {
                if(s3ex.StatusCode == HttpStatusCode.NotFound)
                    return StatusCode((int)s3ex.StatusCode);

                Console.Error.WriteLine(s3ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            Console.Error.WriteLine(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpGet]
        public async Task<ActionResult> GetBuckets()
        {
            try
            {
                var buckets = await s3Repository.GetBucketsAsync();
                return Json(buckets);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{bucket}")]
        public async Task<ActionResult> GetObjects([FromRoute] string bucket)
        {
            if(string.IsNullOrWhiteSpace(bucket))
                return StatusCode((int)HttpStatusCode.NotFound);

            bucket = bucket.DecodeRouteParameter();

            try
            {
                var objects = await s3Repository.GetObjectsAsync(bucket);
                return Json(objects);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{bucket}/download/{objectName}")]
        public async Task<ActionResult> Download([FromRoute] string bucket, [FromRoute] string objectName)
        {
            if(string.IsNullOrWhiteSpace(bucket) || string.IsNullOrWhiteSpace(objectName))
                return StatusCode((int)HttpStatusCode.NotFound);

            bucket = bucket.DecodeRouteParameter();
            objectName = objectName.DecodeRouteParameter();

            try
            {
                using var obj = await s3Repository.GetObjectAsync(bucket, objectName);
                using var stream = obj.ResponseStream;

                Response.ContentLength = obj.ContentLength;
                Response.ContentType = "application/octet-stream";

                var contentDisposition = new ContentDisposition
                {
                    FileName = Path.GetFileName(objectName),
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
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
