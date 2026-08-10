using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AirportVisitSystem.Models
{
    public class CreateVisitViewModel
    {
        [Required]
        public string VisitTitle { get; set; }

        [Required]
        public string VisitDescription { get; set; }

        [Required]
        public int DepartmentID { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }

        [Required]
        public int HostEmployeeID { get; set; }
        public IEnumerable<SelectListItem> Hosts { get; set; }

        [Required]
        public int VisitTypeID { get; set; }
        public IEnumerable<SelectListItem> VisitTypes { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime VisitDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
    }
}
