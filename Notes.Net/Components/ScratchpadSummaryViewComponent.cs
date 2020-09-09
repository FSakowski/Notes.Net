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
            return View(new ScratchpadSummaryViewModel
            {
                Scratchpads = noteService.Scratchpads,
            });
        }
    }
}
