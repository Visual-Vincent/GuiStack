﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;

namespace GuiStack.Authentication.AWS
{
    public class S3Authenticator : Authenticator<AWSCredentials, AmazonS3Client>
    {
        public override AmazonS3Client Authenticate(AWSCredentials credentials)
        {
            var config = new AmazonS3Config() {
                AuthenticationRegion = AWSConfigs.AWSRegion,
                MaxErrorRetry = 1,
                ForcePathStyle = EnvironmentVariables.S3ForcePathStyle // Should be True for LocalStack support
            };

            string endpointUrl = Environment.GetEnvironmentVariable("AWS_S3_ENDPOINT_URL");

            if(!string.IsNullOrWhiteSpace(endpointUrl))
                config.ServiceURL = endpointUrl;

            return new AmazonS3Client(credentials, config);
        }

        public override AWSCredentials GetCredentials()
        {
            return new BasicAWSCredentials(
                Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
                Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")
            );
        }
    }
}
