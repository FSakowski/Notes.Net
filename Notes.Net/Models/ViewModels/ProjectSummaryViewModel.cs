using System.Collections.Generic;

namespace Notes.Net.Models.ViewModels
{
    public class ProjectSummaryViewModel
    {
        public IEnumerable<Project> Projects { get; set; }

        public Project ActiveProject { get; set; }

        public Scratchpad ActiveScratchpad { get; set; }
    }
}
