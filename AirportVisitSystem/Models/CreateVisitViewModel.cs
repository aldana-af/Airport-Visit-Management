using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        public int VisitTypeID { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> VisitTypes { get; set; }

        public List<int> SelectedVisitorIds { get; set; } = new();

        [ValidateNever]
        public List<Visitor> AllVisitors { get; set; }

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
