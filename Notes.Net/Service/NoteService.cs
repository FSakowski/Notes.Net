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

        public string GeneralProjectTitle => "General";

        public string DefaultScratchpadTitle => "Unsorted";

        public int DefaultWidth => 300;

        public int DefaultHeight => 200;

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
                note.Created = DateTime.Now;
                note.CreatedBy = serviceContext.CurrentUser;

                if (note.Width == 0 || note.Height == 0)
                {
                    // apply default size
                    note.Width = DefaultWidth;
                    note.Height = DefaultHeight;
                }

                if (note.PosX == 0 && note.PosY == 0)
                    MoveNoteToFreePosition(note);
            }

            repository.SaveNote(note);
        }

        public Note SaveQuickNote(string content)
        {
            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException(nameof(content));

            var project = repository.Projects.FirstOrDefault(p => p.Title == GeneralProjectTitle);
            if (project == null)
            {
                project = new Project()
                {
                    Title = GeneralProjectTitle
                };
                SaveProject(project);
            }

            var scratch = EnsureDefaultScratchpadCreated(project);

            var title = content.Substring(0, Math.Min(10, content.Length));

            var note = new Note()
            {
                Content = content,
                ScratchpadId = scratch.ScratchpadId,
                Title = $"{DateTime.Now:yyyy-MM-dd HH:mm} {title}"
            };

            SaveNote(note);
            return note;
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

        private Scratchpad EnsureDefaultScratchpadCreated(Project proj)
        {
            var scratch = repository.Scratchpads.FirstOrDefault(s => s.ProjectId == proj.ProjectId && s.Title == DefaultScratchpadTitle);
            if (scratch != null)
                return scratch;

            scratch = new Scratchpad()
            {
                Title = DefaultScratchpadTitle,
                LastAccess = DateTime.Now,
                Owner = serviceContext.CurrentUser,
                Notes = new List<Note>(),
                ProjectId = proj.ProjectId
            };

            SaveScratchpad(scratch);
            return scratch;
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

            sp.LastAccess = DateTime.Now;

            repository.SaveScratchpad(sp);
        }

        public void SaveProject(Project proj)
        {
            repository.SaveProject(proj);

            if (proj.Scratchpads.Count == 0)
            {
                EnsureDefaultScratchpadCreated(proj);
            }
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
