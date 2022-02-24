using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GuiStack.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Interprets the string as a <see cref="FromRouteAttribute"/> parameter and decodes it.
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// Parameters with the <see cref="FromRouteAttribute"/> normally aren't properly decoded before they're passed to their respective controller. 
        /// This extensions takes care of that.
        /// </remarks>
        public static string DecodeRouteParameter(this string value)
        {
            return value.Replace("%2F", "/");
        }
    }
}
