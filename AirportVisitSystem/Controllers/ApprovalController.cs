using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AirportVisitSystem.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class ApprovalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
