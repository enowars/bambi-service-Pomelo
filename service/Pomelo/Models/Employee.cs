﻿namespace Pomelo.Models
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

        public List<TotalPlanning> TotalPlannings { get; set; }

        public List<WeeklyCapacity> WeeklyCapacities { get; set; }

        public List<Vacation> Vacations { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}