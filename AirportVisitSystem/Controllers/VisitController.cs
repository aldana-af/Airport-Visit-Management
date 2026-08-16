using AirportVisitSystem.Data;
using AirportVisitSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class VisitController : Controller
{
    // home computer
    //private readonly AirportVisitDatabase1 _context;
    //public VisitController(AirportVisitDatabase1 context) => _context = context;


    // office computer
    private readonly AirportVisitDb _context;
    public VisitController(AirportVisitDb context) => _context = context;


    [Authorize(Roles = "Employee,Manager")]
    public async Task<IActionResult> Index(string searchTerm)
    {
        ViewData["Role"] = User.IsInRole("Employee") ? "Employee" : "Manager";
        ViewData["Title"] = "Visits";
        ViewData["SearchTerm"] = searchTerm;

        var query = _context.Visits.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(v => v.VisitTitle.Contains(searchTerm));

        var visits = await query.OrderByDescending(v => v.VisitDate).ToListAsync();
        return View(visits);
    }

    [Authorize(Roles = "Employee")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["Role"] = "Employee";
        ViewData["Title"] = "Request Visit";

        var vm = new CreateVisitViewModel
        {
            VisitTypes = await GetVisitTypeOptions(),
            AllVisitors = await _context.Visitors.OrderBy(v => v.Name).ToListAsync()
        };
        return View(vm);
    }

    [Authorize(Roles = "Employee")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVisitViewModel vm)
    {
        if (vm.SelectedVisitorIds == null || !vm.SelectedVisitorIds.Any())
            ModelState.AddModelError("SelectedVisitorIds", "Select at least one visitor.");

        if (vm.StartTime < new TimeSpan(9, 0, 0) || vm.EndTime > new TimeSpan(17, 0, 0))
            ModelState.AddModelError("StartTime", "Visits can only be scheduled between 9 AM and 5 PM.");

        if (vm.EndTime <= vm.StartTime)
            ModelState.AddModelError("EndTime", "Visit end time must be after start time.");

        if (!ModelState.IsValid)
        {
            ViewData["Role"] = "Employee";
            ViewData["Title"] = "Request Visit";
            vm.VisitTypes = await GetVisitTypeOptions();
            vm.AllVisitors = await _context.Visitors.OrderBy(v => v.Name).ToListAsync();
            return View(vm);
        }

        int employeeFormUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var employee = _context.EmployeeHosts.First(e => e.EmployeeFormUserId == employeeFormUserId);

        var visit = new Visit
        {
            VisitTitle = vm.VisitTitle,
            VisitDescription = vm.VisitDescription,
            DepartmentID = employee.DepartmentID,
            HostEmployeeID = employee.EmployeeID,
            VisitTypeID = vm.VisitTypeID,
            VisitStatus = "Pending",
            CreatedDate = DateTime.Now,
            VisitDate = vm.VisitDate,
            StartTime = vm.StartTime,
            EndTime = vm.EndTime
        };

        _context.Visits.Add(visit);
        await _context.SaveChangesAsync(); // needed so visit.VisitID is generated before use below

        foreach (var visitorId in vm.SelectedVisitorIds)
        {
            var visitVisitor = new VisitVisitor
            {
                VisitorID = visitorId,
                VisitID = visit.VisitID,
                VisitorStatus = "Pending"
            };
            _context.VisitVisitors.Add(visitVisitor);
            await _context.SaveChangesAsync(); // need VisitVisitorID generated before the Approval row below

            _context.Approvals.Add(new Approval
            {
                VisitVisitorID = visitVisitor.VisitVisitorID,
                ApprovalStatus = "Pending",
                RequestedEmployeeID = employee.EmployeeID,
                ApprovingManagerID = null,
                RequestDate = DateTime.Now
            });
        }
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Employee,Manager")]
    public async Task<IActionResult> Details(int id)
    {
        ViewData["Role"] = User.IsInRole("Employee") ? "Employee" : "Manager";
        ViewData["Title"] = "Visit Details";

        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.VisitID == id);
        if (visit == null) return NotFound();

        var visitVisitors = await _context.VisitVisitors.Where(vv => vv.VisitID == id).ToListAsync();
        var visitorIds = visitVisitors.Select(vv => vv.VisitorID).ToList();
        var visitors = await _context.Visitors.Where(v => visitorIds.Contains(v.VisitorID)).ToListAsync();

        var visitVisitorIds = visitVisitors.Select(vv => vv.VisitVisitorID).ToList();
        var approvalWithManager = await _context.Approvals
            .Where(a => visitVisitorIds.Contains(a.VisitVisitorID) && a.ApprovingManagerID != null)
            .FirstOrDefaultAsync();

        var badgeIds = visitVisitors.Where(vv => vv.BadgeID.HasValue).Select(vv => vv.BadgeID.Value).ToList();
        var badges = await _context.Badges.Where(b => badgeIds.Contains(b.BadgeID)).ToListAsync();

        string managerSignature = "Pending";
        if (approvalWithManager != null)
        {
            var manager = await _context.SiteVisitingManagers
                .FirstOrDefaultAsync(m => m.ManagerID == approvalWithManager.ApprovingManagerID);
            if (manager != null) managerSignature = manager.Name;
        }

        var vm = new VisitDetailsViewModel
        {
            Visit = visit,
            Department = await _context.Departments.FirstAsync(d => d.DepartmentID == visit.DepartmentID),
            Host = await _context.EmployeeHosts.FirstAsync(e => e.EmployeeID == visit.HostEmployeeID),
            VisitType = await _context.VisitTypes.FirstAsync(t => t.VisitTypeID == visit.VisitTypeID),
            ManagerSignature = managerSignature, // new

            Visitors = visitVisitors.Select(vv => new VisitDetailsVisitorRow
            {
                VisitVisitorID = vv.VisitVisitorID,
                Name = visitors.First(v => v.VisitorID == vv.VisitorID).Name,
                VisitorStatus = vv.VisitorStatus,
                BadgeNumber = vv.BadgeID.HasValue ? badges.FirstOrDefault(b => b.BadgeID == vv.BadgeID)?.BadgeNumber : null,
                CheckIn = vv.CheckIn,
                CheckOut = vv.CheckOut
            }).ToList()
        };

        return View(vm);
    }

    private async Task<List<SelectListItem>> GetVisitTypeOptions() =>
        await _context.VisitTypes
            .Select(t => new SelectListItem { Value = t.VisitTypeID.ToString(), Text = t.Type })
            .ToListAsync();
}
