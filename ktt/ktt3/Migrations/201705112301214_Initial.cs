namespace ktt3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            // TimeTrack calculated column
            //DropColumn("dbo.TimeTrack", "WorkedTime");
            //Sql("ALTER TABLE dbo.TimeTrack ADD WorkedTime AS DATEDIFF(Minute,StartTime,EndTime)");
        }

        public override void Down()
        {
        }
    }
}
