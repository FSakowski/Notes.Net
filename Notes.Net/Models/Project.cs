using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Notes.Net.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(40)]
        public string Title { get; set; }

        [JsonIgnore]
        public ICollection<Scratchpad> Scratchpads { get; set; }

        public Tenant Owner { get; set; }
    }
}
