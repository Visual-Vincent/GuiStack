/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2023
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;

namespace GuiStack.Authentication.AWS
{
    public class SNSAuthenticator : Authenticator<AWSCredentials, AmazonSimpleNotificationServiceClient>
    {
        public override AmazonSimpleNotificationServiceClient Authenticate(AWSCredentials credentials)
        {
            var config = new AmazonSimpleNotificationServiceConfig() {
                AuthenticationRegion = AWSConfigs.AWSRegion,
                MaxErrorRetry = 1
            };

            string endpointUrl = Environment.GetEnvironmentVariable("AWS_SNS_ENDPOINT_URL");

            if(!string.IsNullOrWhiteSpace(endpointUrl))
                config.ServiceURL = endpointUrl;

            return new AmazonSimpleNotificationServiceClient(credentials, config);
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
