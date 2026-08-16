namespace AirportVisitSystem.Models
{
    public class SiteVisitingManager
    {
        public int ManagerID { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public int ManagerLoginID { get; set; } // Foreign key to Logins table

        // Links this manager to their authoritative record in EmployeeForm.
        // See the matching note in EmployeeHost.cs.
        public int? EmployeeFormUserId { get; set; }
    }
}
