namespace AirportVisitSystem.Models
{
    // Admin is an Airport-specific designation, same as Employee/Manager —
    // being an Admin in EmployeeForm does NOT automatically make someone
    // an Admin here. Airport decides its own roles independently; it just
    // borrows EmployeeForm for identity and credentials.
    public class AirportAdmin
    {
        public int AdminID { get; set; }

        // Links to the authoritative record in EmployeeForm.
        public int EmployeeFormUserId { get; set; }
    }
}
