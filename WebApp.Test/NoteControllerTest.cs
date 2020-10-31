using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Notes.Net.Controllers;
using Notes.Net.Models;
using Notes.Net.Models.ViewModels;
using Notes.Net.Service;
using System;
using System.IO;
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

        [Fact]
        public void Can_edit_note_content()
        {
            Mock<INoteService> mock = new Mock<INoteService>();
            mock.Setup(m => m.Notes).Returns((new Note[]
            {
                new Note()
                {
                    NoteId = 1,
                    Content = "Test"
                }
            }).AsQueryable());

            INoteService service = mock.Object;

            var data = "New Content";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            NoteController controller = new NoteController(service)
            {
                ControllerContext = controllerContext
            };

            var result = controller.Save(1);

            Assert.Equal(data, service.Notes.First(n => n.NoteId == 1).Content);
        }

        [Fact]
        public void Sanitize_note_content()
        {
            Mock<INoteService> mock = new Mock<INoteService>();
            mock.Setup(m => m.Notes).Returns((new Note[]
            {
                new Note()
                {
                    NoteId = 1,
                    Content = "Test"
                }
            }).AsQueryable());

            INoteService service = mock.Object;

            var data = "<script>alert('test');</script>";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            NoteController controller = new NoteController(service)
            {
                ControllerContext = controllerContext
            };

            var result = controller.Save(1).Result;

            Assert.IsType<OkResult>(result);
            Assert.Equal(string.Empty, service.Notes.First(n => n.NoteId == 1).Content);
        }

        [Fact]
        public void Can_delete_a_note()
        {
            Mock<IRepository> mock = new Mock<IRepository>();
            mock.Setup(m => m.Notes).Returns((new Note[]
            {
                new Note()
                {
                    NoteId = 1,
                    Content = "Test"
                }
            }).AsQueryable());

            Mock<IServiceContext> mock2 = new Mock<IServiceContext>();
            mock2.Setup(m => m.GetCurrentUser()).ReturnsAsync(new User()
            {
                UserId = 1,
                Name = "admin"
            });

            NoteService service = new NoteService(mock2.Object, mock.Object);

            var controller = new NoteController(service);
            var result = controller.Delete(1);

            Assert.IsType<OkResult>(result);
            mock.Verify(m => m.DeleteNoteAsync(1), Times.Once);
        }

        [Fact]
        public void Test_metadata_is_updated()
        {
            DateTime start = DateTime.Now;

            Mock<IRepository> mock = new Mock<IRepository>();
            mock.Setup(m => m.Notes).Returns((new Note[]
            {
                new Note()
                {
                    NoteId = 1,
                    Content = "Test",
                    Modified = new DateTime(2020, 1, 1, 12, 00, 00),
                    ScratchpadId = 1
                }
            }).AsQueryable());

            mock.Setup(m => m.Scratchpads).Returns((new Scratchpad[]
            {
                new Scratchpad() { ScratchpadId = 1}
            }).AsQueryable());

            Mock<IServiceContext> mock2 = new Mock<IServiceContext>();
            mock2.Setup(m => m.GetCurrentUser()).ReturnsAsync(new User()
            {
                UserId = 1,
                Name = "admin"
            });

            NoteService service = new NoteService(mock2.Object, mock.Object);

            var data = "Test 123";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            NoteController controller = new NoteController(service)
            {
                ControllerContext = controllerContext
            };

            var result = controller.Save(1).Result;
            Assert.IsType<OkResult>(result);
            Assert.Equal("Test 123", service.Notes.First(n => n.NoteId == 1).Content);
            Assert.True(service.Notes.First(n => n.NoteId == 1).Modified >= start);
        }
    }
}
