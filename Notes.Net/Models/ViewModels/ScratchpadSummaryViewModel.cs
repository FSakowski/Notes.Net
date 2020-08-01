using System.Collections.Generic;

namespace Notes.Net.Models.ViewModels
{
    public class ScratchpadSummaryViewModel
    {
        public IEnumerable<Scratchpad> Scratchpads { get; set; }
        public Scratchpad ActiveScratchpad { get; set; }
    }
}
