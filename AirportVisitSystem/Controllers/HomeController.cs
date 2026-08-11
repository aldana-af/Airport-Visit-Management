using AirportVisitSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class HomeController : Controller
{
    // home computer
    //private readonly AirportVisitDatabase1 _context;
    // office computer
    private readonly AirportVisitDb _context;

    // home computer
    //public HomeController(AirportVisitDatabase1 context) => _context = context;
    // office computer
    public HomeController(AirportVisitDb context) => _context = context;

    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Employee()
    {
        int loginId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var employee = _context.EmployeeHosts.First(e => e.LoginID == loginId);

        ViewData["Greeting"] = $"Welcome, {employee.Name}";
        ViewData["Role"] = "Employee";

        var upcomingVisits = await _context.Visits
            .Where(v => v.HostEmployeeID == employee.EmployeeID && v.VisitDate >= DateTime.Today)
            .OrderBy(v => v.VisitDate)
            .ToListAsync();

        return View(upcomingVisits);
    }

    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Manager()
    {
        int loginId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var manager = _context.SiteVisitingManagers.First(m => m.ManagerLoginID == loginId);

        ViewData["Greeting"] = $"Welcome, {manager.Name}";
        ViewData["Role"] = "Manager";

        var upcomingVisits = await _context.Visits
            .Where(v => v.VisitDate >= DateTime.Today)
            .OrderBy(v => v.VisitDate)
            .ToListAsync();

        return View(upcomingVisits);
    }
}
