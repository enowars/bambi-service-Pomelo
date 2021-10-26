namespace Pomelo.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pomelo.Models;

    public record EmployeeDetailsDto(
        long Id,
        string Name,
        string? Note,
        List<EmployeeProjectHoursDto> EmployeeProjectHours,
        List<EmployeeProjectWeeklyCapacityDto> EmployeeProjectWeeklyCapacities)
    {
        public EmployeeDetailsDto(Employee e)
            : this(
                  e.Id,
                  e.Name,
                  e.Note,
                  e.EmployeeProjectHours.Select(e => new EmployeeProjectHoursDto(e)).ToList(),
                  e.EmployeeProjectWeeklyCapacities.Select(e => new EmployeeProjectWeeklyCapacityDto(e)).ToList())
        {
        }
    }
}
