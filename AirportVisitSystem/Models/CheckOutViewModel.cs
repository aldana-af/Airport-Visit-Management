namespace AirportVisitSystem.Models
{
    public class CheckOutViewModel
    {
        public int VisitID { get; set; }
        public string VisitTitle { get; set; }
        public DateTime CurrentTime { get; set; }
        public List<string> VisitorNames { get; set; }
    }
}
