using Moq;
using Notes.Net.Controllers;
using Notes.Net.Models;
using Notes.Net.Models.ViewModels;
using Notes.Net.Service;
using System;
using System.Linq;
using Xunit;

namespace WebApp.Test
{
    public class HomeControllerTests
    {
        [Fact]
        public void Can_Calculate_Statistics()
        {
            Mock<INoteService> mock = new Mock<INoteService>();
            mock.Setup(m => m.Scratchpads).Returns((new Scratchpad[]
            {
                new Scratchpad {ScratchpadId = 1, Title = "SP 1"},
                new Scratchpad {ScratchpadId = 2, Title = "SP 2"},
                new Scratchpad {ScratchpadId = 3, Title = "SP 3"}
            }).AsQueryable());

            string[] content = new string[]
            {
                "Sample",
                "elpmas"
            };

            mock.Setup(m => m.Notes).Returns((new Note[]
            {
                new Note {NoteId = 1, Title = "N1", Content = content[0] },
                new Note {NoteId = 2, Title = "N2", Content = content[1] }
            }).AsQueryable());

            HomeController controller = new HomeController(mock.Object);
            var result = controller.Index().Result.ViewData.Model as UsageViewModel;

            Assert.Equal(3, result.TotalScratchpads);
            Assert.Equal(2, result.TotalNotes);
            Assert.Equal(content.Sum(s => s.Length), result.TotalContent);
        }
    }
}
