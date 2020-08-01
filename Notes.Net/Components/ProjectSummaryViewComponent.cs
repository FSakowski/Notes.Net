using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Notes.Net.Controllers;
using Notes.Net.Models;
using Notes.Net.Models.ViewModels;
using Notes.Net.Service;
using System.Linq;

namespace Notes.Net.Components
{
    public class ProjectSummaryViewComponent : ViewComponent
    {
        private readonly INoteService noteService;

        public ProjectSummaryViewComponent(INoteService service)
        {
            noteService = service;
        }

        public IViewComponentResult Invoke()
        {
            Project activeProj = null;
            if (RouteData.Values.ContainsKey("project"))
            {
                activeProj = noteService.Projects.FirstOrDefault(p => p.Title == (string)RouteData.Values["project"]);
            }

            Scratchpad activeSP = null;
            if (RouteData.Values.ContainsKey("scratchpad"))
            {
                activeSP = noteService.Scratchpads.FirstOrDefault(s => s.Title == (string)RouteData.Values["scratchpad"]);
            }

            return View(new ProjectSummaryViewModel
            {
                Projects = noteService.Projects,
                ActiveProject = activeProj,
                ActiveScratchpad = activeSP
            });
        }
    }
}
