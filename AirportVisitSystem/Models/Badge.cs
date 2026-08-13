namespace AirportVisitSystem.Models
{
    public class Badge
    {
        public int BadgeID { get; set; }

        public string BadgeNumber { get; set; }

        public string Status {  get; set; } // "Active" --> badge in user , "Inactive" --> badge available
    }
}
