using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GuiStack.Models;

namespace GuiStack.Extensions
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Converts the <see cref="IEnumerable"/>&lt;<see cref="S3Object"/>&gt; to <see cref="IEnumerable"/>&lt;<see cref="S3ObjectModel"/>&gt;.
        /// </summary>
        /// <param name="objs">The collection to convert.</param>
        /// <param name="bucketName">The name of the bucket that the objects belong to.</param>
        public static IEnumerable<S3ObjectModel> ToObjectModel(this IEnumerable<S3Object> objs, string bucketName)
        {
            return objs.Select(obj => new S3ObjectModel() {
                BucketName = bucketName,
                Object = obj
            });
        }
    }
}
