﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using Amazon.S3;
using GuiStack.Extensions;

namespace GuiStack.Services
{
    public interface IS3UrlBuilder
    {
        string GetHttpUrl(AmazonS3Client client, string bucketName, string objectName);
        string GetS3Uri(AmazonS3Client client, string bucketName, string objectName);
    }

    public class S3UrlBuilder : IS3UrlBuilder
    {
        private const string S3UriFormat = "s3://{0}/{1}";

        public string GetHttpUrl(AmazonS3Client client, string bucketName, string objectName)
        {
            if(client == null)
                throw new ArgumentNullException(nameof(client));

            bucketName = Uri.EscapeDataString(bucketName);
            objectName = Uri.EscapeDataString(objectName).DecodeRouteParameter();

            Uri url = new Uri(client.Config.DetermineServiceURL());

            if(EnvironmentVariables.S3ForcePathStyle)
                return $"{url.Scheme}://{url.Authority}/{bucketName}/{objectName}";

            return $"{url.Scheme}://{bucketName}.{url.Authority}/{objectName}";
        }

        public string GetS3Uri(AmazonS3Client client, string bucketName, string objectName)
        {
            if(client == null)
                throw new ArgumentNullException(nameof(client));

            bucketName = Uri.EscapeDataString(bucketName);
            objectName = Uri.EscapeDataString(objectName).DecodeRouteParameter();

            return string.Format(S3UriFormat, bucketName, objectName);
        }
    }
}
