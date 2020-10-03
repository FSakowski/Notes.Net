using System.ComponentModel.DataAnnotations;

namespace Notes.Net.Models.ViewModels
{
    public class QuickNoteViewModel
    {
        [Required]
        public string Content { get; set; }
    }
}
