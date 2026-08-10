namespace AirportVisitSystem.Models
{
    public class SiteVisitingManager
    {
        public int ManagerID { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public int ManagerLoginID { get; set; } // Foreign key to Logins table
    }
}
