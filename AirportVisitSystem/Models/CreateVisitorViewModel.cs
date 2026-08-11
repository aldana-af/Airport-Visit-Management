using System.ComponentModel.DataAnnotations;

namespace AirportVisitSystem.Models
{
    public class CreateVisitorViewModel
    {
        [Required(ErrorMessage = "Visitor Name cannot be empty.")]
        public string Name { get; set; }

        [Required]
        public string Organization { get; set; }

        public string Position { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
