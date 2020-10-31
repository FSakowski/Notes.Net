using System.ComponentModel.DataAnnotations;

namespace Notes.Net.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string Name { get; set; }

        public bool Admin { get; set; }

        public string Passwort { get; set; }

        public string Email { get; set; }
    }
}
