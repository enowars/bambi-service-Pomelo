namespace Pomelo.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Vacation
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public long Id { get; set; }

        public long EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public long CalendarWeek { get; set; }

        public long AbsentDays { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
