using Microsoft.AspNetCore.Mvc;

namespace AirportVisitSystem.Controllers
{
    public class VisitorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
