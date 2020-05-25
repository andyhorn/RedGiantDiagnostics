using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Requests.Admin
{
    public class AdminPasswordResetRequest
    {
        [Required(ErrorMessage = "New password is required")]
        [Display(Name = "New password")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [MaxLength(24, ErrorMessage = "Password cannot be more than 24 characters")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "New password confirmation is required")]
        [Display(Name = "Confirm new password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}