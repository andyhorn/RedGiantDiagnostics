using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Requests
{
    public class PasswordChangeRequest
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Current password")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}