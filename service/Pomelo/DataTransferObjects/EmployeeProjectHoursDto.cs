namespace Pomelo.DataTransferObjects
{
    using Pomelo.Models;

    public record EmployeeProjectHoursDto(
        long EmployeeId,
        string EmployeeName,
        long TotalHours,
        long DeliveredHours)
    {
        public EmployeeProjectHoursDto(EmployeeProjectHours eph)
            : this(
                  eph.EmployeeId,
                  eph.Employee.Name,
                  eph.TotalHours,
                  eph.DeliveredHours)
        {
        }
    }
}
