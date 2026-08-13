using AirportVisitSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AirportVisitSystem.Data
{
    public class AirportVisitDatabase1 : DbContext
    {
        public AirportVisitDatabase1(DbContextOptions<AirportVisitDatabase1> options) : base(options) { }

        public DbSet<Logins> Logins { get; set; }
        public DbSet<EmployeeHost> EmployeeHosts { get; set; }
        public DbSet<SiteVisitingManager> SiteVisitingManagers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<VisitType> VisitTypes { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<VisitVisitor> VisitVisitors { get; set; }
        public DbSet<Approval> Approvals { get; set; }
        public DbSet<Badge> Badges { get; set; }

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
            modelBuilder.Entity<Badge>().ToTable("Badge");
        }
    }
}
