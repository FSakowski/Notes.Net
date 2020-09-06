using System.Linq;

namespace Notes.Net.Models
{
    public interface IRepository
    {
        public IQueryable<Project> Projects { get; }

        public IQueryable<Scratchpad> Scratchpads { get; }

        public IQueryable<Note> Notes { get; }

        public void SaveScratchpad(Scratchpad sp);

        public void SaveNote(Note note);

        public void SaveProject(Project proj);

        public void DeleteScratchpad(int sratchpadId);

        public void DeleteNote(int noteId);

        public void DeleteProject(int projectId);
    }
}
