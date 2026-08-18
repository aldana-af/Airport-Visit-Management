namespace AirportVisitSystem.Models
{
    public class AdminRosterRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AdminHomeViewModel
    {
        public List<AdminRosterRow> Hosts { get; set; } = new();
        public List<AdminRosterRow> Managers { get; set; } = new();
    }
}