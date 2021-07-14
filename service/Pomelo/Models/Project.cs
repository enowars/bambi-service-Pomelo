namespace Pomelo.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Project
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public long Id { get; set; }

        public long OwnerId { get; set; }

        public Employee Owner { get; set; }

        public string Name { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public List<TotalPlanning> TotalPlannings { get; set; }

        public List<WeeklyCapacity> WeeklyCapacities { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
