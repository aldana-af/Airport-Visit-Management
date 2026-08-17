using System.ComponentModel.DataAnnotations;

namespace AirportVisitSystem.Models
{
    // Shared by both registration forms: identify the person via EmployeeForm
    // before anything gets created locally. Either field can be used —
    // validated in the controller, not here, since "at least one of two"
    // isn't a clean single-property DataAnnotation.
    public class RegisterEmployeeHostViewModel
    {
        public string Username { get; set; }

        // not to be confused with BadgeID which is used for visitors access badges
        public string EmployeeId { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentID { get; set; }
    }

    public class RegisterManagerViewModel
    {
        public string Username { get; set; }
        public string EmployeeId { get; set; }
    }
}
