using Microsoft.AspNetCore.Mvc;
using Notes.Net.Models.ViewModels;
using Notes.Net.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Net.Controllers
{
    public class HomeController : Controller
    {
        private readonly INoteService noteService;

        public HomeController(INoteService service)
        {
            noteService = service;
        }

        public async Task<ViewResult> Index()
        {
            Task<int> totalSPCount = Task.Run(() => noteService.Scratchpads.Count());
            Task<int> totalNCount = Task.Run(() => noteService.Notes.Count());
            Task<int> totalContent = Task.Run(() => noteService.Notes.Sum(n => n.Content.Length));

            await Task.WhenAll(totalSPCount, totalNCount, totalContent);
            return View(new UsageViewModel()
            {
                TotalScratchpads = totalSPCount.Result,
                TotalNotes = totalNCount.Result,
                TotalContent = totalContent.Result
            });
        }
    }
}
