/*
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
using Amazon.SQS;

namespace GuiStack.Authentication.AWS
{
    public class SQSAuthenticator : Authenticator<AWSCredentials, AmazonSQSClient>
    {
        public override AmazonSQSClient Authenticate(AWSCredentials credentials)
        {
            var config = new AmazonSQSConfig() {
                AuthenticationRegion = AWSConfigs.AWSRegion,
                MaxErrorRetry = 1
            };

            string endpointUrl = Environment.GetEnvironmentVariable("AWS_SQS_ENDPOINT_URL");

            if(!string.IsNullOrWhiteSpace(endpointUrl))
                config.ServiceURL = endpointUrl;

            return new AmazonSQSClient(credentials, config);
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
