using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Net.Models.ViewModels
{
    public class CreateScratchpadViewModel
    {
        [Required]
        [MaxLength(40)]
        public string Title { get; set; }

        [Required]
        public int? Project { get; set; }
    }
}
