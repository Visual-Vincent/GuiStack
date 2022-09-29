/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GuiStack.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "no-href")]
    public class NoHrefTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("no-href");

            if(context.AllAttributes.ContainsName("href"))
                return;

            output.Attributes.Add("href", "javascript:void(0)");
        }
    }
}
