using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Requests.Admin
{
    public class AdminPasswordResetRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "New password confirmation is required")]
        [Display(Name = "Confirm new password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}