namespace Pomelo.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pomelo.Models;

    public record ProjectInfoDto(
        string Name,
        DateTime Begin,
        DateTime End,
        DateTime DeliveredHoursTimestamp)
    {
        public ProjectInfoDto(Project project)
            : this(
                  project.Name,
                  project.Begin,
                  project.End,
                  project.DeliveredHoursTimestamp)
        {
        }
    }
}
