///
/// Indentity insertion, see :
/// https://stackoverflow.com/questions/13086006/how-can-i-force-entity-framework-to-insert-identity-columns
/// https://msdn.microsoft.com/es-es/library/ms176057.aspx
/// dataContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Project] ON"); > Doesn't allow intentity insert almost you change also the model creation 
/// OnModelCreating(...) so entities have DatabaseGeneratedOption.None, so instead dealing with changes we are using "DBCC CHECKIDENT..."
/// 
using ktt3.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity;

namespace ktt3.DataAccess
{
    public class DBSeedData
    {
        private void Add<T>(List<T> items, DbSet<T> dbSet, bool AddOrUpdate) where T : class
        {
            if (AddOrUpdate)
                items.ForEach(p => dbSet.AddOrUpdate<T>(p));
            else
                items.ForEach(p => dbSet.Add(p));
        }

        public void Seed(ktt3.DataAccess.Ktt3DbContext dataContext, bool AddOrUpdate=false )
        {
            // Delete all data from tables before seed (except __MigrationHistory)
            // WARN: sp_MSForEachTable is not documented and is supposed only to internal use so better doesn't use this way and go for manual delete
            //dataContext.Database.ExecuteSqlCommand("sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");
            //dataContext.Database.ExecuteSqlCommand("sp_MSForEachTable 'IF OBJECT_ID(''?'') NOT IN (ISNULL(OBJECT_ID(''[dbo].[__MigrationHistory]''),0)) DELETE FROM ?'");
            //dataContext.Database.ExecuteSqlCommand("EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'");

            // Delete all data from tables before seed
            dataContext.Database.ExecuteSqlCommand("DELETE FROM TimeTrack");
            dataContext.Database.ExecuteSqlCommand("DELETE FROM Job");
            dataContext.Database.ExecuteSqlCommand("DELETE FROM Project");

            // Create projects
            var projects = new List<Project>
            {
                new Project { ProjectID=1, Name="Project Leave Matrix" },
                new Project { ProjectID=2, Name="Project Make a pie" },
                new Project { ProjectID=3, Name="Project Climb Everest" },
            };
            dataContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT ([Project], RESEED, 0)");
            this.Add<Project>(projects, dataContext.Projects, true);
            dataContext.SaveChanges();

            // Create jobs
            var jobs = new List<Job>
            {
                new Job { JobID=1, ProjectID=1, Priority=1, Status = "+", Description="Follow white rabbit", EstimatedTime=100  },
                new Job { JobID=2, ProjectID=1, Priority=1, Status = "-", Description="Take red pill", EstimatedTime=100  },
                new Job { JobID=3, ProjectID=1, Priority=1, Status = "", Description="Kill agent Smith", EstimatedTime=null  },
                new Job { JobID=4, ProjectID=2, Priority=1, Status = "+", Description="Peel apples", EstimatedTime=null  },
                new Job { JobID=5, ProjectID=2, Priority=1, Status = "-", Description="Make cream", EstimatedTime=null  },
                new Job { JobID=6, ProjectID=2, Priority=1, Status = "", Description="Bake it", EstimatedTime=null  },
                new Job { JobID=7, ProjectID=3, Priority=1, Status = null, Description="Buy equipment", EstimatedTime=null  },
                new Job { JobID=8, ProjectID=3, Priority=1, Status = "%", Description="Travel to Everest", EstimatedTime=null  },
                new Job { JobID=9, ProjectID=3, Priority=1, Status = "-", Description="Climb", EstimatedTime=null  },


                new Job { JobID=10, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=11, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=12, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=13, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=14, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=15, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=16, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=17, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=18, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=19, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=20, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=21, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=22, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=23, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=24, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=25, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=26, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=27, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=28, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=29, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=30, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
                new Job { JobID=31, ProjectID=3, Priority=2, Status = "-", Description="Climb", EstimatedTime=null  },
            };

            dataContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT ([Job], RESEED, 0)");
            this.Add<Job>(jobs, dataContext.Jobs, true);
            dataContext.SaveChanges();

            // Create TimeTracks
            var timeTracks = new List<TimeTrack>
            {
                new TimeTrack { TimeTrackID=01, JobID=1, WorkDate=DateTime.Now.AddDays(0), StartTime=DateTime.Now.AddDays(0), EndTime = DateTime.Now.AddDays(0).AddHours(1) },
                new TimeTrack { TimeTrackID=02, JobID=1, WorkDate=DateTime.Now.AddDays(1), StartTime=DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1) },
                new TimeTrack { TimeTrackID=03, JobID=1, WorkDate=DateTime.Now.AddDays(2), StartTime=DateTime.Now.AddDays(2), EndTime = DateTime.Now.AddDays(2).AddHours(1) },
                new TimeTrack { TimeTrackID=04, JobID=1, WorkDate=DateTime.Now.AddDays(3), StartTime=DateTime.Now.AddDays(3), EndTime = DateTime.Now.AddDays(3).AddHours(1) },
                new TimeTrack { TimeTrackID=05, JobID=1, WorkDate=DateTime.Now.AddDays(4), StartTime=DateTime.Now.AddDays(4), EndTime = DateTime.Now.AddDays(4).AddHours(1) },

                new TimeTrack { TimeTrackID=06, JobID=2, WorkDate=DateTime.Now.AddDays(0), StartTime=DateTime.Now.AddDays(0), EndTime = DateTime.Now.AddDays(0).AddHours(1) },
                new TimeTrack { TimeTrackID=07, JobID=2, WorkDate=DateTime.Now.AddDays(1), StartTime=DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1) },
                new TimeTrack { TimeTrackID=08, JobID=2, WorkDate=DateTime.Now.AddDays(2), StartTime=DateTime.Now.AddDays(2), EndTime = DateTime.Now.AddDays(2).AddHours(1) },
                new TimeTrack { TimeTrackID=09, JobID=2, WorkDate=DateTime.Now.AddDays(3), StartTime=DateTime.Now.AddDays(3), EndTime = DateTime.Now.AddDays(3).AddHours(1) },
                new TimeTrack { TimeTrackID=10, JobID=2, WorkDate=DateTime.Now.AddDays(4), StartTime=DateTime.Now.AddDays(4), EndTime = DateTime.Now.AddDays(4).AddHours(1) },

                new TimeTrack { TimeTrackID=11, JobID=4, WorkDate=DateTime.Now.AddDays(0), StartTime=DateTime.Now.AddDays(0), EndTime = DateTime.Now.AddDays(0).AddHours(1) },
                new TimeTrack { TimeTrackID=12, JobID=4, WorkDate=DateTime.Now.AddDays(1), StartTime=DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1) },
                new TimeTrack { TimeTrackID=13, JobID=4, WorkDate=DateTime.Now.AddDays(2), StartTime=DateTime.Now.AddDays(2), EndTime = DateTime.Now.AddDays(2).AddHours(1) },
                new TimeTrack { TimeTrackID=14, JobID=4, WorkDate=DateTime.Now.AddDays(3), StartTime=DateTime.Now.AddDays(3), EndTime = DateTime.Now.AddDays(3).AddHours(1) },
                new TimeTrack { TimeTrackID=15, JobID=4, WorkDate=DateTime.Now.AddDays(4), StartTime=DateTime.Now.AddDays(4), EndTime = DateTime.Now.AddDays(4).AddHours(1) },

