using AirportVisitSystem.Data;
using AirportVisitSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AccountController : Controller
{
    // home computer
    private readonly AirportVisitDatabase1 _context;
    // office computer
    //private readonly AirportVisitDb _context;

    private readonly PasswordHasher<object> _hasher = new();

    // home computer
    public AccountController(AirportVisitDatabase1 context)
     // office computer
     //public AccountController(AirportVisitDb context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var login = _context.Logins.FirstOrDefault(l => l.Username == model.Username);
        if (login == null || login.PasswordHash != model.Password)
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        bool isEmployee = _context.EmployeeHosts.Any(e => e.LoginID == login.LoginID);
        bool isManager = _context.SiteVisitingManagers.Any(m => m.ManagerLoginID == login.LoginID);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, login.LoginID.ToString()),
            new Claim(ClaimTypes.Name, login.Username)
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

        ModelState.AddModelError("", "This login isn't linked to an Employee or Manager record.");
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

