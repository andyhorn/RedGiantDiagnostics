using System.ComponentModel.DataAnnotations;

namespace API.Contracts
{
    public class PasswordChangeRequest
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}