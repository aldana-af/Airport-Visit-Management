namespace AirportVisitSystem.Models
{
    public class Visit
    {
        public int VisitID { get; set; }

        public string VisitTitle { get; set; }

        public string VisitDescription { get; set; }

        public int DepartmentID { get; set; }

        public int HostEmployeeID { get; set; }

        public string VisitStatus { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime VisitDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int VisitTypeID { get; set; }
    }
}
