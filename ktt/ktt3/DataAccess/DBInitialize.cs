namespace ktt3.DataAccess
{
    //class DBInitialize : System.Data.Entity.DropCreateDatabaseIfModelChanges<ktt3DbContext>
    //class DBInitialize : System.Data.Entity.DropCreateDatabaseAlways<ktt3DbContext>
    class DBInitialize : System.Data.Entity.CreateDatabaseIfNotExists<Ktt3DbContext>
    {
        protected override void Seed(Ktt3DbContext dataContext)
        {
            var s = new DBSeedData();
            s.Seed(dataContext, false);
            base.Seed(dataContext);
        }

    }
}
