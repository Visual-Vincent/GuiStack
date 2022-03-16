using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using Amazon.Runtime;
using Amazon.SQS.Model;

namespace GuiStack.Extensions
{
    public static class AWSExtensions
    {
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
