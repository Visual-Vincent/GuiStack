/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

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
