namespace Pomelo.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Employee
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public long Id { get; set; }

        public string Name { get; set; }

        public string Department { get; set; }

        public string? Note { get; set; }

        public List<PlannedHours> PlannedHours { get; set; }

        public List<WeeklyProjectCapacity> WeeklyProjectCapacities { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
