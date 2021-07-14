namespace Pomelo.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Pomelo.Database;
    using Pomelo.Models;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookingController : Controller
    {
        private readonly PomeloDbContext dbContext;

        public BookingController(PomeloDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(long projectId, IFormFile file)
        {
            var localPath = $"wwwroot/Uploads/{file.GetHashCode()}";
            using var stream = System.IO.File.Create(localPath);
            await file.CopyToAsync(stream);
            var lines = await System.IO.File.ReadAllLinesAsync(localPath, this.HttpContext.RequestAborted);
            var header = lines[0];
            if (header != projectId.ToString())
            {
                return this.BadRequest("projectId and file header don't match");
            }

            foreach (var line in lines.Skip(1))
            {
                var split = line.Split(',');
                var employeeId = long.Parse(split[0]);
                var hours = long.Parse(split[0]);
                var totalPlanning = await this.dbContext.TotalPlannings
                    .Where(tp => tp.ProjectId == projectId)
                    .Where(tp => tp.EmployeeId == employeeId)
                    .SingleAsync(this.HttpContext.RequestAborted);
                if (totalPlanning != null)
                {
                    totalPlanning.PerformedHours = hours;
                }
                else
                {
                    totalPlanning = new TotalPlanning()
                    {
                        ProjectId = projectId,
                        EmployeeId = employeeId,
                        PerformedHours = hours,
                        TotalHours = 0,
                    };
                    this.dbContext.TotalPlannings.Add(totalPlanning);
                }
            }

            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);
            return this.Ok(localPath);
        }
    }
}
