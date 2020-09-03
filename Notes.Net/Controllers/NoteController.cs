using Microsoft.AspNetCore.Mvc;
using Notes.Net.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
