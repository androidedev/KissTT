///
/// https://docs.microsoft.com/es-es/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/creating-an-entity-framework-data-model-for-an-asp-net-mvc-application
/// 
/// Note:
///   Entity Framework interprets a property as a foreign key property if it's named "navigation property name"+"primary key property name" (for example, StudentID for the Student 
///   navigation property since the Student entity's primary key is ID). 
///   Foreign key properties can also be named the same simply "primary key property name" (for example, CourseID since the Course entity's primary key is CourseID).
///
/// Code first and migrations examples :
/// 
/// Update-Database -ProjectName ktt3 -Verbose -Force
/// Update-Database -ProjectName ktt3 -Verbose              
/// Add-Migration Initial -ProjectName ktt3                                
/// Add-Migration Initial -IgnoreChanges -ProjectName ktt3
/// Add-Migration -ProjectName ktt3 Preview                 > we can add a migration and then delete it to preview what changes will be done in next scaffolding
/// Update-Database -ProjectName ktt3 -Verbose -Script      > generate a script of changes without actually modify the DB 
/// 
namespace ktt3.DataAccess
{
    using Model;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class Ktt3DbContext : DbContext
    {

        public Ktt3DbContext() : base("name=ktt3Connection")
        {
            // set in app.config : entityFramework > contexts > context > databaseInitializer 
            //Database.SetInitializer<ktt3DbContext>(new DBInitialize());
        }

        /// <summary>
        /// You could have omitted the DbSet<Job> and DbSet<TimeTrack> statements and it would work the same. 
        /// The Entity Framework would include them implicitly because the Project entity references the Job entity and the Job entity references the TimeTrack entity.
        /// 
        /// I'm using the inner collections of Projects, but i left the other two for convenience because I'm using it in some places like exporter
        /// </summary>

        public DbSet<Project> Projects { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<TimeTrack> TimeTracks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Job>()
                .Property(e => e.Status)
                .IsFixedLength();

            modelBuilder.Entity<Job>()
                .HasMany(e => e.TimeTracks)
                .WithRequired(e => e.Job)
                .HasForeignKey(e => e.JobID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Jobs)
                .WithRequired(e => e.Project)
                .HasForeignKey(e => e.ProjectID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Project>()
                .Property(p => p.ProjectID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Job>()
                .Property(j => j.JobID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<TimeTrack>()
                .Property(tt => tt.TimeTrackID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        }

    }

}
