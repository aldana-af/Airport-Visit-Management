using AirportVisitSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AirportVisitSystem.Controllers
{
    // TEMPORARY — sanity-checks that Airport can actually reach EmployeeForm's
    // API through IEmployeeFormApiClient before we wire it into real login/
    // registration flows. No [Authorize] on purpose, so it's easy to hit from
    // a browser while testing. Delete this whole file once step 3 is confirmed.
    public class DiagnosticsController : Controller
    {
        private readonly IEmployeeFormApiClient _employeeFormApiClient;

        public DiagnosticsController(IEmployeeFormApiClient employeeFormApiClient)
        {
            _employeeFormApiClient = employeeFormApiClient;
        }

        // GET /Diagnostics/LookupByUsername?username=afelemban
        public async Task<IActionResult> LookupByUsername(string username)
        {
            var profile = await _employeeFormApiClient.LookupByUsernameAsync(username);

            if (profile == null)
            {
                return Json(new { found = false, message = $"No EmployeeForm user matched username '{username}'." });
            }

            return Json(new { found = true, profile });
        }

        // GET /Diagnostics/VerifyLogin?username=afelemban&password=aldana1104
        public async Task<IActionResult> VerifyLogin(string username, string password)
        {
            var result = await _employeeFormApiClient.VerifyLoginAsync(username, password);
            return Json(result);
        }
    }
}