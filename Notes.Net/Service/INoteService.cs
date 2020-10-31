using Notes.Net.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Net.Service
{
    public interface INoteService
    {
        public IQueryable<Project> Projects { get; }

        public IQueryable<Scratchpad> Scratchpads { get; }

        public IQueryable<Note> Notes { get; }

        public Task SaveScratchpadAsync(Scratchpad sp);

        public Task SaveNoteAsync(Note note, bool updateMetaData = true);

        public Task SaveProjectAsync(Project proj);

        public Task DeleteScratchpadAsync(Scratchpad sp);

        public Task DeleteNoteAsync(Note note);

        public Task DeleteProjectAsync(Project proj);

        public Task<Note> SaveQuickNoteAsync(string content);
    }
}
