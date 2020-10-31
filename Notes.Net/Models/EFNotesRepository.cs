using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Net.Models
{
    public class EFNotesRepository : IRepository
    {
        private readonly NotesDbContext context;

        public EFNotesRepository(NotesDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Project> Projects => context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Scratchpads)
            .ThenInclude(s => s.Notes);

        public IQueryable<Scratchpad> Scratchpads => context.Scratchpads
            .Include(s => s.Notes)
            .Include(s => s.Owner);

        public IQueryable<Note> Notes => context.Notes;

        public IQueryable<User> Users => context.Users;

        public async Task DeleteNoteAsync(int noteId)
        {
            var note = Notes.FirstOrDefault(n => n.NoteId == noteId);
            if (note == null)
                throw new ArgumentException("note not found", nameof(noteId));

            context.Remove(note);
            await context.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(int projectId)
        {
            var project = Projects.FirstOrDefault(p => p.ProjectId == projectId);
            if (project == null)
                throw new ArgumentException("project not found", nameof(projectId));

            context.Remove(project);
            await context.SaveChangesAsync();
        }

        public async Task DeleteScratchpadAsync(int scratchpadId)
        {
            var scratchpad = Scratchpads.FirstOrDefault(s => s.ScratchpadId == scratchpadId);
            if (scratchpad == null)
                throw new ArgumentException("scratchpad not found", nameof(scratchpadId));

            context.Remove(scratchpad);
            await context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = Users.FirstOrDefault(user => user.UserId == userId);
            if (user == null)
                throw new ArgumentException("user not found", nameof(userId));

            context.Remove(user);
            await context.SaveChangesAsync();
        }

        public async Task SaveNoteAsync(Note note)
        {
            if (note.NoteId == 0)
            {
                context.Notes.Add(note);
            }
            else
            {
                var db = Notes.First(n => n.NoteId == note.NoteId);
                db.Title = note.Title;
                db.Content = note.Content;
                db.Created = note.Created;
                db.CreatedBy = note.CreatedBy;
                db.Height = note.Height;
                db.Width = note.Width;
                db.Modified = note.Modified;
                db.ModifiedBy = note.ModifiedBy;
                db.PosX = note.PosX;
                db.PosY = note.PosY;
                db.ScratchpadId = note.ScratchpadId;
            }

            await context.SaveChangesAsync();
        }

        public async Task SaveProjectAsync(Project proj)
        {
            if (proj.ProjectId == 0)
            {
                context.Projects.Add(proj);
            }
            else
            {
                var db = Projects.First(p => p.ProjectId == proj.ProjectId);
                db.Title = proj.Title;
            }

            await context.SaveChangesAsync();
        }

        public async Task SaveScratchpadAsync(Scratchpad sp)
        {
            if (sp.ScratchpadId == 0)
            {
                context.Scratchpads.Add(sp);
            }
            else
            {
                var db = Scratchpads.First(s => s.ScratchpadId == sp.ScratchpadId);
                db.LastAccess = sp.LastAccess;
                db.ProjectId = sp.ProjectId;
                db.Title = sp.Title;
            }

            await context.SaveChangesAsync();
        }

        public async Task SaveUserAsync(User user)
        {
            if (user.UserId == 0)
            {
                context.Users.Add(user);
            }
            else
            {
                var db = Users.First(u => u.UserId == user.UserId);
                db.Name = user.Name;
                db.Email = user.Email;
                db.Passwort = user.Passwort;
                db.Admin = user.Admin;
            }

            await context.SaveChangesAsync();
        }
    }
}
