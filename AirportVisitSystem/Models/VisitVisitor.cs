namespace AirportVisitSystem.Models
{
    public class VisitVisitor
    {
        public int VisitVisitorID { get; set; }
        public int VisitorID { get; set; }
        public int VisitID { get; set; }
        public int? BadgeID { get; set; }
        public string VisitorStatus { get; set; } // Allowed, Denied, Pending
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
    }
}
