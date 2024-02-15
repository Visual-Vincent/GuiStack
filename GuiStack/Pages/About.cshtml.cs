/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GuiStack.Pages
{
    public class AboutModel : PageModel
    {
        public static readonly string Description = Regex.Replace(ApplicationInfo.Description, @"^GuiStack\s+-\s+", "");

        public void OnGet()
        {
        }
    }
}
