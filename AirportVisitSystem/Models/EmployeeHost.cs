using System.ComponentModel.DataAnnotations;

namespace AirportVisitSystem.Models
{
    public class EmployeeHost
    {
        //[Key]
        public int EmployeeID { get; set; }

        public string Name { get; set; }

        public int DepartmentID { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Role { get; set; } 

        public int LoginID { get; set; } // Foreign key to Logins table

        // Links this EmployeeHost to their authoritative record in EmployeeForm.
        // Nullable for now so existing rows aren't broken — every NEW
        // registration going forward should always set this (see step 5).
        // Name/Email/Role/LoginID above are staying in place temporarily;
        // they'll be removed in a later cleanup once nothing reads them
        // locally anymore and everything is live-fetched from EmployeeForm.
        public int? EmployeeFormUserId { get; set; }
    }
}
