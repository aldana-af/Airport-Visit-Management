using Microsoft.EntityFrameworkCore;
using AirportVisitSystem.Models;

namespace AirportVisitSystem.Data
{
    public class AirportVisitDb : DbContext
    {
        public AirportVisitDb(DbContextOptions<AirportVisitDb> options) : base(options)
        { }

        // DbSets for existing models. Add additional DbSet<> properties as needed.
        public DbSet<EmployeeHost> EmployeeHosts { get; set; }
        public DbSet<SiteVisitingManager> SiteVisitingManagers { get; set; }
        public DbSet<Logins> Logins { get; set; }
        public DbSet<Visitor> Visitor { get; set; }
        public DbSet<Visit> Visit { get; set; }
        public DbSet<VisitType> VisitType { get; set; }
        public DbSet<Department> Department { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Logins>().ToTable("Logins");
            modelBuilder.Entity<EmployeeHost>().ToTable("EmployeeHost");
            modelBuilder.Entity<SiteVisitingManager>().ToTable("SiteVisitingManager");
            modelBuilder.Entity<Visitor>().ToTable("Visitor");
            modelBuilder.Entity<Visit>().ToTable("Visit");
            modelBuilder.Entity<VisitType>().ToTable("VisitType");
            modelBuilder.Entity<Department>().ToTable("Department");
        }
    }
}
