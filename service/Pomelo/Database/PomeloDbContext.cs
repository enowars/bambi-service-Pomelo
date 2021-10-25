namespace Pomelo.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Pomelo.Models;

    public class PomeloDbContext : DbContext
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public PomeloDbContext(DbContextOptions<PomeloDbContext> contextOptions)
            : base(contextOptions)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<EmployeeProjectWeeklyCapacity> EmployeeProjectWeeklyCapacities { get; set; }

        public DbSet<EmployeeProjectHours> EmployeeProjectHours { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(e =>
            {
                e.HasIndex(e => e.Department);
                e.HasIndex(e => e.Name);
            });

            modelBuilder.Entity<EmployeeProjectWeeklyCapacity>(e =>
            {
                e.HasKey(e => new { e.EmployeeId, e.ProjectId });
            });

            modelBuilder.Entity<EmployeeProjectHours>(e =>
            {
                e.HasKey(e => new { e.EmployeeId, e.ProjectId });
            });
        }
    }
}
