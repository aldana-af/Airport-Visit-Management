namespace AirportVisitSystem.Models
{
    public class Approval
    {
        public int ApprovalID { get; set; }
        public int VisitVisitorID { get; set; }
        public string ApprovalStatus { get; set; }
        public int RequestedEmployeeID { get; set; }
        public int? ApprovingManagerID { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? DecisionDate { get; set; }
    }
}
