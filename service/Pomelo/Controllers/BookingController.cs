namespace Pomelo.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
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
        public async Task<ActionResult<string>> Upload(long projectId, IFormFile file)
        {
            var user = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var dep = this.HttpContext.User.FindFirst(ClaimTypes.GroupSid)?.Value;
            if (user == null || dep == null)
            {
                return this.Unauthorized();
            }

            var project = await this.dbContext.Projects
                .Where(p => p.Id == projectId)
                .SingleAsync(this.HttpContext.RequestAborted);

            if (project.Department != dep)
            {
                return this.Forbid();
            }

            var guid = new Guid();
            var localPath = $"/Uploads/{projectId}_{guid}.csv";
            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();
            var lines = content.Split('\n');
            var header = lines[0];

            foreach (var line in lines.Skip(1))
            {
                var split = line.Split(',');
                var employeeId = long.Parse(split[0]);
                var hours = long.Parse(split[1]);
                var totalPlanning = await this.dbContext.EmployeeProjectHours
                    .Where(tp => tp.ProjectId == projectId)
                    .Where(tp => tp.EmployeeId == employeeId)
                    .SingleOrDefaultAsync(this.HttpContext.RequestAborted);
                if (totalPlanning != null)
                {
                    totalPlanning.DeliveredHours = hours;
                }
                else
                {
                    totalPlanning = new EmployeeProjectHours()
                    {
                        ProjectId = projectId,
                        EmployeeId = employeeId,
                        DeliveredHours = hours,
                        TotalHours = 0,
                    };
                    this.dbContext.EmployeeProjectHours.Add(totalPlanning);
                }
            }

            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);
            await System.IO.File.WriteAllBytesAsync($"wwwroot/{localPath}", Encoding.ASCII.GetBytes(content), this.HttpContext.RequestAborted);
            return this.Ok(localPath);
        }
    }
}
