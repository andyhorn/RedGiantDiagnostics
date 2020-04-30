using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Contracts
{
    public class UserUpdateRequest
    {
        [Required]
        public string Id { get; set; }
        public string Email { get; set; } = null;
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public IEnumerable<string> Roles { get; set; } = null;
    }
}