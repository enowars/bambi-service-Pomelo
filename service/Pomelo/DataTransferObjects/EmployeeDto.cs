namespace Pomelo.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pomelo.Models;

    public record EmployeeDto(
        long Id,
        string Name,
        string? Note)
    {
        public EmployeeDto(Employee e)
            : this(
                  e.Id,
                  e.Name,
                  e.Note)
        {
        }
    }
}
