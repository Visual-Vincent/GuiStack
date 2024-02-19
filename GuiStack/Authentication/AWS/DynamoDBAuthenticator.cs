/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;

namespace GuiStack.Authentication.AWS
{
    public class DynamoDBAuthenticator : Authenticator<AWSCredentials, AmazonDynamoDBClient>
    {
        public override AmazonDynamoDBClient Authenticate(AWSCredentials credentials)
        {
            var config = new AmazonDynamoDBConfig() {
                AuthenticationRegion = AWSConfigs.AWSRegion,
                MaxErrorRetry = 1
            };

            string endpointUrl = Environment.GetEnvironmentVariable("AWS_DYNAMODB_ENDPOINT_URL");

            if(!string.IsNullOrWhiteSpace(endpointUrl))
                config.ServiceURL = endpointUrl;

            return new AmazonDynamoDBClient(credentials, config);
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
