using System.Linq;
using System.Threading.Tasks;

namespace Notes.Net.Models
{
    public interface IRepository
    {
        public IQueryable<Project> Projects { get; }

        public IQueryable<Scratchpad> Scratchpads { get; }

        public IQueryable<Note> Notes { get; }

        public IQueryable<User> Users { get; }

        public Task SaveScratchpadAsync(Scratchpad sp);

        public Task SaveNoteAsync(Note note);

        public Task SaveProjectAsync(Project proj);

        public Task DeleteScratchpadAsync(int scratchpadId);

        public Task DeleteNoteAsync(int noteId);

        public Task DeleteProjectAsync(int projectId);

        public Task SaveUserAsync(User user);

        public Task DeleteUserAsync(int userId);
    }
}
