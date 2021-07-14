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
            this.logger.LogDebug($"POST Register({employeeName}, {department})");
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
        public async Task<IActionResult> Latest()
        {
            return this.Json(await this.dbContext.Employees
                .OrderByDescending(e => e.Id)
                .Take(10)
                .ToArrayAsync(this.HttpContext.RequestAborted));
        }

        [HttpPost]
        public async Task<IActionResult> Note([FromForm] string newNote)
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
            return this.Json(dbOwner);
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
                .Include(e => e.TotalPlannings)
                .ThenInclude(tp => tp.Project)
                .Include(e => e.WeeklyCapacities)
                .Include(e => e.Vacations)
                .SingleAsync(this.HttpContext.RequestAborted);

            return this.Json(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Vacation(long calendarWeek, long days)
        {
            var owner = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (owner == null)
            {
                return this.Forbid();
            }

            this.dbContext.Vacations.Add(new Vacation()
            {
                AbsentDays = days,
                CalendarWeek = calendarWeek,
                EmployeeId = long.Parse(owner),
            });

            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);

            return this.NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Vacation(long id)
        {
            this.logger.LogInformation($"DELETE Vacation({id})");
            var owner = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (owner == null)
            {
                return this.Forbid();
            }

            var vac = await this.dbContext.Vacations
                .Where(v => v.Id == id)
                .Where(v => v.EmployeeId == long.Parse(owner))
                .SingleAsync(this.HttpContext.RequestAborted);
            this.dbContext.Vacations.Remove(vac);
            await this.dbContext.SaveChangesAsync(this.HttpContext.RequestAborted);

            return this.NoContent();
        }
    }
}
