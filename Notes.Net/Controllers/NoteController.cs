using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;
using Notes.Net.Service;
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
                noteService.SaveNote(note, false);
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
    }
}
