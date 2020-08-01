using System;
using System.ComponentModel.DataAnnotations;

namespace Notes.Net.Models
{
    public class NoteVersion
    {
        [Key]
        public int NoteVersionId { get; set; }

        public Note Origin { get; set; }

        public string Content { get; set; }

        public DateTime EntryDate { get; set; }

        public User TriggerdBy { get; set; }

    }
}
