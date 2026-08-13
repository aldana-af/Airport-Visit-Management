namespace AirportVisitSystem.Models
{
    public class CheckInViewModel
    {
        public int VisitID { get; set; }

        public string VisitTitle { get; set; }

        public DateTime CurrentTime  { get; set; }

        public List<CheckInVisitorRow> Visitors { get; set; } 
    }
}
