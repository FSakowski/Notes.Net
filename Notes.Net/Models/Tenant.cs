using System.ComponentModel.DataAnnotations;

namespace Notes.Net.Models
{
    public class Tenant
    {
        [Key]
        public int TenantId { get; set; }

        public string Name { get; set; }
    }
}
