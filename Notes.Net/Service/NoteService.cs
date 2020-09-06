using Notes.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Notes.Net.Service
{
    public class NoteService : INoteService
    {
        private readonly IServiceContext serviceContext;

        private readonly IRepository repository;

        public NoteService(IServiceContext serviceContext, IRepository repository)
        {
            this.serviceContext = serviceContext;
            this.repository = repository;
        }

        public IQueryable<Project> Projects => repository.Projects;

        public IQueryable<Scratchpad> Scratchpads => repository.Scratchpads;

        public IQueryable<Note> Notes => repository.Notes;

        public void SaveNote(Note note, bool updateMetaData = true)
        {
            bool newEntry = note.NoteId == 0;

            if (updateMetaData || newEntry)
                UpdateMetaData(note, newEntry);

            if (note.ScratchpadId <= 0)
                throw new ArgumentException("note was not assigned to a scratchpad", nameof(note));

            if (!Scratchpads.Any(s => s.ScratchpadId == note.ScratchpadId))
                throw new ArgumentException($"The scratchpad ({note.ScratchpadId}) associated with the note was not found", nameof(note));

            if (newEntry)
            {
                if (note.Width == 0 || note.Height == 0)
                {
                    // apply default size
                    note.Width = 300;
                    note.Height = 200;
                }

                if (note.PosX == 0 && note.PosY == 0)
                    MoveNoteToFreePosition(note);
            }

            repository.SaveNote(note);
        }

        private void UpdateMetaData(Note note, bool newEntry)
        {
            if (newEntry)
            {
                note.Created   = DateTime.Now;
                note.CreatedBy = serviceContext.CurrentUser;
            } else
            {
                note.Modified   = DateTime.Now;
                note.ModifiedBy = serviceContext.CurrentUser;
            }
        }

        public void MoveNoteToFreePosition(Note note)
        {
            int margin = 20;

            var lstNote = Notes.Where(n => n.ScratchpadId == note.ScratchpadId).OrderBy(n => n.PosX).ThenBy(n => n.PosY).LastOrDefault();
            note.PosX = margin;
            note.PosY = (lstNote == null ? 120 : lstNote.PosY + lstNote.Height) + margin;
        }

        public void SaveScratchpad(Scratchpad sp)
        {
            if (sp.ProjectId <= 0)
                throw new ArgumentException("scratchpad was not assigned to a project", nameof(sp));

            if (!Projects.Any(p => p.ProjectId == sp.ProjectId))
                throw new ArgumentException($"The project ({sp.ProjectId}) associated with the scratchpad was not found", nameof(sp));

            repository.SaveScratchpad(sp);
        }

        public void SaveProject(Project proj)
        {
            repository.SaveProject(proj);
        }

        public void DeleteNote(Note note)
        {
            repository.DeleteNote(note.NoteId);
        }

        public void DeleteScratchpad(Scratchpad sp)
        {
            repository.DeleteScratchpad(sp.ScratchpadId);
        }

        public void DeleteProject(Project proj)
        {
            repository.DeleteProject(proj.ProjectId);
        }
    }
}
