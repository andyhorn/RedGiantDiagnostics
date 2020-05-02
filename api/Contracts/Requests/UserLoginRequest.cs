using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Requests
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "Email address is required")]
        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}