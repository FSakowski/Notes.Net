using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Notes.Net.Controllers;
using Notes.Net.Models;
using Notes.Net.Models.ViewModels;
using Notes.Net.Service;

namespace Notes.Net.Components
{
    public class ScratchpadSummaryViewComponent : ViewComponent
    {
        private readonly INoteService noteService;

        public ScratchpadSummaryViewComponent(INoteService service)
        {
            noteService = service;
        }

        public IViewComponentResult Invoke()
        {
            Scratchpad activeSP = null;
            if ((string)RouteData.Values["controller"] == nameof(ScratchpadController) &&
                RouteData.Values.ContainsKey("scratchpad"))
            {
                activeSP = noteService.Scratchpads.FirstOrDefault(s => s.Title == (string)RouteData.Values["scratchpad"]);
            }

            return View(new ScratchpadSummaryViewModel
            {
                Scratchpads = noteService.Scratchpads,
                ActiveScratchpad = activeSP
            });
        }
    }
}
