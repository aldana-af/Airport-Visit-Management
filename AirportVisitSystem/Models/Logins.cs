using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace AirportVisitSystem.Models
{
    public class Logins
    {
        public int LoginID { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; } 
    }
}
