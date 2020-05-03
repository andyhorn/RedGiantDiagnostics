using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Requests.Admin
{
    public class AdminUserRegistrationRequest
    {
        [Required(ErrorMessage = "Email address is required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password confirmation is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}