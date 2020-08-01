using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notes.Net.Models
{
    public class Scratchpad 
    {
        [Key]
        public int ScratchpadId { get; set; }

        [Required]
        [MaxLength(40)]
        public string Title { get; set; }

        public Project Project { get; set; }

        public ICollection<Note> Notes { get; set; }

        public DateTime LastAccess { get; set; }

        public User Owner { get; set; }
    }
}
