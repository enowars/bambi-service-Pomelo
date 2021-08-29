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

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] string employeeName, [FromForm] string department)
        {
            this.logger.LogDebug($"POST /account/register({employeeName}, {department})");
            var newEmployee = new Employee()
            {
                Name = employeeName,
                Department = department,
            };
            this.dbContext.Employees.Add(newEmployee);
            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employeeName),
                new Claim(ClaimTypes.NameIdentifier, newEmployee.Id.ToString()),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await this.HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
            return this.Json(newEmployee);
        }

        [HttpGet]
        public async Task<IActionResult> Info()
        {
            var owner = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (owner == null)
            {
                return this.Forbid();
            }

            var dbOwner = await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(owner))
                .FirstAsync(this.HttpContext.RequestAborted);
            return this.Json(dbOwner);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync();
            return this.NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<Employee[]>> Latest()
        {
            return await this.dbContext.Employees
                .OrderByDescending(e => e.Id)
                .Take(10)
                .ToArrayAsync(this.HttpContext.RequestAborted);
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> Note([FromForm] string newNote)
        {
            var owner = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (owner == null)
            {
                return this.Forbid();
            }

            var dbOwner = await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(owner))
                .FirstAsync(this.HttpContext.RequestAborted);
            dbOwner.Note = newNote;
            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);
            return dbOwner;
        }

        [HttpGet]
        public async Task<IActionResult> Data()
        {
            var owner = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (owner == null)
            {
                return this.Forbid();
            }

            var employee = await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(owner))
                .AsNoTracking()
                .SingleAsync(this.HttpContext.RequestAborted);

            var plannings = await this.dbContext.EmployeeProjectHours
                .Where(tp => tp.EmployeeId == employee.Id)
                .AsNoTracking()
                .ToListAsync(this.HttpContext.RequestAborted);

            var capacities = await this.dbContext.EmployeeProjectWeeklyCapacities
                .Where(wc => wc.EmployeeId == employee.Id)
                .AsNoTracking()
                .ToListAsync(this.HttpContext.RequestAborted);

            employee.EmployeeProjectHours = plannings;
            employee.EmployeeProjectWeeklyCapacities = capacities;

            return this.Json(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Department()
        {
            this.logger.LogInformation($"GET Department");
            var user = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user == null)
            {
                return this.Forbid();
            }

            var employee = await this.dbContext.Employees
                .Where(e => e.Id == long.Parse(user))
                .SingleAsync(this.HttpContext.RequestAborted);

            var department = await this.dbContext.Employees
                .Where(e => e.Department == employee.Department)
                .ToArrayAsync(this.HttpContext.RequestAborted);

            return this.Json(department);
        }

        [HttpGet]
        public async Task<IActionResult> UserData(long employeeId)
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

            return this.Json(employee);
        }
    }
}
