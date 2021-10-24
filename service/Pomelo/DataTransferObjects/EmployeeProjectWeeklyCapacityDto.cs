namespace Pomelo.DataTransferObjects
{
    using System;
    using Pomelo.Models;

    public record EmployeeProjectWeeklyCapacityDto(
        long EmployeeId,
        long ProjectId,
        DateTime Start,
        long Capacity)
    {
        public EmployeeProjectWeeklyCapacityDto(EmployeeProjectWeeklyCapacity epwc)
            : this(
                  epwc.EmployeeId,
                  epwc.ProjectId,
                  epwc.Start,
                  epwc.Capacity)
        {
        }
    }
}
