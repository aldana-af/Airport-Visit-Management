namespace AirportVisitSystem.Models.EmployeeFormApi
{
    // Sent to EmployeeForm's POST /api/auth/verify
    public class EmployeeFormAuthRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    // Mirrors EmployeeForm's AuthVerifyResponse. Kept as a separate copy
    // (not shared code) since these are two independent apps — this is
    // just Airport's understanding of what EmployeeForm sends back.
    public class EmployeeFormAuthResult
    {
        public bool Success { get; set; }
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string EmployeeId { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
    }

    // Mirrors EmployeeForm's EmployeeLookupResponse.
    public class EmployeeFormProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public string EmployeeId { get; set; }
        public string Phone { get; set; }
    }
}