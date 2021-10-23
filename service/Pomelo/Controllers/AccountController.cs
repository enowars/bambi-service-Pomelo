namespace Pomelo.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Pomelo.Database;
    using Pomelo.DataTransferObjects;
    using Pomelo.Models;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly PomeloDbContext dbContext;

        public AccountController(ILogger<AccountController> logger, PomeloDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<EmployeeDto>> Account()
        {
            var userId = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return this.Unauthorized();
            }

            var employee = await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(userId))
                .SingleAsync(this.HttpContext.RequestAborted);

            return new EmployeeDto(employee);
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeDto>>> Department()
        {
            this.logger.LogInformation($"GET Department");
            var user = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user == null)
            {
                return this.Unauthorized();
            }

            var employee = await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(user))
                .SingleAsync(this.HttpContext.RequestAborted);

            var department = await this.dbContext.Employees
                .Where(e => e.Department == employee.Department)
                .ToArrayAsync(this.HttpContext.RequestAborted);

            return department.Select(e => new EmployeeDto(e)).ToList();
        }

        [HttpGet]
        public async Task<ActionResult<EmployeeDto>> Employee(long employeeId)
        {
            var user = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user == null)
            {
                return this.Unauthorized();
            }

            var dbUser = await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(user))
                .AsNoTracking()
                .SingleAsync(this.HttpContext.RequestAborted);

            var employee = await this.dbContext.Employees
                .Where(e => e.Id == employeeId)
                .AsNoTracking()
                .SingleAsync(this.HttpContext.RequestAborted);

            if (dbUser.Department != employee.Department)
            {
                return this.Forbid();
            }

            employee.EmployeeProjectHours = await this.dbContext.EmployeeProjectHours
                .Where(tp => tp.EmployeeId == dbUser.Id)
                .AsNoTracking()
                .ToListAsync(this.HttpContext.RequestAborted);

            employee.EmployeeProjectWeeklyCapacities = await this.dbContext.EmployeeProjectWeeklyCapacities
                .Where(wc => wc.EmployeeId == dbUser.Id)
                .AsNoTracking()
                .ToListAsync(this.HttpContext.RequestAborted);

            return new EmployeeDto(employee);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Register([FromForm] string employeeName, [FromForm] string department, [FromForm] string? note)
        {
            this.logger.LogDebug($"POST /account/register({employeeName}, {department})");
            var newEmployee = new Employee()
            {
                Name = employeeName,
                Department = department,
                Note = note,
            };
            this.dbContext.Employees.Add(newEmployee);
            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);

            // Save the name, id and department to the session
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employeeName),
                new Claim(ClaimTypes.NameIdentifier, newEmployee.Id.ToString()),
                new Claim(ClaimTypes.GroupSid, department),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await this.HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
            return new EmployeeDto(newEmployee);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Note([FromForm] string note)
        {
            var userId = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return this.Forbid();
            }

            (await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(userId))
                .SingleAsync(this.HttpContext.RequestAborted))
                .Note = note;

            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);

            return new EmployeeDto(await this.dbContext.Employees
                .Where(e => e.Name == this.HttpContext.User.FindFirst(ClaimTypes.Name)!.Value)
                .FirstAsync(this.HttpContext.RequestAborted));
        }

        [HttpPost]
        public async Task<NoContentResult> Logout()
        {
            await this.HttpContext.SignOutAsync();
            return this.NoContent();
        }
    }
}
