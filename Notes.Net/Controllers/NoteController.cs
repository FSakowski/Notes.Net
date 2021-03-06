﻿using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Notes.Net.Infrastructure;
using Notes.Net.Models;
using Notes.Net.Models.ViewModels;
using Notes.Net.Service;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Notes.Net.Controllers
{
    public class NoteController : Controller
    {
        private readonly INoteService noteService;

        public NoteController(INoteService service)
        {
            noteService = service;
        }

        [HttpGet]
        public IActionResult Create([FromQuery] int scratchpad)
        {
            if (scratchpad <= 0)
                return View(new UpdateNoteViewModel());

            var scratch = noteService.Scratchpads.FirstOrDefault(s => s.ScratchpadId == scratchpad);

            if (scratch == null)
                return View(new UpdateNoteViewModel());

            var items = noteService.Scratchpads.Where(s => s.ProjectId == scratch.ProjectId).ToList();

            ViewBag.ScratchpadItems = new SelectList(items, nameof(Scratchpad.ScratchpadId), nameof(Scratchpad.Title), scratchpad);
            return View(new UpdateNoteViewModel()
            {
                Project = scratch.ProjectId,
                Scratchpad = scratch.ScratchpadId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([Required] UpdateNoteViewModel post)
        {
            if (ModelState.IsValid)
            {
                var note = new Note()
                {
                    ScratchpadId = post.Scratchpad.Value,
                    Title = post.Title,
                    Content = post.Content
                };

                await noteService.SaveNoteAsync(note);

                var scratch = noteService.Scratchpads.First(s => s.ScratchpadId == note.ScratchpadId);
                var project = noteService.Projects.First(p => p.ProjectId == scratch.ProjectId);

                return RedirectToAction("View", "Scratchpad",
                    new { project = project.Title, scratchpad = scratch.Title });
            }
            else
            {
                if (post.Scratchpad > 0)
                {
                    var scratch = noteService.Scratchpads.FirstOrDefault(s => s.ScratchpadId == post.Scratchpad);

                    if (scratch != null)
                    {
                        var items = noteService.Scratchpads.Where(s => s.ProjectId == scratch.ProjectId).ToList();
                        ViewBag.ScratchpadItems = new SelectList(items, nameof(Scratchpad.ScratchpadId), nameof(Scratchpad.Title), post.Scratchpad);
                    }
                }

                return View(post);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuickNote(QuickNoteViewModel post)
        {
            if (!ModelState.IsValid)
            {
                TempData["QuickNote"] = ModelState.First().Value.Errors.First().ErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            var note = await noteService.SaveQuickNoteAsync(post.Content);

            var scratch = noteService.Scratchpads.First(s => s.ScratchpadId == note.ScratchpadId);
            var project = noteService.Projects.First(p => p.ProjectId == scratch.ProjectId);

            return RedirectToAction("View", "Scratchpad",
                new { project = project.Title, scratchpad = scratch.Title });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var note = noteService.Notes.FirstOrDefault(n => n.NoteId == id);
            if (note == null)
                return NotFound(id);

            var scratch = noteService.Scratchpads.First(s => s.ScratchpadId == note.ScratchpadId);
            var items = noteService.Scratchpads.Where(s => s.ProjectId == scratch.ProjectId).ToList();
            ViewBag.ScratchpadItems = new SelectList(items, nameof(Scratchpad.ScratchpadId), nameof(Scratchpad.Title), scratch);

            return View(new UpdateNoteViewModel()
            {
                NoteId = note.NoteId,
                Title = note.Title,
                Scratchpad = scratch.ScratchpadId,
                Project = scratch.ProjectId,
                Content = note.Content
            }); ;
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateNoteViewModel post)
        {
            var note = noteService.Notes.First(n => n.NoteId == post.NoteId);

            if (ModelState.IsValid)
            {
                note.Title = post.Title;
                note.ScratchpadId = post.Scratchpad.Value;

                await noteService.SaveNoteAsync(note);

                var scratch = noteService.Scratchpads.First(s => s.ScratchpadId == note.ScratchpadId);
                var project = noteService.Projects.First(p => p.ProjectId == scratch.ProjectId);

                return RedirectToAction("View", "Scratchpad",
                    new { project = project.Title, scratchpad = scratch.Title });
            }
            else
            {
                if (post.Scratchpad > 0)
                {
                    var scratch = noteService.Scratchpads.FirstOrDefault(s => s.ScratchpadId == post.Scratchpad);

                    if (scratch != null)
                    {
                        var items = noteService.Scratchpads.Where(s => s.ProjectId == scratch.ProjectId).ToList();
                        ViewBag.ScratchpadItems = new SelectList(items, nameof(Scratchpad.ScratchpadId), nameof(Scratchpad.Title), post.Scratchpad);
                    }
                }

                return View(post);
            }
        }

        [HttpPost("[controller]/{noteId:int}/save")]
        public async Task<IActionResult> Save(
            [FromRoute] int noteId)
        {
            if (noteId <= 0)
                ModelState.AddModelError("noteId", "note id may not be empty");

            var note = noteService.Notes.FirstOrDefault(n => n.NoteId == noteId);
            if (note == null)
                ModelState.AddModelError("noteId", "note not found");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using (var reader = new StreamReader(Request.Body))
            {
                string plainText = await reader.ReadToEndAsync();
                var sanitizer = new HtmlSanitizer();
                note.Content = sanitizer.Sanitize(plainText, $"{Request.Scheme}://{Request.Host}{Request.PathBase}");
                await noteService.SaveNoteAsync(note);
            }

            return Ok();
        }

        [HttpPost("[controller]/{noteId:int}/savepos/@{x:int},{y:int}")]
        public async Task<IActionResult> SavePosition(
            [FromRoute] int noteId,
            [FromRoute] int x,
            [FromRoute] int y)
        {
            if (noteId <= 0)
                ModelState.AddModelError("noteId", "note id may not be empty");

            var note = noteService.Notes.FirstOrDefault(n => n.NoteId == noteId);
            if (note == null)
                ModelState.AddModelError("noteId", "note not found");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            note.PosX = x;
            note.PosY = y;
            await noteService.SaveNoteAsync(note, false);

            return Ok();
        }

        [HttpPost("[controller]/{noteId:int}/savesize/{w:int}x{h:int}")]
        public async Task<IActionResult> SaveSize(
            [FromRoute] int noteId,
            [FromRoute] int w,
            [FromRoute] int h)
        {
            if (noteId <= 0)
                ModelState.AddModelError("noteId", "note id may not be empty");

            var note = noteService.Notes.FirstOrDefault(n => n.NoteId == noteId);
            if (note == null)
                ModelState.AddModelError("noteId", "note not found");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            note.Width = w;
            note.Height = h;
            await noteService.SaveNoteAsync(note, false);

            return Ok();
        }

        [HttpDelete("[controller]/{noteId:int}")]
        public async Task<IActionResult> Delete(int noteId)
        {
            if (noteId <= 0)
                ModelState.AddModelError("noteId", "note id may not be empty");

            var note = noteService.Notes.FirstOrDefault(n => n.NoteId == noteId);
            if (note == null)
                ModelState.AddModelError("noteId", "note not found");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await noteService.DeleteNoteAsync(note);
            return Ok();
        }
    }
}
