namespace AirportVisitSystem.Models
{
    public class ApprovalQueueRow
    {
        public int VisitID { get; set; }
        public string VisitTitle { get; set; }
        public DateTime VisitDate { get; set; }
        public string RequestedByName { get; set; }
    }
}
