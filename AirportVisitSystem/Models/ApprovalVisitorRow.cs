namespace AirportVisitSystem.Models
{
    public class ApprovalVisitorRow
    {
        public int VisitVisitorID { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsAllowed { get; set; }
    }
}
