namespace Pomelo.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pomelo.Models;

    public record ProjectInfoDto(
        long Id,
        string Name,
        DateTime Begin,
        DateTime End,
        DateTime DeliveredHoursTimestamp)
    {
        public ProjectInfoDto(Project project)
            : this(
                  project.Id,
                  project.Name,
                  project.Begin,
                  project.End,
                  project.DeliveredHoursTimestamp)
        {
        }
    }
}
