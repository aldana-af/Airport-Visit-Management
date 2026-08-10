namespace AirportVisitSystem.Models
{
    public class VisitDetailsViewModel
    {
        public Visit Visit { get; set; }
        public Department Department { get; set; }
        public EmployeeHost Host { get; set; }
        public VisitType VisitType { get; set; }
        public List<VisitDetailsVisitorRow> Visitors { get; set; }
    }
}
