using AirportVisitSystem.Data;
using AirportVisitSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize(Roles = "Manager")]
public class ApprovalController : Controller
{
    // home computer
    //private readonly AirportVisitDatabase1 _context;
    // office computer
    private readonly AirportVisitDb _context;
    
    //home computer
    //public ApprovalController(AirportVisitDatabase1 context) => _context = context;
    //office computer
    public ApprovalController(AirportVisitDb context) => _context = context;

    public async Task<IActionResult> Index(string searchTerm)
    {
        ViewData["Role"] = "Manager";
        ViewData["Title"] = "Approvals";
        ViewData["SearchTerm"] = searchTerm;

        var pendingVisitIds = await _context.Approvals
            .Where(a => a.ApprovalStatus == "Pending")
            .Join(_context.VisitVisitors, a => a.VisitVisitorID, vv => vv.VisitVisitorID, (a, vv) => vv.VisitID)
            .Distinct()
            .ToListAsync();

        var query = _context.Visits.Where(v => pendingVisitIds.Contains(v.VisitID));
        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(v => v.VisitTitle.Contains(searchTerm));

        var visits = await query.OrderBy(v => v.VisitDate).ToListAsync();

        var rows = new List<ApprovalQueueRow>();
        foreach (var visit in visits)
        {
            var visitVisitorIds = await _context.VisitVisitors
                .Where(vv => vv.VisitID == visit.VisitID).Select(vv => vv.VisitVisitorID).ToListAsync();
            var requestedById = await _context.Approvals
                .Where(a => visitVisitorIds.Contains(a.VisitVisitorID))
                .Select(a => a.RequestedEmployeeID).FirstAsync();
            var requestedBy = await _context.EmployeeHosts.FirstAsync(e => e.EmployeeID == requestedById);

            rows.Add(new ApprovalQueueRow
            {
                VisitID = visit.VisitID,
                VisitTitle = visit.VisitTitle,
                VisitDate = visit.VisitDate,
                RequestedByName = requestedBy.Name
            });
        }

        return View(rows);
    }

    [HttpGet]
    public async Task<IActionResult> Manage(int visitId)
    {
        ViewData["Role"] = "Manager";
        ViewData["Title"] = "Manage Approval";

        var visit = await _context.Visits.FirstOrDefaultAsync(v => v.VisitID == visitId);
        if (visit == null) return NotFound();

        var visitVisitors = await _context.VisitVisitors.Where(vv => vv.VisitID == visitId).ToListAsync();
        var visitVisitorIds = visitVisitors.Select(vv => vv.VisitVisitorID).ToList();
        var visitorIds = visitVisitors.Select(vv => vv.VisitorID).ToList();
        var visitors = await _context.Visitors.Where(v => visitorIds.Contains(v.VisitorID)).ToListAsync();

        var firstApproval = await _context.Approvals.FirstAsync(a => visitVisitorIds.Contains(a.VisitVisitorID));
        var requestedBy = await _context.EmployeeHosts.FirstAsync(e => e.EmployeeID == firstApproval.RequestedEmployeeID);

        var vm = new ApprovalManageViewModel
        {
            Visit = visit,
            RequestedBy = requestedBy,
            Department = await _context.Departments.FirstAsync(d => d.DepartmentID == visit.DepartmentID),
            VisitType = await _context.VisitTypes.FirstAsync(t => t.VisitTypeID == visit.VisitTypeID),
            Visitors = visitVisitors.Select(vv =>
            {
                var visitor = visitors.First(v => v.VisitorID == vv.VisitorID);
                return new ApprovalVisitorRow
                {
                    VisitVisitorID = vv.VisitVisitorID,
                    Name = visitor.Name,
                    Organization = visitor.Organization,
                    Phone = visitor.Phone,
                    Email = visitor.Email,
                    IsAllowed = vv.VisitorStatus == "Allowed"
                };
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int visitId, List<int> AllowedVisitVisitorIds)
    {
        AllowedVisitVisitorIds ??= new List<int>();
        var manager = await GetCurrentManager();

        var visitVisitors = await _context.VisitVisitors.Where(vv => vv.VisitID == visitId).ToListAsync();
        var visitVisitorIds = visitVisitors.Select(vv => vv.VisitVisitorID).ToList();
        var approvals = await _context.Approvals.Where(a => visitVisitorIds.Contains(a.VisitVisitorID)).ToListAsync();

        bool anyAllowed = false;
        foreach (var vv in visitVisitors)
        {
            bool allowed = AllowedVisitVisitorIds.Contains(vv.VisitVisitorID);
            vv.VisitorStatus = allowed ? "Allowed" : "Denied";
            if (allowed) anyAllowed = true;
        }

        foreach (var approval in approvals)
        {
            approval.ApprovalStatus = "Approved";
            approval.ApprovingManagerID = manager.ManagerID;
            approval.DecisionDate = DateTime.Now;
        }

        var visit = await _context.Visits.FirstAsync(v => v.VisitID == visitId);
        visit.VisitStatus = anyAllowed ? "Approved" : "Rejected";

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int visitId)
    {
        var manager = await GetCurrentManager();

        var visitVisitors = await _context.VisitVisitors.Where(vv => vv.VisitID == visitId).ToListAsync();
        var visitVisitorIds = visitVisitors.Select(vv => vv.VisitVisitorID).ToList();
        var approvals = await _context.Approvals.Where(a => visitVisitorIds.Contains(a.VisitVisitorID)).ToListAsync();

        foreach (var vv in visitVisitors) vv.VisitorStatus = "Denied";
        foreach (var approval in approvals)
        {
            approval.ApprovalStatus = "Rejected";
            approval.ApprovingManagerID = manager.ManagerID;
            approval.DecisionDate = DateTime.Now;
        }

        var visit = await _context.Visits.FirstAsync(v => v.VisitID == visitId);
        visit.VisitStatus = "Rejected";

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    private async Task<SiteVisitingManager> GetCurrentManager()
    {
        int loginId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return await _context.SiteVisitingManagers.FirstAsync(m => m.ManagerLoginID == loginId);
    }
}
