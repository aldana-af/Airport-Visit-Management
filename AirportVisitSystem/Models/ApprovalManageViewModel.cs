namespace AirportVisitSystem.Models
{
    public class ApprovalManageViewModel
    {
        public Visit Visit { get; set; }
        public EmployeeHost RequestedBy { get; set; }
        public Department Department { get; set; }
        public VisitType VisitType { get; set; }
        public List<ApprovalVisitorRow> Visitors { get; set; }
    }
}
