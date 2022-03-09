using System;
using System.Net;
using System.Runtime.CompilerServices;
using Amazon.Runtime;

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
    }
}
