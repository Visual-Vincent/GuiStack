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
