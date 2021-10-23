namespace Pomelo.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pomelo.Models;

    public record ProjectDto(
        long Id,
        string Name,
        DateTime Begin,
        DateTime End,
        DateTime DeliveredHoursTimestamp,
        List<EmployeeProjectHoursDto> EmployeeProjectHours,
        List<EmployeeProjectWeeklyCapacityDto> EmployeeProjectWeeklyCapacities)
    {
        public ProjectDto(Project project)
            : this(
                  project.Id,
                  project.Name,
                  project.Begin,
                  project.End,
                  project.DeliveredHoursTimestamp,
                  project.EmployeeProjectHours.Select(e => new EmployeeProjectHoursDto(e)).ToList(),
                  project.EmployeeProjectWeeklyCapacities.Select(e => new EmployeeProjectWeeklyCapacityDto(e)).ToList())
        {
        }
    }
}
