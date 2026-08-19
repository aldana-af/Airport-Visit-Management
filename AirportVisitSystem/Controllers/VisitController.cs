using AirportVisitSystem.Data;
using AirportVisitSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AirportVisitSystem.Services;

public class VisitController : Controller
{
    // home computer
    //private readonly AirportVisitDatabase1 _context;
    //private readonly IEmployeeFormApiClient _employeeFormApiClient;
    //public VisitController(AirportVisitDatabase1 context, IEmployeeFormApiClient employeeFormApiClient)
    //{
        //_context = context;
        //_employeeFormApiClient = employeeFormApiClient;
    //}

    // office computer
    private readonly AirportVisitDb _context;
    private readonly IEmployeeFormApiClient _employeeFormApiClient;
    public VisitController(AirportVisitDb context, IEmployeeFormApiClient employeeFormApiClient)
    {
        _context = context;
        _employeeFormApiClient = employeeFormApiClient;
    }


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

    //----------------------------------CREATE-------------------------------------------------------------------
    //get
    [Authorize(Roles = "Employee,Manager")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["Role"] = User.IsInRole("Employee") ? "Employee" : "Manager";
        ViewData["Title"] = "Request Visit";

        var vm = new CreateVisitViewModel
        {
            VisitTypes = await GetVisitTypeOptions(),
            AllVisitors = await _context.Visitors.OrderBy(v => v.Name).ToListAsync()
        };

        if (User.IsInRole("Manager"))
        {
            vm.Hosts = await GetHostOptions();
        }

        return View(vm);
    }

    //post
    [Authorize(Roles = "Employee,Manager")]
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

        if (User.IsInRole("Manager") && vm.HostEmployeeID == null)
            ModelState.AddModelError("HostEmployeeID", "Select which Employee Host this visit is for.");

        if (!ModelState.IsValid)
        {
            ViewData["Role"] = User.IsInRole("Employee") ? "Employee" : "Manager";
            ViewData["Title"] = "Request Visit";
            vm.VisitTypes = await GetVisitTypeOptions();
            vm.AllVisitors = await _context.Visitors.OrderBy(v => v.Name).ToListAsync();
            if (User.IsInRole("Manager")) vm.Hosts = await GetHostOptions();
            return View(vm);
        }

        EmployeeHost host;
        if (User.IsInRole("Employee"))
        {
            int employeeFormUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            host = _context.EmployeeHosts.First(e => e.EmployeeFormUserId == employeeFormUserId);
        }
        else
        {
            host = await _context.EmployeeHosts.FirstOrDefaultAsync(e => e.EmployeeID == vm.HostEmployeeID);
            if (host == null)
            {
                ModelState.AddModelError("HostEmployeeID", "Selected Employee Host could not be found.");
                ViewData["Role"] = "Manager";
                ViewData["Title"] = "Request Visit";
                vm.VisitTypes = await GetVisitTypeOptions();
                vm.AllVisitors = await _context.Visitors.OrderBy(v => v.Name).ToListAsync();
                vm.Hosts = await GetHostOptions();
                return View(vm);
            }
        }

