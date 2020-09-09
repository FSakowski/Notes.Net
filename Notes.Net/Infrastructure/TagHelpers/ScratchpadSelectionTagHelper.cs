using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Notes.Net.Models;
using Notes.Net.Service;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Net.Infrastructure.TagHelpers
{
    [HtmlTargetElement("select", Attributes = "model-for,project")]
    public class ScratchpadSelectionTagHelper : TagHelper
    {
        private readonly INoteService noteService;

        public ScratchpadSelectionTagHelper(INoteService noteService)
        {
            this.noteService = noteService;
        }

        public ModelExpression ModelFor { get; set; }

        public int? Project { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.AppendHtml((await output.GetChildContentAsync(false)).GetContent());

            if (Project.HasValue)
            {
                foreach (Scratchpad scratch in noteService.Scratchpads.Where(sp => sp.ProjectId == Project))
                {
                    if (ModelFor.Model is int selected && selected == scratch.ScratchpadId)
                    {
                        output.Content.AppendHtml($"<option selected value=\"{scratch.ScratchpadId}\">{scratch.Title}</option>");
                    }
                    else
                    {
                        output.Content.AppendHtml($"<option value=\"{scratch.ScratchpadId}\">{scratch.Title}</option>");
                    }
                }
            }

            output.Attributes.SetAttribute("Name", ModelFor.Name);
            output.Attributes.SetAttribute("Id", ModelFor.Name);
        }

    }
}
