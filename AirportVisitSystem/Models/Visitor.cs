
using System.ComponentModel.DataAnnotations;

namespace AirportVisitSystem.Models
{
    public class Visitor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        public string Organization { get; set; }

        public string Position { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        //public string VisitorStatus { get; set; }
    }
}
