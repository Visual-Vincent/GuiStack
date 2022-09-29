/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Amazon.S3;
using GuiStack.Extensions;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Http;
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
                    return StatusCode((int)s3ex.StatusCode, new { error = s3ex.Message });

                Console.Error.WriteLine(s3ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
            }

            Console.Error.WriteLine(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpPost("{bucketName}")]
        public async Task<ActionResult> CreateBucket([FromRoute] string bucketName)
        {
            if(string.IsNullOrWhiteSpace(bucketName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            bucketName = bucketName.DecodeRouteParameter();

            try
            {
                await s3Repository.CreateBucketAsync(bucketName);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{bucketName}")]
        public async Task<ActionResult> DeleteBucket([FromRoute] string bucketName)
        {
            if(string.IsNullOrWhiteSpace(bucketName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            bucketName = bucketName.DecodeRouteParameter();

            try
            {
                await s3Repository.DeleteBucketAsync(bucketName);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{bucketName}/{objectName}")]
        public async Task<ActionResult> DeleteObject([FromRoute] string bucketName, [FromRoute] string objectName)
        {
            if(string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(objectName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            bucketName = bucketName.DecodeRouteParameter();
            objectName = objectName.DecodeRouteParameter();

            try
            {
                await s3Repository.DeleteObjectAsync(bucketName, objectName);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{bucketName}/download/{objectName}")]
        public async Task<ActionResult> Download([FromRoute] string bucketName, [FromRoute] string objectName)
        {
            if(string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(objectName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            bucketName = bucketName.DecodeRouteParameter();
            objectName = objectName.DecodeRouteParameter();

            try
            {
                using var obj = await s3Repository.GetObjectAsync(bucketName, objectName);
                using var stream = obj.ResponseStream;

                Response.ContentLength = obj.ContentLength;
                Response.ContentType = "application/octet-stream";

                var contentDisposition = new ContentDisposition {
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

        [HttpGet]
        public async Task<ActionResult> GetBuckets()
        {
            try
            {
                var buckets = (await s3Repository.GetBucketsAsync())
                    .OrderBy(b => b.Name);

                return Json(buckets);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{bucketName}")]
        public async Task<ActionResult> GetObjects([FromRoute] string bucketName)
        {
            if(string.IsNullOrWhiteSpace(bucketName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            bucketName = bucketName.DecodeRouteParameter();

            try
            {
                var objects = (await s3Repository.GetObjectsAsync(bucketName))
                    .OrderBy(o => o.Name);

                return Json(objects);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("{bucketName}/rename/{objectName}/{newName}")]
        public async Task<ActionResult> RenameObject([FromRoute] string bucketName, [FromRoute] string objectName, [FromRoute] string newName)
        {
            if(string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(objectName) || string.IsNullOrWhiteSpace(newName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            bucketName = bucketName.DecodeRouteParameter();
            objectName = objectName.DecodeRouteParameter();
            newName = newName.DecodeRouteParameter();

            try
            {
                await s3Repository.RenameObjectAsync(bucketName, objectName, newName);
                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        const long MaxFileSize = 2 * 1024L * 1024L * 1024L; // 2 GB

        [HttpPost("{bucketName}/upload")]
        [Produces("application/json")]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<ActionResult> Upload([FromRoute] string bucketName, IFormFile file)
        {
            try
            {
                string filename = Uri.EscapeDataString(Path.GetFileName(file.FileName));

                if(file.Length > MaxFileSize)
                    throw new Exception($"File \"{filename}\" exceeds the maximum allowed size of {MaxFileSize.ToFormattedFileSize()}");

                using(Stream stream = file.OpenReadStream())
                    await s3Repository.UploadFile(bucketName, filename, stream);

                return Json(new { success = true });
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
