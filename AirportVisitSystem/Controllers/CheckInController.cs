using AirportVisitSystem.Data;
using AirportVisitSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize(Roles = "Employee")]
public class CheckInController : Controller
{
    // home computer
    //private readonly AirportVisitDatabase1 _context;
    //public CheckInController(AirportVisitDatabase1 context) => _context = context;

    // office computer
    private readonly AirportVisitDb _context;
    public CheckInController(AirportVisitDb context) => _context = context;

    private async Task<EmployeeHost> GetCurrentEmployee()
    {
        int loginId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return await _context.EmployeeHosts.FirstAsync(e => e.LoginID == loginId);
    }

    [HttpGet]
    public async Task<IActionResult> CheckIn(int visitId)
    {
        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.VisitID == visitId);
        if (visit == null) return NotFound();

        var employee = await GetCurrentEmployee();
        if (visit.HostEmployeeID != employee.EmployeeID) return Forbid();

        var pending = await _context.VisitVisitors
            .Where(vv => vv.VisitID == visitId && vv.VisitorStatus == "Allowed" && vv.CheckIn == null)
            .ToListAsync();

        if (!pending.Any()) return RedirectToAction("Employee", "Home");

        var visitorIds = pending.Select(vv => vv.VisitorID).ToList();
        var visitors = await _context.Visitors.Where(v => visitorIds.Contains(v.VisitorID)).ToListAsync();

        var availableBadges = await _context.Badges
            .Where(b => b.Status == "Inactive")
            .Take(pending.Count)
            .ToListAsync();

        if (availableBadges.Count < pending.Count)
        {
            TempData["Error"] = $"Not enough badges available. Need {pending.Count}, only {availableBadges.Count} free.";
            return RedirectToAction("Employee", "Home");
        }

        var vm = new CheckInViewModel
        {
            VisitID = visitId,
            VisitTitle = visit.VisitTitle,
            CurrentTime = DateTime.Now,
            Visitors = pending.Select((vv, i) => new CheckInVisitorRow
            {
                VisitVisitorID = vv.VisitVisitorID,
                Name = visitors.First(v => v.VisitorID == vv.VisitorID).Name,
                TentativeBadgeNumber = availableBadges[i].BadgeNumber
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmCheckIn(int visitId)
    {
        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.VisitID == visitId);
        if (visit == null) return NotFound();

        var employee = await GetCurrentEmployee();
        if (visit.HostEmployeeID != employee.EmployeeID) return Forbid();

        var pending = await _context.VisitVisitors
            .Where(vv => vv.VisitID == visitId && vv.VisitorStatus == "Allowed" && vv.CheckIn == null)
            .ToListAsync();

        if (!pending.Any()) return RedirectToAction("Employee", "Home");

        // Re-verify badge availability at the moment of confirmation, not just at page load
        var availableBadges = await _context.Badges
            .Where(b => b.Status == "Inactive")
            .Take(pending.Count)
            .ToListAsync();

        if (availableBadges.Count < pending.Count)
        {
            TempData["Error"] = "Badge availability changed since this page loaded. Please try again.";
            return RedirectToAction("Employee", "Home");
        }

        var now = DateTime.Now;
        for (int i = 0; i < pending.Count; i++)
        {
            pending[i].BadgeID = availableBadges[i].BadgeID;
            pending[i].CheckIn = now;
            availableBadges[i].Status = "Active";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Employee", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> CheckOut(int visitId)
    {
        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.VisitID == visitId);
        if (visit == null) return NotFound();

        var employee = await GetCurrentEmployee();
        if (visit.HostEmployeeID != employee.EmployeeID) return Forbid();

        var pending = await _context.VisitVisitors
            .Where(vv => vv.VisitID == visitId && vv.VisitorStatus == "Allowed" && vv.CheckIn != null && vv.CheckOut == null)
            .ToListAsync();

        if (!pending.Any()) return RedirectToAction("Employee", "Home");

        var visitorIds = pending.Select(vv => vv.VisitorID).ToList();
        var visitors = await _context.Visitors.Where(v => visitorIds.Contains(v.VisitorID)).ToListAsync();

        var vm = new CheckOutViewModel
        {
            VisitID = visitId,
            VisitTitle = visit.VisitTitle,
            CurrentTime = DateTime.Now,
            VisitorNames = pending.Select(vv => visitors.First(v => v.VisitorID == vv.VisitorID).Name).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmCheckOut(int visitId)
    {
        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.VisitID == visitId);
        if (visit == null) return NotFound();

        var employee = await GetCurrentEmployee();
        if (visit.HostEmployeeID != employee.EmployeeID) return Forbid();

        var pending = await _context.VisitVisitors
            .Where(vv => vv.VisitID == visitId && vv.VisitorStatus == "Allowed" && vv.CheckIn != null && vv.CheckOut == null)
            .ToListAsync();

        if (!pending.Any()) return RedirectToAction("Employee", "Home");

        var now = DateTime.Now;
        foreach (var vv in pending)
        {
            vv.CheckOut = now;
            if (vv.BadgeID.HasValue)
            {
                var badge = await _context.Badges.FirstOrDefaultAsync(b => b.BadgeID == vv.BadgeID);
                if (badge != null) badge.Status = "Inactive";
            }
        }

        visit.VisitStatus = "Complete";
        await _context.SaveChangesAsync();
        return RedirectToAction("Employee", "Home");
    }
}