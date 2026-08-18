using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AirportVisitSystem.Models
{
    public class EditVisitViewModel
    {
        public int VisitID { get; set; }

        [Required(ErrorMessage = "Visit Title is required.")]
        public string VisitTitle { get; set; }

        [Required]
        public string VisitDescription { get; set; }

        [Required]
        public int VisitTypeID { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> VisitTypes { get; set; }

        [Required(ErrorMessage = "Specified date required for visit.")]
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