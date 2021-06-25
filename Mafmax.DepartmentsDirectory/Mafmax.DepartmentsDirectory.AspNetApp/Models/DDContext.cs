using System;
using System.Data.Entity;
using System.Linq;

namespace Mafmax.DepartmentsDirectory.AspNetApp.Models
{
    public class DDContext : DbContext
    {
#if DEBUG
        const string connectionName = "DDContext";
#else
        const string connectionName = "DDContextRelease";
#endif
        static DDContext()
        {
            ClearWithoutParents();
        }
        private static void ClearWithoutParents()
        {
            using (var context = new DDContext())
            {
                context.Companies.Load();
                context.Departments.Load();
                context.Groups.Load();
                context.Departments.RemoveRange(context.Departments.Where(x => x.Company == null));
                context.Groups.RemoveRange(context.Groups.Where(x => x.Department == null));
                context.SaveChanges();
            }
        }
        public DDContext()
            : base($"name={connectionName}")
        {
        }

        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<DepartmentEntity> Departments { get; set; }
        public DbSet<CompanyEntity> Companies { get; set; }

    }


}