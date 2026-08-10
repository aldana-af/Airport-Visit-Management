using AirportVisitSystem.Data;
using AirportVisitSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AirportVisitSystem.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class VisitController : Controller
    {
        private readonly AirportVisitDb _context;

        public VisitController(AirportVisitDb context)
        {
            _context = context;
        }

        // Visits submitted by the logged-in employee
        public async Task<IActionResult> Index()
        {
            int employeeId = GetCurrentEmployeeId();
            var visits = await _context.Visit
                .Where(v => v.HostEmployeeID == employeeId)
                .OrderByDescending(v => v.CreatedDate)
                .ToListAsync();
            return View(visits);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new CreateVisitViewModel
            {
                Departments = await GetDepartmentOptions(),
                Hosts = await GetHostOptions(),
                VisitTypes = await GetVisitTypeOptions()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVisitViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Departments = await GetDepartmentOptions();
                vm.Hosts = await GetHostOptions();
                vm.VisitTypes = await GetVisitTypeOptions();
                return View(vm);
            }

            var visit = new Visit
            {
                VisitTitle = vm.VisitTitle,
                VisitDescription = vm.VisitDescription,
                DepartmentID = vm.DepartmentID,
                HostEmployeeID = vm.HostEmployeeID,
                VisitTypeID = vm.VisitTypeID,
                VisitStatus = "Pending",
                CreatedDate = DateTime.Now,
                VisitDate = vm.VisitDate,
                StartTime = vm.StartTime,
                EndTime = vm.EndTime
            };

            _context.Visit.Add(visit);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = visit.VisitID });
        }

        public async Task<IActionResult> Details(int id)
        {
            var visit = await _context.Visit.FirstOrDefaultAsync(v => v.VisitID == id);
            if (visit == null) return NotFound();
            return View(visit);
        }

        private int GetCurrentEmployeeId()
        {
            int loginId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return _context.EmployeeHosts.First(e => e.LoginID == loginId).EmployeeID;
        }

        private async Task<List<SelectListItem>> GetDepartmentOptions() =>
            await _context.Department
                .Select(d => new SelectListItem { Value = d.DepartmentID.ToString(), Text = d.DepartmentName })
                .ToListAsync();

        private async Task<List<SelectListItem>> GetHostOptions() =>
            await _context.EmployeeHosts
                .Select(e => new SelectListItem { Value = e.EmployeeID.ToString(), Text = e.Name })
                .ToListAsync();

        private async Task<List<SelectListItem>> GetVisitTypeOptions() =>
            await _context.VisitType
                .Select(t => new SelectListItem { Value = t.VisitTypeID.ToString(), Text = t.Type })
                .ToListAsync();
    }
}
