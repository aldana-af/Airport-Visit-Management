using AirportVisitSystem.Data;
using AirportVisitSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class VisitorController : Controller
{
    // home computer
    //private readonly AirportVisitDatabase1 _context;
    //public VisitorController(AirportVisitDatabase1 context) => _context = context;

    // office computer
    private readonly AirportVisitDb _context;
    public VisitorController(AirportVisitDb context) => _context = context;


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

    // editing visitor info

    [Authorize(Roles = "Employee")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.VisitorID == id);
        if (visitor == null) return NotFound();

        ViewData["Role"] = "Employee";
        ViewData["Title"] = "Edit Visitor";

        var vm = new EditVisitorViewModel
        {
            VisitorID = visitor.VisitorID,
            Name = visitor.Name,
            Organization = visitor.Organization,
            Position = visitor.Position,
            Phone = visitor.Phone,
            Email = visitor.Email
        };
        return View(vm);
    }

    [Authorize(Roles = "Employee")] // add Roles = "Manager"
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditVisitorViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Role"] = "Employee";
            ViewData["Title"] = "Edit Visitor";
            return View(vm);
        }

        var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.VisitorID == vm.VisitorID);
        if (visitor == null) return NotFound();

        visitor.Name = vm.Name;
        visitor.Organization = vm.Organization;
        visitor.Position = vm.Position;
        visitor.Phone = vm.Phone;
        visitor.Email = vm.Email;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}