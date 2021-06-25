namespace Mafmax.DepartmentsDirectory.AspNetApp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Mafmax.DepartmentsDirectory.AspNetApp.Models.DDContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Mafmax.DepartmentsDirectory.AspNetApp.Models.DDContext context)
        {
         
        }
    }
}
