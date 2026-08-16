using AirportVisitSystem.Data;
using AirportVisitSystem.Models;
using AirportVisitSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AccountController : Controller
{
    // home computer
    //private readonly AirportVisitDatabase1 _context;
    //public AccountController(AirportVisitDatabase1 context, IEmployeeFormApiClient employeeFormApiClient)
    //{
    //    _context = context;
    //    _employeeFormApiClient = employeeFormApiClient;
    //}

    // office computer
    private readonly AirportVisitDb _context;
    private readonly IEmployeeFormApiClient _employeeFormApiClient;

    public AccountController(AirportVisitDb context, IEmployeeFormApiClient employeeFormApiClient)
    {
        _context = context;
        _employeeFormApiClient = employeeFormApiClient;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Credentials are verified against EmployeeForm now — Airport no
        // longer stores or checks passwords itself. See EmployeeForm's
        // POST /api/auth/verify (EmployeeApiController).
        var authResult = await _employeeFormApiClient.VerifyLoginAsync(model.Username, model.Password);

        if (!authResult.Success || authResult.Id == null)
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        int employeeFormUserId = authResult.Id.Value;

        // Being a valid EmployeeForm login isn't enough by itself — the
        // person also has to already be registered as an EmployeeHost
        // and/or SiteVisitingManager here in Airport (see step 5's
        // registration flow, which links new rows by EmployeeFormUserId).
        bool isEmployee = _context.EmployeeHosts.Any(e => e.EmployeeFormUserId == employeeFormUserId);
        bool isManager = _context.SiteVisitingManagers.Any(m => m.EmployeeFormUserId == employeeFormUserId);

        var claims = new List<Claim>
        {
            // NOTE: this now carries EmployeeFormUserId, not the old Airport
            // LoginID. Every controller that reads ClaimTypes.NameIdentifier
            // to look up the current EmployeeHost/SiteVisitingManager must
            // match against EmployeeFormUserId too (already updated —
            // see ApprovalController, CheckInController, HomeController, VisitController).
            new Claim(ClaimTypes.NameIdentifier, employeeFormUserId.ToString()),
            new Claim(ClaimTypes.Name, model.Username)
        };
        if (isEmployee) claims.Add(new Claim(ClaimTypes.Role, "Employee"));
        if (isManager) claims.Add(new Claim(ClaimTypes.Role, "Manager"));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        // Overlap case: let them choose instead of guessing
        if (isEmployee && isManager) return RedirectToAction("ChoosePortal");
        if (isManager) return RedirectToAction("Index", "Approval", new { area = "Manager" });
        if (isEmployee) return RedirectToAction("Index", "Visit", new { area = "Employee" });

        ModelState.AddModelError("", "This EmployeeForm account isn't linked to an Employee or Manager record in Airport yet.");
        return View(model);
    }

    public IActionResult ChoosePortal() => View();

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();
}
