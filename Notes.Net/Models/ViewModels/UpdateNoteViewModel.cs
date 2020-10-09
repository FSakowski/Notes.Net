using System.ComponentModel.DataAnnotations;

namespace Notes.Net.Models.ViewModels
{
    public class UpdateNoteViewModel
    {
        public int NoteId { get; set; }

        [Required]
        [MaxLength(40)]
        public string Title { get; set; }

        [Required]
        public int? Project { get; set; }

        [Required]
        public int? Scratchpad { get; set; }

        public string Content { get; set; }
    }
}
