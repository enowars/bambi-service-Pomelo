namespace Pomelo.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class EmployeeProjectHours
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public long EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public long ProjectId { get; set; }

        public Project Project { get; set; }

        public long TotalHours { get; set; }

        public long DeliveredHours { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
