using Microsoft.AspNetCore.Mvc;
using Notes.Net.Models.ViewModels;

namespace Notes.Net.Components
{
    public class QuickNoteViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(new QuickNoteViewModel());
        }
    }
}
