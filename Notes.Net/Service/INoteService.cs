using Notes.Net.Models;
using System.Linq;

namespace Notes.Net.Service
{
    public interface INoteService
    {
        public IQueryable<Project> Projects { get; }

        public IQueryable<Scratchpad> Scratchpads { get; }

        public IQueryable<Note> Notes { get; }

        public void SaveScratchpad(Scratchpad sp);

        public void SaveNote(Note note);

        public void SaveProject(Project proj);

        public void DeleteScratchpad(Scratchpad sp);

        public void DeleteNote(Note note);
    }
}
