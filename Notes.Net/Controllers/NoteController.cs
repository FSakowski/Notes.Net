using Ganss.XSS;
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
                return View(new CreateNoteViewModel());

            var scratch = noteService.Scratchpads.FirstOrDefault(s => s.ScratchpadId == scratchpad);

            if (scratch == null)
                return View(new CreateNoteViewModel());

            var items = noteService.Scratchpads.Where(s => s.ProjectId == scratch.ProjectId).ToList();

            ViewBag.ScratchpadItems = new SelectList(items, nameof(Scratchpad.ScratchpadId), nameof(Scratchpad.Title), scratchpad);
            return View(new CreateNoteViewModel()
            {
                Project = scratch.ProjectId,
                Scratchpad = scratch.ScratchpadId
            });
        }

        [HttpPost]
        public IActionResult Create([Required] CreateNoteViewModel post)
        {
            if (ModelState.IsValid)
            {
                var note = new Note()
                {
                    ScratchpadId = post.Scratchpad.Value,
                    Title = post.Title,
                    Content = post.Content
                };

                noteService.SaveNote(note);

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
        public IActionResult CreateQuickNote(QuickNoteViewModel post)
        {
            if (!ModelState.IsValid)
            {
                TempData["QuickNote"] = ModelState.First().Value.Errors.First().ErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            var note = noteService.SaveQuickNote(post.Content);

            var scratch = noteService.Scratchpads.First(s => s.ScratchpadId == note.ScratchpadId);
            var project = noteService.Projects.First(p => p.ProjectId == scratch.ProjectId);

            return RedirectToAction("View", "Scratchpad",
                new { project = project.Title, scratchpad = scratch.Title });
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
                noteService.SaveNote(note);
            }

            return Ok();
        }

        [HttpPost("[controller]/{noteId:int}/savepos/@{x:int},{y:int}")]
        public IActionResult SavePosition(
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
            noteService.SaveNote(note, false);

            return Ok();
        }

        [HttpPost("[controller]/{noteId:int}/savesize/{w:int}x{h:int}")]
        public IActionResult SaveSize(
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
            noteService.SaveNote(note, false);

            return Ok();
        }

        [HttpDelete("[controller]/{noteId:int}")]
        public IActionResult Delete(int noteId)
        {
            if (noteId <= 0)
                ModelState.AddModelError("noteId", "note id may not be empty");

            var note = noteService.Notes.FirstOrDefault(n => n.NoteId == noteId);
            if (note == null)
                ModelState.AddModelError("noteId", "note not found");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            noteService.DeleteNote(note);
            return Ok();
        }
    }
}
