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
    using Pomelo.DataTransferObjects;
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
        public async Task<ActionResult<List<ProjectInfoDto>>> Projects()
        {
            var userId = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return this.Unauthorized();
            }

            var dbUser = await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(userId))
                .FirstAsync(this.HttpContext.RequestAborted);

            var projects = await this.dbContext.Projects
                .Where(p => p.Department == dbUser.Department)
                .ToArrayAsync(this.HttpContext.RequestAborted);

            return projects.Select(p => new ProjectInfoDto(p)).ToList();
        }

        [HttpGet]
        public async Task<ActionResult<ProjectDto>> Project(long projectId)
        {
            // TODO use department from session?
            var userId = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return this.Unauthorized();
            }

            var dbUser = await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(userId))
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

            return await this.GetProject(projectId);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Project([FromForm]string name, [FromForm] DateTime begin, [FromForm] DateTime end)
        {
            var userId = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return this.Forbid();
            }

            var dbUser = await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(userId))
                .FirstAsync(this.HttpContext.RequestAborted);
            var newProject = new Project()
            {
                OwnerId = dbUser.Id,
                Department = dbUser.Department,
                Name = name,
                Begin = begin,
                End = end,
            };
            this.dbContext.Projects.Add(newProject);
            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);
            return await this.GetProject(newProject.Id);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Hours([FromForm] long employeeId, [FromForm] long projectId, [FromForm] long hours)
        {
            var project = await this.dbContext.Projects
                .Where(p => p.Id == projectId)
                .AsNoTracking()
                .SingleAsync();
            EmployeeProjectHours? totalPlanning = await this.dbContext.EmployeeProjectHours
                .Where(tp => tp.EmployeeId == employeeId)
                .Where(tp => tp.ProjectId == projectId)
                .SingleOrDefaultAsync(this.HttpContext.RequestAborted);
            if (totalPlanning != null)
            {
                totalPlanning.TotalHours = hours;
            }
            else
            {
                totalPlanning = new EmployeeProjectHours()
                {
                    EmployeeId = employeeId,
                    ProjectId = projectId,
                    TotalHours = hours,
                    DeliveredHours = 0,
                };
                this.dbContext.EmployeeProjectHours.Add(totalPlanning);
            }

            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);
            return this.Json(project);
        }

        [HttpPost]
        public async Task<ProjectDto> Capacity([FromForm] long employeeId, [FromForm] long projectId, [FromForm] DateTime start, [FromForm] long capacity)
        {
            EmployeeProjectWeeklyCapacity? weeklyProjectCapacity = await this.dbContext.EmployeeProjectWeeklyCapacities
                .Where(wpc => wpc.ProjectId == projectId)
                .Where(wpc => wpc.EmployeeId == employeeId)
                .Where(wpc => wpc.Start == start)
                .SingleOrDefaultAsync(this.HttpContext.RequestAborted);

            Console.WriteLine($"{employeeId} {projectId} {start} {capacity}");
            if (weeklyProjectCapacity != null)
            {
                weeklyProjectCapacity.Capacity = capacity;
            }
            else
            {
                weeklyProjectCapacity = new EmployeeProjectWeeklyCapacity()
                {
                    EmployeeId = employeeId,
                    ProjectId = projectId,
                    Start = start,
                    Capacity = capacity,
                };
                this.dbContext.EmployeeProjectWeeklyCapacities.Add(weeklyProjectCapacity);
            }

            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);
            return await this.GetProject(projectId);
        }

        private async Task<ProjectDto> GetProject(long projectId)
        {
            var project = await this.dbContext.Projects
                .Where(p => p.Id == projectId)
                .SingleAsync();

            var projectHours = await this.dbContext.EmployeeProjectHours
                .Where(eph => eph.ProjectId == projectId)
                .ToListAsync();

            var projectCapacities = await this.dbContext.EmployeeProjectWeeklyCapacities
                .Where(epwc => epwc.ProjectId == projectId)
                .ToListAsync();

            project.EmployeeProjectHours = projectHours;
            project.EmployeeProjectWeeklyCapacities = projectCapacities;

            return new ProjectDto(project);
        }
    }
}
