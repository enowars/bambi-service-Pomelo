namespace Pomelo.DataTransferObjects
{
    using System;
    using Pomelo.Models;

    public record EmployeeProjectWeeklyCapacityDto(
        long EmployeeId,
        DateTime Start,
        long Capacity)
    {
        public EmployeeProjectWeeklyCapacityDto(EmployeeProjectWeeklyCapacity epwc)
            : this(
                  epwc.EmployeeId,
                  epwc.Start,
                  epwc.Capacity)
        {
        }
    }
}