        var visit = new Visit
        {
            VisitTitle = vm.VisitTitle,
            VisitDescription = vm.VisitDescription,
            DepartmentID = host.DepartmentID,
            HostEmployeeID = host.EmployeeID,
            VisitTypeID = vm.VisitTypeID,
            VisitStatus = "Pending",
            Status = "Active",
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
                RequestedEmployeeID = host.EmployeeID,
                ApprovingManagerID = null,
                RequestDate = DateTime.Now
            });
        }
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    //-----------------------------------------EDIT---------------------------------------------------------------------
    //get
    [Authorize(Roles = "Employee,Manager")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.VisitID == id);
        if (visit == null) return NotFound();

        if (User.IsInRole("Employee"))
        {
            int employeeFormUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var employee = _context.EmployeeHosts.First(e => e.EmployeeFormUserId == employeeFormUserId);
            if (visit.HostEmployeeID != employee.EmployeeID) return Forbid();
        }

        if (visit.VisitStatus != "Pending" || visit.Status != "Active")
        {
            TempData["Error"] = "Only pending, active visits can be edited.";
            return RedirectToAction("Details", new { id });
        }

        ViewData["Role"] = User.IsInRole("Employee") ? "Employee" : "Manager";
        ViewData["Title"] = "Edit Visit";

        var vm = new EditVisitViewModel
        {
            VisitID = visit.VisitID,
            VisitTitle = visit.VisitTitle,
            VisitDescription = visit.VisitDescription,
            VisitTypeID = visit.VisitTypeID,
            VisitDate = visit.VisitDate,
            StartTime = visit.StartTime,
            EndTime = visit.EndTime,
            VisitTypes = await GetVisitTypeOptions()
        };
        return View(vm);
    }

    //post
    [Authorize(Roles = "Employee,Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditVisitViewModel vm)
    {
        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.VisitID == vm.VisitID);
        if (visit == null) return NotFound();

        if (User.IsInRole("Employee"))
        {
            int employeeFormUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var employee = _context.EmployeeHosts.First(e => e.EmployeeFormUserId == employeeFormUserId);
            if (visit.HostEmployeeID != employee.EmployeeID) return Forbid();
        }

        if (visit.VisitStatus != "Pending" || visit.Status != "Active")
        {
            TempData["Error"] = "Only pending, active visits can be edited.";
            return RedirectToAction("Details", new { id = vm.VisitID });
        }

        if (vm.StartTime < new TimeSpan(9, 0, 0) || vm.EndTime > new TimeSpan(17, 0, 0))
            ModelState.AddModelError("StartTime", "Visits can only be scheduled between 9 AM and 5 PM.");

        if (vm.EndTime <= vm.StartTime)
            ModelState.AddModelError("EndTime", "Visit end time must be after start time.");

        if (!ModelState.IsValid)
        {
            ViewData["Role"] = "Employee";
            ViewData["Title"] = "Edit Visit";
            vm.VisitTypes = await GetVisitTypeOptions();
            return View(vm);
        }

        visit.VisitTitle = vm.VisitTitle;
        visit.VisitDescription = vm.VisitDescription;
        visit.VisitTypeID = vm.VisitTypeID;
        visit.VisitDate = vm.VisitDate;
        visit.StartTime = vm.StartTime;
        visit.EndTime = vm.EndTime;

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = vm.VisitID });
    }

    //----------------------------------CANCEL-----------------------------------------------------------------------
    // post
    [Authorize(Roles = "Employee,Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.VisitID == id);
        if (visit == null) return NotFound();

        if (User.IsInRole("Employee"))
        {
            int employeeFormUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var employee = _context.EmployeeHosts.First(e => e.EmployeeFormUserId == employeeFormUserId);
            if (visit.HostEmployeeID != employee.EmployeeID) return Forbid();
        }

        if (visit.Status != "Active" || (visit.VisitStatus != "Pending" && visit.VisitStatus != "Approved"))
        {
            TempData["Error"] = "This visit can't be cancelled.";
            return RedirectToAction("Details", new { id });
        }

        bool anyCheckedIn = await _context.VisitVisitors
            .AnyAsync(vv => vv.VisitID == id && vv.CheckIn != null);
        if (anyCheckedIn)
        {
            TempData["Error"] = "This visit has already started and can't be cancelled.";
            return RedirectToAction("Details", new { id });
        }

        visit.Status = "Cancelled";
        await _context.SaveChangesAsync();

        TempData["Message"] = "Visit cancelled.";
        return RedirectToAction("Details", new { id });
    }

    //--------------------------------------DETAILS----------------------------------------------------------------------
    [Authorize(Roles = "Employee,Manager")]
    public async Task<IActionResult> Details(int id)
    {
        ViewData["Role"] = User.IsInRole("Employee") ? "Employee" : "Manager";
        ViewData["Title"] = "Visit Details";

        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.VisitID == id);
        if (visit == null) return NotFound();

        // for editing visit info
        int employeeFormUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        bool canEdit = false;
        if (User.IsInRole("Employee"))
        {
            var currentEmployee = _context.EmployeeHosts.FirstOrDefault(e => e.EmployeeFormUserId == employeeFormUserId);
            canEdit = currentEmployee != null
                && visit.HostEmployeeID == currentEmployee.EmployeeID
                && visit.VisitStatus == "Pending"
                && visit.Status == "Active";
        }
        else if (User.IsInRole("Manager"))
        {
            canEdit = visit.VisitStatus == "Pending" && visit.Status == "Active";
        }

        var visitVisitors = await _context.VisitVisitors.Where(vv => vv.VisitID == id).ToListAsync();

        bool canCancel = false;
        if (User.IsInRole("Employee"))
        {
            var currentEmployee = _context.EmployeeHosts.FirstOrDefault(e => e.EmployeeFormUserId == employeeFormUserId);
            canCancel = currentEmployee != null
                && visit.HostEmployeeID == currentEmployee.EmployeeID
                && visit.Status == "Active"
                && (visit.VisitStatus == "Pending" || visit.VisitStatus == "Approved")
                && !visitVisitors.Any(vv => vv.CheckIn != null);
        }
        else if (User.IsInRole("Manager"))
        {
            canCancel = visit.Status == "Active"
                && (visit.VisitStatus == "Pending" || visit.VisitStatus == "Approved")
                && !visitVisitors.Any(vv => vv.CheckIn != null);
        }

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
            CanEdit = canEdit,
            CanCancel = canCancel,

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


    private async Task<List<SelectListItem>> GetHostOptions()
    {
        var hosts = await _context.EmployeeHosts.ToListAsync();
        var options = new List<SelectListItem>();
        foreach (var host in hosts)
        {
            var profile = await _employeeFormApiClient.LookupByEmployeeIdAsync(host.EmployeeID.ToString());
            options.Add(new SelectListItem
            {
                Value = host.EmployeeID.ToString(),
                Text = profile?.Name ?? $"Employee #{host.EmployeeID}"
            });
        }
        return options;
    }
}
