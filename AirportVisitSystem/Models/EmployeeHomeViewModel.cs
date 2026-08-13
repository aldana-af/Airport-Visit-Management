namespace AirportVisitSystem.Models
{
    public class EmployeeHomeViewModel
    {
        public List<TodayVisitRow> TodaysVisits { get; set; }

        public List<Visit> UpcomingVisits { get; set; }
    }
}
