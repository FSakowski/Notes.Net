using Microsoft.AspNetCore.Razor.TagHelpers;
using Notes.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Net.Infrastructure.TagHelpers
{
    [HtmlTargetElement("note", Attributes = "data")]
    public class NoteTagHelper : TagHelper
    {
        public Note Data {
            get;
            set;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";

            if (Data != null)
            {
                output.Attributes.SetAttribute("data-note", $"{Data.NoteId}");
                output.Attributes.SetAttribute("style",
                    "position: absolute; " +
                    $"left: {Data.PosX}px; "+ 
                    $"top: {Data.PosY}px; " + 
                    $"width: {Data.Width}px; " + 
                    $"height: {Data.Height}px;");
            }
        }
    }
}
