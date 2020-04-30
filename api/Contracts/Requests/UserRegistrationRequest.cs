using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Contracts
{
    public class UserRegistrationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public List<string> Roles { get; set; } = new List<string> { Contracts.Roles.User };
    }
}