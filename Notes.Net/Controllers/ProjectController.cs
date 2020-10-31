using Microsoft.AspNetCore.Mvc;
using Notes.Net.Models;
using Notes.Net.Service;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Net.Controllers
{
    public class ProjectController : Controller
    {
        private readonly INoteService noteService;

        public ProjectController(INoteService service)
        {
            noteService = service;
        }


        public ViewResult Create()
        {
            return View(new Project());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Required] Project project)
        {
            if (ModelState.IsValid)
            {
                await noteService.SaveProjectAsync(project);
                return RedirectToAction("View", "Scratchpad", 
                    new { project = project.Title, scratchpad = project.Scratchpads.FirstOrDefault()?.Title });
            } else
                return View(project);
        }
    }
}
