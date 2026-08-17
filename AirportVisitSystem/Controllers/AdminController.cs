using AirportVisitSystem.Data;
using AirportVisitSystem.Models;
using AirportVisitSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AirportVisitSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEmployeeFormApiClient _employeeFormApiClient;

        // home computer
        //private readonly AirportVisitDatabase1 _context;
        //public AdminController(AirportVisitDatabase1 context, IEmployeeFormApiClient employeeFormApiClient)
        //{
        //    _context = context;
        //    _employeeFormApiClient = employeeFormApiClient;
        //}

        // office computer
        private readonly AirportVisitDb _context;
        public AdminController(AirportVisitDb context, IEmployeeFormApiClient employeeFormApiClient)
        {
            _context = context;
            _employeeFormApiClient = employeeFormApiClient;
        }

        // GET: /Admin/RegisterEmployeeHost
        public IActionResult RegisterEmployeeHost()
        {
            ViewBag.Departments = _context.Departments.ToList();
            return View(new RegisterEmployeeHostViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEmployeeHost(RegisterEmployeeHostViewModel model)
        {
            ViewBag.Departments = _context.Departments.ToList();

            if (string.IsNullOrWhiteSpace(model.Username) && string.IsNullOrWhiteSpace(model.EmployeeId))
            {
                ModelState.AddModelError("", "Enter either a username or an employee ID to look up the employee.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Person must already exist in EmployeeForm — Airport never
            // creates EmployeeForm records itself (see the integration plan:
            // EmployeeForm is the one-way source of truth for identity).
            var profile = !string.IsNullOrWhiteSpace(model.Username)
                ? await _employeeFormApiClient.LookupByUsernameAsync(model.Username)
                : await _employeeFormApiClient.LookupByEmployeeIdAsync(model.EmployeeId);

            if (profile == null)
            {
                ModelState.AddModelError("", "No matching employee found in EmployeeForm. They need to be registered there first.");
                return View(model);
            }

            bool alreadyRegistered = _context.EmployeeHosts.Any(e => e.EmployeeFormUserId == profile.Id);
            if (alreadyRegistered)
            {
                ModelState.AddModelError("", $"{profile.Name} is already registered as an EmployeeHost.");
                return View(model);
            }

            if (!int.TryParse(profile.EmployeeId, out int employeeId))
            {
                ModelState.AddModelError("", $"EmployeeForm's employee ID for {profile.Name} ('{profile.EmployeeId}') isn't a valid number and can't be used as the AVS record ID.");
                return View(model);
            }

            if (_context.EmployeeHosts.Any(e => e.EmployeeID == employeeId))
            {
                ModelState.AddModelError("", $"An EmployeeHost record already exists with ID {employeeId}, but for a different person. This ID can't be reused.");
                return View(model);
            }

            var employeeHost = new EmployeeHost
            {
                DepartmentID = model.DepartmentID,
                EmployeeFormUserId = profile.Id,
                // Uses EmployeeForm's EmployeeId directly as Airport's own
                // EmployeeID (a deliberate choice to keep one shared
                // identifier across both systems, rather than a separate
                // Airport-only surrogate key).
                EmployeeID = employeeId
                // Name/Email/Role/LoginID intentionally left unset — this
                // data is live-fetched from EmployeeForm going forward,
                // not cached locally (see step 3's schema notes).
            };

            _context.EmployeeHosts.Add(employeeHost);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{profile.Name} was registered as an EmployeeHost.";
            return RedirectToAction("RegisterEmployeeHost");
        }

        // GET: /Admin/RegisterManager
        public IActionResult RegisterManager()
        {
            return View(new RegisterManagerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterManager(RegisterManagerViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) && string.IsNullOrWhiteSpace(model.EmployeeId))
            {
                ModelState.AddModelError("", "Enter either a username or an employee ID to look up the employee.");
                return View(model);
            }

            var profile = !string.IsNullOrWhiteSpace(model.Username)
                ? await _employeeFormApiClient.LookupByUsernameAsync(model.Username)
                : await _employeeFormApiClient.LookupByEmployeeIdAsync(model.EmployeeId);

            if (profile == null)
            {
                ModelState.AddModelError("", "No matching employee found in EmployeeForm. They need to be registered there first.");
                return View(model);
            }

            bool alreadyRegistered = _context.SiteVisitingManagers.Any(m => m.EmployeeFormUserId == profile.Id);
            if (alreadyRegistered)
            {
                ModelState.AddModelError("", $"{profile.Name} is already registered as a Manager.");
                return View(model);
            }

            // fix 
            int nextManagerId = _context.SiteVisitingManagers.Any()
                ? _context.SiteVisitingManagers.Max(m => m.ManagerID) + 1: 1;

            var manager = new SiteVisitingManager
            {
                ManagerID = nextManagerId,
                EmployeeFormUserId = profile.Id
            };

            _context.SiteVisitingManagers.Add(manager);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{profile.Name} was registered as a Manager.";
            return RedirectToAction("RegisterManager");
        }
    }
}