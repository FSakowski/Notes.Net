using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Notes.Net.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }

        [Required]
        [MaxLength(40)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Created { get; set; }

        public User CreatedBy { get; set; }

        public DateTime Modified { get; set; }

        public User ModifiedBy { get; set; }

        public int PosX { get; set; }

        public int PosY { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int ScratchpadId { get; set; }

        public Note()
        {

        }
    }
}
