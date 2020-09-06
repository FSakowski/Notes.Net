using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Notes.Net.Models;
using Notes.Net.Service;
using System.Threading.Tasks;

namespace Notes.Net.Infrastructure.TagHelpers
{
    [HtmlTargetElement("select", Attributes = "model-for")]
    public class ProjectSelectionTagHelper : TagHelper
    {
        private readonly INoteService noteService;

        public ProjectSelectionTagHelper(INoteService noteService)
        {
            this.noteService = noteService;
        }

        public ModelExpression ModelFor { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.AppendHtml((await output.GetChildContentAsync(false)).GetContent());

            foreach (Project proj in noteService.Projects)
            {
                if (ModelFor.Model is Project selected && selected.ProjectId == proj.ProjectId)
                {
                    output.Content.AppendHtml($"<option selected value=\"{proj.ProjectId}\">{proj.Title}</option>");
                } else
                {
                    output.Content.AppendHtml($"<option value=\"{proj.ProjectId}\">{proj.Title}</option>");
                }

            }

            output.Attributes.SetAttribute("Name", ModelFor.Name);
            /*output.Attributes.SetAttribute("Id", ModelFor.Name);*/
        }

    }
}
