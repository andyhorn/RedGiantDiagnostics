using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Requests
{
    public class UserUpdateRequest
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; } = null;

        [Display(Name = "First name")]
        public string FirstName { get; set; } = null;

        [Display(Name = "Last name")]
        public string LastName { get; set; } = null;
    }
}