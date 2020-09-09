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
            return View(new ProjectSummaryViewModel
            {
                Projects = noteService.Projects
            });
        }
    }
}
