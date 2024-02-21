/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.SQS.Model;

namespace GuiStack.Extensions
{
    public static class AWSExtensions
    {
        private static readonly Dictionary<string, string> DynamoDBBillingModeMap = new Dictionary<string, string>() {
            { BillingMode.PAY_PER_REQUEST.Value, "On-demand" },
            { BillingMode.PROVISIONED.Value, "Provisioned" }
        };

        private static readonly Dictionary<string, string> DynamoDBTableClassMap = new Dictionary<string, string>() {
            { TableClass.STANDARD.Value, "Standard" },
            { TableClass.STANDARD_INFREQUENT_ACCESS.Value, "Standard-IA" }
        };

        /// <summary>
        /// Throws a <see cref="WebException"/> if the response returns a non-successful status code (includes 3xx redirects, since they are unusable by the caller).
        /// </summary>
        /// <param name="response">The response to check.</param>
        /// <param name="serviceName">Optional. The name of the Amazon service that is returning the response. This is included in the error message.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfUnsuccessful(this AmazonWebServiceResponse response, string serviceName = "API")
        {
            if(!response.HttpStatusCode.IsSuccessful())
            {
                if(string.IsNullOrWhiteSpace(serviceName))
                    serviceName = "API";
                else
                    serviceName = serviceName.Trim();

                throw new WebException($"Amazon {serviceName} returned status code {(int)response.HttpStatusCode}");
            }
        }

        /// <summary>
        /// Converts the <see cref="BillingMode"/> into a human-readable string.
        /// </summary>
        /// <param name="entity">The <see cref="BillingMode"/> to convert.</param>
        public static string ToHumanReadableString(this BillingMode entity)
        {
            if(entity == null)
                return "(Unknown)";

            return DynamoDBBillingModeMap.GetValueOrDefault(entity?.Value) ?? "(Unknown)";
        }

        /// <summary>
        /// Converts the <see cref="TableClass"/> into a human-readable string.
        /// </summary>
        /// <param name="entity">The <see cref="TableClass"/> to convert.</param>
        public static string ToHumanReadableString(this TableClass entity)
        {
            if(entity == null)
                return "(Unknown)";

            return DynamoDBTableClassMap.GetValueOrDefault(entity?.Value) ?? "(Unknown)";
        }

        public static Dictionary<string, string> ToStringAttributes(this Dictionary<string, MessageAttributeValue> attributes)
        {
            Dictionary<string, string> strAttributes = new Dictionary<string, string>();

            foreach(var kvp in attributes)
            {
                switch(kvp.Value.DataType.ToLower())
                {
                    case "string":
                    case "number":
                        strAttributes.Add(kvp.Key, kvp.Value.StringValue);
                        break;

                    case "binary":
                        using(var memoryStream = kvp.Value.BinaryValue)
                        {
                            byte[] data = memoryStream.ToArray();
                            strAttributes.Add(kvp.Key, Convert.ToBase64String(data));
                        }
                        break;

                    default:
                        throw new ArgumentException($"Unknown data type '{kvp.Value.DataType}'");
                }
            }

            return strAttributes;
        }
    }
}
