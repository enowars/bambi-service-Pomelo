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

        public DbSet<WeeklyProjectCapacity> WeeklyProjectCapacities { get; set; }

        public DbSet<PlannedHours> PlannedHours { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
