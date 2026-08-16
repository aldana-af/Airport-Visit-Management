using AirportVisitSystem.Models.EmployeeFormApi;
using System.Threading.Tasks;

namespace AirportVisitSystem.Services
{
    // Airport's window into EmployeeForm. Every call to EmployeeForm's
    // API — for login and for registration lookups — goes through here,
    // so there's exactly one place that knows the URLs/headers involved.
    public interface IEmployeeFormApiClient
    {
        // Calls POST /api/auth/verify. Used by AccountController's login flow —
        // Airport no longer checks its own Logins table for credentials.
        Task<EmployeeFormAuthResult> VerifyLoginAsync(string username, string password);

        // Calls GET /api/employees/lookup?username=... Used when registering
        // an EmployeeHost/SiteVisitingManager to confirm the person already
        // exists in EmployeeForm. Returns null if no match (404).
        Task<EmployeeFormProfile> LookupByUsernameAsync(string username);

        // Same lookup, by badge ID instead of username.
        Task<EmployeeFormProfile> LookupByBadgeIdAsync(string badgeId);
    }
}