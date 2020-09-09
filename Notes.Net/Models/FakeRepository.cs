using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Net.Models
{
    public class FakeRepository : IRepository
    {
        private readonly List<Project> projects = new List<Project>();

        private readonly List<Scratchpad> scratchpads = new List<Scratchpad>();

        private readonly List<Note> notes = new List<Note>();

        private readonly User CurrentUser = new User()
        {
            Admin = true,
            Name = "Florian",
            UserId = 1
        };

        public IQueryable<Project> Projects => projects.AsQueryable();

        public IQueryable<Scratchpad> Scratchpads => scratchpads.AsQueryable();

        public IQueryable<Note> Notes => notes.AsQueryable();

        public FakeRepository()
        {
            var proj = new Project()
            {
                ProjectId = 1,
                Title = "Uni"
            };

            proj.Scratchpads = new List<Scratchpad>() {
                new Scratchpad{
                    ScratchpadId = 1,
                    Title = "Fernuni Allgemein",
                    LastAccess = new DateTime(2020, 03, 13, 12, 40, 20),
                    ProjectId = 1,
                    Notes = new List<Note>()
                    {
                        new Note()
                        {
                            Title = "Bevorstehende Termine",
                            Content = "Test",
                            Created = new DateTime(2020, 02, 20, 14, 30, 23),
                            CreatedBy = CurrentUser,
                            Height = 400,
                            Width = 500,
                            PosX = 50,
                            PosY = 120,
                            Modified = new DateTime(2020, 2, 20, 16, 50, 12),
                            ModifiedBy = CurrentUser,
                            NoteId = 1,
                            ScratchpadId = 1
                        },
                        new Note()
                        {
                            Title = "Einsendeaufgaben",
                            Content = "<p>Das ist eine <b>Beispielnotiz</b></p>",
                            Created = new DateTime(2020, 08, 04, 18, 20, 13),
                            CreatedBy = CurrentUser,
                            Height = 300,
                            Width = 300,
                            PosX = 450,
                            PosY = 520,
                            Modified = new DateTime(2020, 8, 04, 18, 50, 50),
                            ModifiedBy = CurrentUser,
                            NoteId = 2,
                            ScratchpadId = 1
                        }
                    }
                },

                new Scratchpad
                {
                    ScratchpadId = 2,
                    Title = "Modul BWL II",
                    LastAccess = new DateTime(2020, 03, 10, 12, 40, 20),
                    ProjectId = 1,
                    Notes = new List<Note>()
                },

                new Scratchpad
                {
                    ScratchpadId = 3,
                    Title = "Modul Alg. Mathematik",
                    LastAccess = new DateTime(2019, 06, 14, 12, 40, 20),
                    ProjectId = 1,
                    Notes = new List<Note>()
                },

                new Scratchpad
                {
                    ScratchpadId = 4,
                    Title = "Modul VWL Makroökonomie",
                    LastAccess = new DateTime(2019, 09, 20, 12, 40, 20),
                    ProjectId = 1,
                    Notes = new List<Note>()
                }
            };

            projects.Add(proj);
            scratchpads.AddRange(proj.Scratchpads);
            scratchpads.ForEach(s => notes.AddRange(s.Notes));

            proj = new Project()
            {
                ProjectId = 2,
                Title = "Arbeit"
            };

            proj.Scratchpads = new List<Scratchpad>() {
                new Scratchpad
                {
                    ScratchpadId = 5,
                    Title = "Allgemeines",
                    LastAccess = new DateTime(2020, 07, 27, 20, 52, 50),
                    Notes = new List<Note>(),
                    ProjectId = 2,
                }
            };

            projects.Add(proj);
            scratchpads.AddRange(proj.Scratchpads);
        }

        public void DeleteNote(int noteId)
        {
            var note = notes.FirstOrDefault(n => n.NoteId == noteId);
            if (note == null)
                throw new ArgumentException("note not found", nameof(noteId));

            scratchpads.Where(sp => sp.Notes.Contains(note)).ToList().ForEach(sp => sp.Notes.Remove(note));
            notes.Remove(note);
        }

        public void DeleteScratchpad(int scratchpadId)
        {
            var scratchpad = scratchpads.FirstOrDefault(s => s.ScratchpadId == scratchpadId);
            if (scratchpad == null)
                throw new ArgumentException("scratchpad not found", nameof(scratchpadId));

            projects.Where(p => p.Scratchpads.Contains(scratchpad)).ToList().ForEach(p => p.Scratchpads.Remove(scratchpad));

            scratchpads.Remove(scratchpad);
        }

        public void DeleteProject(int projectId)
        {
            var project = projects.FirstOrDefault(p => p.ProjectId == projectId);
            if (project == null)
                throw new ArgumentException("project not found", nameof(projectId));

            projects.Remove(project);
        }

        public void SaveNote(Note note)
        {
            if (note.NoteId == 0)
            {
                note.NoteId = notes.Count == 0 ? 1 : notes.Last().NoteId + 1;
                notes.Add(note);

                var scratch = Scratchpads.First(s => s.ScratchpadId == note.ScratchpadId);
                scratch.Notes.Add(note);
            } else
            {
                var db = Notes.First(n => n.NoteId == note.NoteId);
                db.Title = note.Title;
                db.Content = note.Content;
                db.Created = note.Created;
                db.CreatedBy = note.CreatedBy;
                db.Height = note.Height;
                db.Width = note.Width;
                db.Modified = note.Modified;
                db.ModifiedBy = note.ModifiedBy;
                db.PosX = note.PosX;
                db.PosY = note.PosY;
                db.ScratchpadId = note.ScratchpadId;
            }
        }

        public void SaveScratchpad(Scratchpad sp)
        {
            if (sp.ScratchpadId == 0)
            {
                sp.ScratchpadId = scratchpads.Count == 0 ? 1 : scratchpads.Last().ScratchpadId + 1;
                sp.Notes = new List<Note>();
                scratchpads.Add(sp);

                var proj = Projects.First(p => p.ProjectId == sp.ProjectId);
                proj.Scratchpads.Add(sp);
            } else
            {
                var db = Scratchpads.First(s => s.ScratchpadId == sp.ScratchpadId);
                db.LastAccess = sp.LastAccess;
                db.ProjectId = sp.ProjectId;
                db.Title = sp.Title;
            }
        }

        public void SaveProject(Project proj)
        {
            if (proj.ProjectId == 0)
            {
                proj.ProjectId = projects.Count == 0 ? 1 : projects.Last().ProjectId + 1;
                proj.Owner = new Tenant()
                {
                    TenantId = 1,
                    Name = "Test"
                };

                if (proj.Scratchpads == null)
                {
                    proj.Scratchpads = new List<Scratchpad>();

                    var sp = new Scratchpad()
                    {
                        Title = "General",
                        LastAccess = DateTime.Now,
                        Owner = CurrentUser,
                        Notes = new List<Note>()
                    };
                    SaveScratchpad(sp);
                }
                projects.Add(proj);
            } else
            {
                var db = Projects.First(p => p.ProjectId == proj.ProjectId);
                db.Title = proj.Title;
            }
        }
    }
}
