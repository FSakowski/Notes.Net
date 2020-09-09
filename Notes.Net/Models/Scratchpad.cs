using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Notes.Net.Models
{
    public class Scratchpad 
    {
        [Key]
        public int ScratchpadId { get; set; }

        [Required]
        [MaxLength(40)]
        public string Title { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [JsonIgnore]
        public ICollection<Note> Notes { get; set; }

        public DateTime LastAccess { get; set; }

        public User Owner { get; set; }
    }
}
