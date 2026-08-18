using AirportVisitSystem.Models.EmployeeFormApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirportVisitSystem.Services
{
    // DEV/TEST ONLY. Stands in for EmployeeFormApiClient when
    // EmployeeFormApi:UseMock is true in config — returns instant, fake
    // data instead of making real HTTP calls. Lets you test Airport-only
    // features (check-in, approvals, UI work, etc.) without EmployeeForm
    // running at all, and with zero timeout/connectivity risk.
    //
    // NEVER trust this for real credential verification — VerifyLoginAsync
    // here accepts ANY password for a known username. Only ever wired up
    // when the config flag is explicitly on (see Program.cs); the real
    // EmployeeFormApiClient is used otherwise.
    public class MockEmployeeFormApiClient : IEmployeeFormApiClient
    {
        // Add/edit fake people here as needed for whatever you're testing.
        // Ids/EmployeeIds are made up — they don't need to match anything
        // real in EmployeeForm since this client never talks to it.
        private static readonly List<EmployeeFormProfile> _fakeEmployees = new()
        {
            new EmployeeFormProfile { Id = 7, Name = "Mock Employee One", Email = "mock1@example.com", Role = "Employee", Username = "mockemployee1", EmployeeId = "9001", Phone = "555-0001" },
            new EmployeeFormProfile { Id = 90002, Name = "Mock Manager One", Email = "mock2@example.com", Role = "Manager", Username = "mockmanager1", EmployeeId = "9002", Phone = "555-0002" },
            new EmployeeFormProfile { Id = 1, Name = "Mock Admin One", Email = "mock3@example.com", Role = "Admin", Username = "mockadmin1", EmployeeId = "9003", Phone = "555-0003" },
        };

        public Task<EmployeeFormAuthResult> VerifyLoginAsync(string username, string password)
        {
            var match = _fakeEmployees.FirstOrDefault(e => e.Username == username);

            if (match == null)
            {
                return Task.FromResult(new EmployeeFormAuthResult
                {
                    Success = false,
                    Message = "Invalid username or password. (mock)"
                });
            }

            // Mock mode: password isn't actually checked — any value works
            // for a known mock username. This is intentional for fast
            // testing, never used against real accounts.
            return Task.FromResult(new EmployeeFormAuthResult
            {
                Success = true,
                Id = match.Id,
                Name = match.Name,
                Email = match.Email,
                Role = match.Role,
                EmployeeId = match.EmployeeId,
                Phone = match.Phone
            });
        }

        public Task<EmployeeFormProfile> LookupByUsernameAsync(string username)
        {
            var match = _fakeEmployees.FirstOrDefault(e => e.Username == username);
            return Task.FromResult(match);
        }

        public Task<EmployeeFormProfile> LookupByEmployeeIdAsync(string employeeId)
        {
            var match = _fakeEmployees.FirstOrDefault(e => e.EmployeeId == employeeId);
            return Task.FromResult(match);
        }
    }
}