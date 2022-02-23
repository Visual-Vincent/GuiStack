using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuiStack.Models;
using GuiStack.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GuiStack.Pages.S3
{
    public class IndexModel : PageModel
    {
        private IS3Repository s3BucketRepository;

        [BindProperty(SupportsGet = true)]
        public string Bucket { get; set; }

        public IndexModel(IS3Repository s3BucketRepository)
        {
            this.s3BucketRepository = s3BucketRepository;
        }

        public void OnGet()
        {
        }

        public async Task<IEnumerable<S3Bucket>> GetBuckets()
        {
            return await s3BucketRepository.GetBucketsAsync();
        }

        public async Task<IEnumerable<S3Object>> GetBucketContents(string bucket)
        {
            return await s3BucketRepository.GetObjectsAsync(bucket);
        }
    }
}
