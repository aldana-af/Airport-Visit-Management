using AirportVisitSystem.Data;
using AirportVisitSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class VisitorController : Controller
{
    private readonly AirportVisitDatabase1 _context;
    public VisitorController(AirportVisitDatabase1 context) => _context = context;

    [Authorize(Roles = "Employee,Manager")]
    public async Task<IActionResult> Index(string searchTerm)
    {
        ViewData["Role"] = User.IsInRole("Employee") ? "Employee" : "Manager";
        ViewData["Title"] = "Visitors";
        ViewData["SearchTerm"] = searchTerm;

        var query = _context.Visitors.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(v => v.Name.Contains(searchTerm));

        var visitors = await query.OrderBy(v => v.Name).ToListAsync();
        return View(visitors);
    }

    [Authorize(Roles = "Employee")]
    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Role"] = "Employee";
        ViewData["Title"] = "Add Visitor";
        return View(new CreateVisitorViewModel());
    }

    [Authorize(Roles = "Employee")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateVisitorViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Role"] = "Employee";
            ViewData["Title"] = "Add Visitor";
            return View(vm);
        }

        _context.Visitors.Add(new Visitor
        {
            Name = vm.Name,
            Organization = vm.Organization,
            Position = vm.Position,
            Phone = vm.Phone,
            Email = vm.Email
        });
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}