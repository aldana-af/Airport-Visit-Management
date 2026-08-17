using AirportVisitSystem.Models.EmployeeFormApi;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AirportVisitSystem.Services
{
    // Talks to EmployeeForm over HTTP. BaseAddress and the X-Api-Key header
    // are set once, in Program.cs, when this typed client is registered —
    // this class only knows the relative paths and how to shape the calls.
    public class EmployeeFormApiClient : IEmployeeFormApiClient
    {
        private readonly HttpClient _httpClient;

        public EmployeeFormApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EmployeeFormAuthResult> VerifyLoginAsync(string username, string password)
        {
            var request = new EmployeeFormAuthRequest { Username = username, Password = password };

            // EmployeeForm returns 200 on success and 401 on bad credentials —
            // both are "the call worked, here's the answer," so we read the
            // body either way rather than throwing on non-2xx.
            var response = await _httpClient.PostAsJsonAsync("api/auth/verify", request);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var result = await response.Content.ReadFromJsonAsync<EmployeeFormAuthResult>();
                return result ?? new EmployeeFormAuthResult { Success = false, Message = "Empty response from EmployeeForm." };
            }

            // Anything else (API key rejected, EmployeeForm unreachable, 500, etc.)
            // is a system problem, not "wrong password" — surface it distinctly
            // so the login screen can show something more useful than
            // "invalid username or password" when the real issue is connectivity.
            return new EmployeeFormAuthResult
            {
                Success = false,
                Message = $"EmployeeForm returned an unexpected status: {(int)response.StatusCode}."
            };
        }

        public Task<EmployeeFormProfile> LookupByUsernameAsync(string username)
            => LookupAsync($"api/employees/lookup?username={Uri.EscapeDataString(username)}");

        public Task<EmployeeFormProfile> LookupByEmployeeIdAsync(string employeeId)
            => LookupAsync($"api/employees/lookup?employeeId={Uri.EscapeDataString(employeeId)}");

        private async Task<EmployeeFormProfile> LookupAsync(string relativeUrl)
        {
            var response = await _httpClient.GetAsync(relativeUrl);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // No matching employee in EmployeeForm — this is an expected,
                // normal outcome (e.g. someone not onboarded yet), not an error.
                return null;
            }

            response.EnsureSuccessStatusCode(); // throws for API key issues, 500s, etc.

            return await response.Content.ReadFromJsonAsync<EmployeeFormProfile>();
        }
    }
}