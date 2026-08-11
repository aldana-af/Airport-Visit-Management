using Microsoft.EntityFrameworkCore;
using AirportVisitSystem.Models;

namespace AirportVisitSystem.Data
{
    public class AirportVisitDb : DbContext
    {
        public AirportVisitDb(DbContextOptions<AirportVisitDb> options) : base(options)
        { }

        // DbSets for existing models. Add additional DbSet<> properties as needed.
        public DbSet<Logins> Logins { get; set; }
        public DbSet<EmployeeHost> EmployeeHosts { get; set; }
        public DbSet<SiteVisitingManager> SiteVisitingManagers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<VisitType> VisitTypes { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<VisitVisitor> VisitVisitors { get; set; }
        public DbSet<Approval> Approvals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Logins>().ToTable("Logins").HasKey(l => l.LoginID);
            modelBuilder.Entity<EmployeeHost>().ToTable("EmployeeHost").HasKey(e => e.EmployeeID);
            modelBuilder.Entity<SiteVisitingManager>().ToTable("SiteVisitingManager").HasKey(m => m.ManagerID);
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<VisitType>().ToTable("VisitType");
            modelBuilder.Entity<Visitor>().ToTable("Visitor");
            modelBuilder.Entity<Visit>().ToTable("Visit");
            modelBuilder.Entity<VisitVisitor>().ToTable("VisitVisitor");
            modelBuilder.Entity<Approval>().ToTable("Approval");
        }
    }
}
