namespace AirportVisitSystem.Models
{
    public class EmployeeHost
    {
        public int EmployeeID { get; set; }

        public string Name { get; set; }

        public int DepartmentID { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Role { get; set; } 

        public int LoginID { get; set; } // Foreign key to Logins table
    }
}
