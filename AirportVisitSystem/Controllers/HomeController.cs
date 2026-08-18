using AirportVisitSystem.Data;
using AirportVisitSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class HomeController : Controller
{
    // home computer
    //private readonly AirportVisitDatabase1 _context;
    //public HomeController(AirportVisitDatabase1 context) => _context = context;

    // office computer
    private readonly AirportVisitDb _context;
    public HomeController(AirportVisitDb context) => _context = context;

    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Employee()
    {
        int employeeFormUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var employee = _context.EmployeeHosts.First(e => e.EmployeeFormUserId == employeeFormUserId);

        ViewData["Greeting"] = $"Welcome, {User.FindFirstValue(ClaimTypes.GivenName)}";
        ViewData["Role"] = "Employee";

        var todaysVisits = await _context.Visits
            .Where(v => v.HostEmployeeID == employee.EmployeeID
                     && v.VisitDate == DateTime.Today
                     && v.VisitStatus == "Approved")
            .ToListAsync();

        var upcomingVisits = await _context.Visits
            .Where(v => v.HostEmployeeID == employee.EmployeeID && v.VisitDate > DateTime.Today)
            .OrderBy(v => v.VisitDate)
            .ToListAsync();

        var vm = new EmployeeHomeViewModel
        {
            TodaysVisits = new List<TodayVisitRow>(),
            UpcomingVisits = upcomingVisits
        };

        foreach (var visit in todaysVisits)
        {
            var visitVisitors = await _context.VisitVisitors
                .Where(vv => vv.VisitID == visit.VisitID && vv.VisitorStatus == "Allowed")
                .ToListAsync();

            bool hasUncheckedIn = visitVisitors.Any(vv => vv.CheckIn == null);
            bool hasUncheckedOut = visitVisitors.Any(vv => vv.CheckIn != null && vv.CheckOut == null);

            vm.TodaysVisits.Add(new TodayVisitRow
            {
                Visit = visit,
                ActionType = hasUncheckedIn ? "CheckIn" : (hasUncheckedOut ? "CheckOut" : "None")
            });
        }

        return View(vm);
    }

    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Manager()
    {
        int employeeFormUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var manager = _context.SiteVisitingManagers.First(m => m.EmployeeFormUserId == employeeFormUserId);

        ViewData["Greeting"] = $"Welcome, {User.FindFirstValue(ClaimTypes.GivenName)}";
        ViewData["Role"] = "Manager";

        var upcomingVisits = await _context.Visits
            .Where(v => v.VisitDate >= DateTime.Today)
            .OrderBy(v => v.VisitDate)
            .ToListAsync();

        return View(upcomingVisits);
    }
}