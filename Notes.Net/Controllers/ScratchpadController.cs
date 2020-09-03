﻿using Microsoft.AspNetCore.Mvc;
using Notes.Net.Models;
using Notes.Net.Models.ViewModels;
using Notes.Net.Service;
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

            return View(sp);
        }

        public ViewResult Create()
        {
            return View(new CreateScratchpadViewModel());
        }

        [HttpPost]
        public IActionResult Create([Required] CreateScratchpadViewModel post)
        {
            if (ModelState.IsValid)
            {
                Project proj = noteService.Projects.FirstOrDefault(p => p.ProjectId == post.Project);
                if (proj == null)
                {
                    ModelState.AddModelError(nameof(post.Project), $"Project {post.Project} not found");
                    return View(post);
                }

                Scratchpad scratch = new Scratchpad() { Title = post.Title, Project = proj };

                noteService.SaveScratchpad(scratch);
                return RedirectToAction("View", "Scratchpad",
                    new { project = scratch.Project.Title, scratchpad = scratch.Title });
            }
            else
                return View(post);
        }
    }
}