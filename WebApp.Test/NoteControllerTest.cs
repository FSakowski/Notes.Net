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
    public class NoteControllerTest
    {
        [Fact]
        public void Can_save_position_of_a_note()
        {
            Mock<INoteService> mock = new Mock<INoteService>();
            mock.Setup(m => m.Notes).Returns((new Note[]
            {
                new Note()
                {
                    NoteId = 1,
                    PosX = 10,
                    PosY = 10,
                }
            }).AsQueryable());

            INoteService service = mock.Object;
            NoteController controller = new NoteController(service);
            var result = controller.SavePosition(1, 50, 100);

            Assert.Equal(50, service.Notes.First(n => n.NoteId == 1).PosX);
            Assert.Equal(100, service.Notes.First(n => n.NoteId == 1).PosY);
        }

        [Fact]
        public void Can_save_size_of_a_note()
        {
            Mock<INoteService> mock = new Mock<INoteService>();
            mock.Setup(m => m.Notes).Returns((new Note[]
            {
                new Note()
                {
                    NoteId = 1,
                    Width = 100,
                    Height = 200,
                }
            }).AsQueryable());

            INoteService service = mock.Object;
            NoteController controller = new NoteController(service);
            var result = controller.SaveSize(1, 400, 500);

            Assert.Equal(400, service.Notes.First(n => n.NoteId == 1).Width);
            Assert.Equal(500, service.Notes.First(n => n.NoteId == 1).Height);
        }
    }
}
