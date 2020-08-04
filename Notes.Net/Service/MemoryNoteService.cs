using Notes.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Notes.Net.Service
{
    public class MemoryNoteService : INoteService
    {
        private readonly List<Project> projects = new List<Project>();

        private readonly List<Scratchpad> scratchpads = new List<Scratchpad>();

        private readonly List<Note> notes = new List<Note>();

        private readonly IServiceContext serviceContext;

        public MemoryNoteService(IServiceContext context)
        {
            serviceContext = context;

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
                    Notes = new List<Note>()
                    {
                        new Note()
                        {
                            Title = "Bevorstehende Termine",
                            Content = "Test",
                            Created = new DateTime(2020, 02, 20, 14, 30, 23),
                            CreatedBy = serviceContext.CurrentUser,
                            Height = 400,
                            Width = 500,
                            PosX = 50,
                            PosY = 120,
                            Modified = new DateTime(2020, 2, 20, 16, 50, 12),
                            ModifiedBy = serviceContext.CurrentUser,
                            NoteId = 1
                        },
                        new Note()
                        {
                            Title = "Einsendeaufgaben",
                            Content = "<p>Das ist eine <b>Beispielnotiz</b></p>",
                            Created = new DateTime(2020, 08, 04, 18, 20, 13),
                            CreatedBy = serviceContext.CurrentUser,
                            Height = 300,
                            Width = 300,
                            PosX = 450,
                            PosY = 520,
                            Modified = new DateTime(2020, 8, 04, 18, 50, 50),
                            ModifiedBy = serviceContext.CurrentUser,
                            NoteId = 2
                        }
                    }
                },

                new Scratchpad
                {
                    ScratchpadId = 2,
                    Title = "Modul BWL II",
                    LastAccess = new DateTime(2020, 03, 10, 12, 40, 20),
                    Notes = new List<Note>()
                },

                new Scratchpad
                {
                    ScratchpadId = 3,
                    Title = "Modul Alg. Mathematik",
                    LastAccess = new DateTime(2019, 06, 14, 12, 40, 20),
                    Notes = new List<Note>()
                },

                new Scratchpad
                {
                    ScratchpadId = 4,
                    Title = "Modul VWL Makroökonomie",
                    LastAccess = new DateTime(2019, 09, 20, 12, 40, 20),
                    Notes = new List<Note>()
                }
            };

            projects.Add(proj);
            scratchpads.AddRange(proj.Scratchpads);

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
                    Notes = new List<Note>()
                }
            };

            projects.Add(proj);
            scratchpads.AddRange(proj.Scratchpads);
        }

        public IQueryable<Project> Projects => projects.AsQueryable();

        public IQueryable<Scratchpad> Scratchpads => scratchpads.AsQueryable();

        public IQueryable<Note> Notes => notes.AsQueryable();

        public void DeleteNote(Note note)
        {
            scratchpads.Where(sp => sp.Notes.Contains(note)).ToList().ForEach(sp => sp.Notes.Remove(note));
            notes.Remove(note);
        }

        public void DeleteScratchpad(Scratchpad sp)
        {
            scratchpads.Remove(sp);
        }

        public void SaveNote(Note note)
        {
            if (note.NoteId == 0)
            {
                note.NoteId = serviceContext.GetNextFreeNumber<Note>();
                notes.Add(note);
                SaveNoteMetaData(note, true);
                note.Scratchpad.Notes.Add(note);
            } else
            {
                SaveNoteMetaData(note, false);
            }
        }

        private void SaveNoteMetaData(Note note, bool newEntry)
        {
            if (newEntry)
            {
                note.Created   = DateTime.Now;
                note.CreatedBy = serviceContext.CurrentUser;
            } else
            {
                note.Modified   = DateTime.Now;
                note.ModifiedBy = serviceContext.CurrentUser;
            }
        }

        public void SaveScratchpad(Scratchpad sp)
        {
            if (sp.ScratchpadId == 0)
            {
                sp.ScratchpadId = serviceContext.GetNextFreeNumber<Scratchpad>();
                sp.Notes = new List<Note>();
                scratchpads.Add(sp);
                sp.Project.Scratchpads.Add(sp);
            }
        }

        public void SaveProject(Project proj)
        {
            if (proj.ProjectId == 0)
            {
                proj.ProjectId = serviceContext.GetNextFreeNumber<Project>();
                proj.Owner = serviceContext.CurrentTenant;

                if (proj.Scratchpads == null)
                {
                    proj.Scratchpads = new List<Scratchpad>();

                    var sp = new Scratchpad()
                    {
                        Title = "General",
                        LastAccess = DateTime.Now,
                        Owner = serviceContext.CurrentUser,
                        Notes = new List<Note>(),
                        Project = proj
                    };
                    SaveScratchpad(sp);
                }
                projects.Add(proj);
            }
        }
    }
}
