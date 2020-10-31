using Notes.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task SaveNoteAsync(Note note, bool updateMetaData = true)
        {
            bool newEntry = note.NoteId == 0;

            if (updateMetaData || newEntry)
                await UpdateMetaData(note, newEntry);

            if (note.ScratchpadId <= 0)
                throw new ArgumentException("note was not assigned to a scratchpad", nameof(note));

            if (!Scratchpads.Any(s => s.ScratchpadId == note.ScratchpadId))
                throw new ArgumentException($"The scratchpad ({note.ScratchpadId}) associated with the note was not found", nameof(note));

            if (newEntry)
            {
                note.Created = DateTime.Now;
                note.CreatedBy = await serviceContext.GetCurrentUser();

                if (note.Width == 0 || note.Height == 0)
                {
                    // apply default size
                    note.Width = DefaultWidth;
                    note.Height = DefaultHeight;
                }

                if (note.PosX == 0 && note.PosY == 0)
                    MoveNoteToFreePosition(note);
            }

            await repository.SaveNoteAsync(note);
        }

        public async Task<Note> SaveQuickNoteAsync(string content)
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
                await SaveProjectAsync(project);
            }

            var scratch = await EnsureDefaultScratchpadCreated(project);

            var title = content.Substring(0, Math.Min(10, content.Length));

            var note = new Note()
            {
                Content = content,
                ScratchpadId = scratch.ScratchpadId,
                Title = $"{DateTime.Now:yyyy-MM-dd HH:mm} {title}"
            };

            await SaveNoteAsync(note);
            return note;
        }

        private async Task UpdateMetaData(Note note, bool newEntry)
        {
            if (newEntry)
            {
                note.Created   = DateTime.Now;
                note.CreatedBy = await serviceContext.GetCurrentUser();
            } else
            {
                note.Modified   = DateTime.Now;
                note.ModifiedBy = await serviceContext.GetCurrentUser();
            }
        }

        private async Task<Scratchpad> EnsureDefaultScratchpadCreated(Project proj)
        {
            var scratch = repository.Scratchpads.FirstOrDefault(s => s.ProjectId == proj.ProjectId && s.Title == DefaultScratchpadTitle);
            if (scratch != null)
                return scratch;

            scratch = new Scratchpad()
            {
                Title = DefaultScratchpadTitle,
                LastAccess = DateTime.Now,
                Owner = await serviceContext.GetCurrentUser(),
                Notes = new List<Note>(),
                ProjectId = proj.ProjectId
            };

            await SaveScratchpadAsync(scratch);
            return scratch;
        }

        public void MoveNoteToFreePosition(Note note)
        {
            int margin = 20;

            var lstNote = Notes.Where(n => n.ScratchpadId == note.ScratchpadId).OrderBy(n => n.PosX).ThenBy(n => n.PosY).LastOrDefault();
            note.PosX = margin;
            note.PosY = (lstNote == null ? 120 : lstNote.PosY + lstNote.Height) + margin;
        }

        public async Task SaveScratchpadAsync(Scratchpad sp)
        {
            if (sp.ProjectId <= 0)
                throw new ArgumentException("scratchpad was not assigned to a project", nameof(sp));

            if (!Projects.Any(p => p.ProjectId == sp.ProjectId))
                throw new ArgumentException($"The project ({sp.ProjectId}) associated with the scratchpad was not found", nameof(sp));

            sp.LastAccess = DateTime.Now;

            await repository.SaveScratchpadAsync(sp);
        }

        public async Task SaveProjectAsync(Project proj)
        {
            await repository.SaveProjectAsync(proj);

            if (proj.Scratchpads.Count == 0)
            {
                await EnsureDefaultScratchpadCreated(proj);
            }
        }

        public async Task DeleteNoteAsync(Note note)
        {
            await repository.DeleteNoteAsync(note.NoteId);
        }

        public async Task DeleteScratchpadAsync(Scratchpad sp)
        {
            await repository.DeleteScratchpadAsync(sp.ScratchpadId);
        }

        public async Task DeleteProjectAsync(Project proj)
        {
            await repository.DeleteProjectAsync(proj.ProjectId);
        }
    }
}
