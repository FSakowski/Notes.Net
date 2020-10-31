using Microsoft.EntityFrameworkCore;

namespace Notes.Net.Models
{
    public class NotesDbContext : DbContext
    {
        public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Scratchpad> Scratchpads { get; set; }
        public DbSet<Note> Notes { get; set; }

    }
}
