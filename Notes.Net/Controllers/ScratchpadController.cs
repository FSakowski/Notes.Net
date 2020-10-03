using Microsoft.AspNetCore.Mvc;
using Notes.Net.Models;
using Notes.Net.Models.ViewModels;
using Notes.Net.Service;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Net.Controllers
{
    public class ScratchpadController : Controller
    {
        private readonly INoteService noteService;

        public ScratchpadController(INoteService service)
        {
            noteService = service;
        }

        [Route("/scratchpad/{id:int}")]
        public IActionResult View([Required] int id)
        {
            var sp = noteService.Scratchpads.FirstOrDefault(s => s.ScratchpadId == id);

            if (sp == null)
                return NotFound();

            return View(sp);
        }

        [Route("/scratchpad/{project}/{scratchpad}")]
        public IActionResult View([Required] string project, [Required] string scratchpad)
        {
            var proj = noteService.Projects.FirstOrDefault(p => p.Title == project);

            if (proj == null)
                return NotFound();

            var sp = proj.Scratchpads.FirstOrDefault(s => s.Title == scratchpad);

            if (sp == null)
                return NotFound();

            ViewBag.Project    = proj.ProjectId;
            ViewBag.Scratchpad = sp.ScratchpadId;

            return View(sp);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View(new Scratchpad());
        }

        [HttpPost]
        public IActionResult Create(Scratchpad post)
        {
            if (post.ProjectId <= 0)
                ModelState.AddModelError(nameof(Scratchpad.ProjectId), "No Project selected");

            if (!ModelState.IsValid)
                return View(post);

            Scratchpad scratch = new Scratchpad() { Title = post.Title, ProjectId = post.ProjectId };

            noteService.SaveScratchpad(scratch);

            var proj = noteService.Projects.First(p => p.ProjectId == scratch.ProjectId);

            return RedirectToAction("View", "Scratchpad",
                new { project = proj.Title, scratchpad = scratch.Title });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var scratch = noteService.Scratchpads.FirstOrDefault(s => s.ScratchpadId == id);
            if (scratch == null)
                return NotFound(id);

            return View(scratch);
        }

        [HttpPost]
        public IActionResult Edit(Scratchpad scratchpad)
        {
            if (!ModelState.IsValid)
                return View(scratchpad);

            noteService.SaveScratchpad(scratchpad);

            var proj = noteService.Projects.First(p => p.ProjectId == scratchpad.ProjectId);

            return RedirectToAction("View", "Scratchpad",
                new { project = proj.Title, scratchpad = scratchpad.Title });
        }

        [Route("/scratchpad/list")]
        [HttpGet]
        public IEnumerable<Scratchpad> Get([Required] int projectId)
        {
            return noteService.Scratchpads.Where(s => s.ProjectId == projectId).ToList();
        }
    }
}
