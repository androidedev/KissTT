namespace ktt3.Migrations
{
    using DataAccess;
    using Model;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ktt3.DataAccess.Ktt3DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ktt3.DataAccess.Ktt3DbContext context)
        {
            //  This method will be called after migrating to the latest version.
            DBSeedData seeder = new DBSeedData();
            seeder.Seed(context, true);

        }
    }
}
