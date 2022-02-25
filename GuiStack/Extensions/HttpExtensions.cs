using System;
using System.Net;

namespace GuiStack.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Returns whether or not the status code is successful.
        /// </summary>
        /// <param name="statusCode">The status code to check.</param>
        public static bool IsSuccessful(this HttpStatusCode statusCode)
        {
            int code = (int)statusCode;
            return code >= 200 && code < 300;
        }
    }
}
