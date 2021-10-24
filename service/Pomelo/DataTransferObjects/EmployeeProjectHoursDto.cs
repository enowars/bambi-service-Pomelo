namespace Pomelo.DataTransferObjects
{
    using Pomelo.Models;

    public record EmployeeProjectHoursDto(
        long EmployeeId,
        long ProjectId,
        long TotalHours,
        long DeliveredHours)
    {
        public EmployeeProjectHoursDto(EmployeeProjectHours eph)
            : this(
                  eph.EmployeeId,
                  eph.ProjectId,
                  eph.TotalHours,
                  eph.DeliveredHours)
        {
        }
    }
}