                new TimeTrack { TimeTrackID=16, JobID=7, WorkDate=DateTime.Now.AddDays(0), StartTime=DateTime.Now.AddDays(0), EndTime = DateTime.Now.AddDays(0).AddHours(1) },
                new TimeTrack { TimeTrackID=17, JobID=7, WorkDate=DateTime.Now.AddDays(1), StartTime=DateTime.Now.AddDays(1), EndTime = null },
                new TimeTrack { TimeTrackID=18, JobID=7, WorkDate=DateTime.Now.AddDays(2), StartTime=DateTime.Now.AddDays(2), EndTime = null },
                new TimeTrack { TimeTrackID=19, JobID=7, WorkDate=DateTime.Now.AddDays(3), StartTime=DateTime.Now.AddDays(3), EndTime = DateTime.Now.AddDays(3).AddHours(1) },
                new TimeTrack { TimeTrackID=20, JobID=7, WorkDate=DateTime.Now.AddDays(4), StartTime=DateTime.Now.AddDays(4), EndTime = DateTime.Now.AddDays(4).AddHours(1) },
            };

            dataContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT ([TimeTrack], RESEED, 0)");
            this.Add<TimeTrack>(timeTracks, dataContext.TimeTracks, true);
            dataContext.SaveChanges();

        }
    }
}
