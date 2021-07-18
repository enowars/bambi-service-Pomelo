namespace Pomelo.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Pomelo.Database;
    using Pomelo.Models;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProjectController : Controller
    {
        private readonly PomeloDbContext dbContext;

        public ProjectController(PomeloDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> DepartmentProjects()
        {
            var owner = this.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (owner == null)
            {
                return this.Forbid();
            }

            var dbOwner = await this.dbContext.Employees
                .Where(e => e.Name == owner)
                .FirstAsync(this.HttpContext.RequestAborted);

            var projects = await this.dbContext.Projects
                .Where(p => p.Department == dbOwner.Department)
                .ToArrayAsync(this.HttpContext.RequestAborted);

            return this.Json(projects);
        }

        [HttpGet]
        public async Task<IActionResult> Details(long projectId)
        {
            var user = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user == null)
            {
                return this.Forbid();
            }

            var dbUser = await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(user))
                .AsNoTracking()
                .SingleAsync(this.HttpContext.RequestAborted);

            var project = await this.dbContext.Projects
                .Where(p => p.Id == projectId)
                .AsNoTracking()
                .SingleAsync(this.HttpContext.RequestAborted);

            if (project.Department != dbUser.Department)
            {
                return this.Forbid();
            }

            var plannings = await this.dbContext.PlannedHours
                .Where(tp => tp.ProjectId == projectId)
                .AsNoTracking()
                .ToListAsync(this.HttpContext.RequestAborted);

            var capacities = await this.dbContext.WeeklyProjectCapacities
                .Where(wc => wc.ProjectId == projectId)
                .AsNoTracking()
                .ToListAsync(this.HttpContext.RequestAborted);

            project.PlannedHours = plannings;
            project.WeeklyProjectCapacities = capacities;

            return this.Json(project);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]string name, [FromForm] DateTime begin, [FromForm] DateTime end)
        {
            var owner = this.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (owner == null)
            {
                return this.Forbid();
            }

            var dbOwner = await this.dbContext.Employees
                .Where(e => e.Name == owner)
                .FirstAsync(this.HttpContext.RequestAborted);
            var newProject = new Project()
            {
                Owner = dbOwner,
                Department = dbOwner.Department,
                Name = name,
                Begin = begin,
                End = end,
            };
            this.dbContext.Projects.Add(newProject);
            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);
            return this.Json(newProject);
        }

        [HttpPost]
        public async Task<IActionResult> TotalPlanning([FromForm] long employeeId, [FromForm] long projectId, [FromForm] long hours)
        {
            PlannedHours? totalPlanning = await this.dbContext.PlannedHours
                .Where(tp => tp.EmployeeId == employeeId)
                .Where(tp => tp.ProjectId == projectId)
                .SingleOrDefaultAsync(this.HttpContext.RequestAborted);
            if (totalPlanning != null)
            {
                totalPlanning.TotalHours = hours;
            }
            else
            {
                totalPlanning = new PlannedHours()
                {
                    EmployeeId = employeeId,
                    ProjectId = projectId,
                    TotalHours = hours,
                    PerformedHours = 0,
                };
                this.dbContext.PlannedHours.Add(totalPlanning);
            }

            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);
            return this.Json(totalPlanning);
        }

        [HttpPost]
        public async Task WeeklyProjectCapacity([FromForm] long employeeId, [FromForm] long projectId, [FromForm] DateTime start, [FromForm] long capacity)
        {
            WeeklyProjectCapacity? weeklyProjectCapacity = await this.dbContext.WeeklyProjectCapacities
                .Where(wpc => wpc.ProjectId == projectId)
                .Where(wpc => wpc.EmployeeId == employeeId)
                .Where(wpc => wpc.Start == start)
                .SingleOrDefaultAsync(this.HttpContext.RequestAborted);

            Console.WriteLine($"{employeeId} {projectId} {start} {capacity}");
            if (weeklyProjectCapacity != null)
            {
                weeklyProjectCapacity.Hours = capacity;
            }
            else
            {
                weeklyProjectCapacity = new WeeklyProjectCapacity()
                {
                    EmployeeId = employeeId,
                    ProjectId = projectId,
                    Start = start,
                    Hours = capacity,
                };
                this.dbContext.WeeklyProjectCapacities.Add(weeklyProjectCapacity);
            }

            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);
        }
    }
}
