namespace AirportVisitSystem.Models
{
    public class VisitDetailsVisitorRow
    {
        public int VisitVisitorID { get; set; }
        public string Name { get; set; }
        public string VisitorStatus { get; set; }
        public string BadgeNumber { get; set; } // new
        public DateTime? CheckIn { get; set; }  // new
        public DateTime? CheckOut { get; set; } // new
    }
}
