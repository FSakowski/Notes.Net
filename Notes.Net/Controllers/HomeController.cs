using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes.Net.Models.ViewModels;
using Notes.Net.Service;
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
            int totalSPCount = await noteService.Scratchpads.CountAsync();
            int totalNCount = await noteService.Notes.CountAsync();
            int totalContent = await noteService.Notes.SumAsync(n => n.Content == null ? 0 : n.Content.Length);

            return View(new UsageViewModel()
            {
                TotalScratchpads = totalSPCount,
                TotalNotes = totalNCount,
                TotalContent = totalContent
            });
        }
    }
}
