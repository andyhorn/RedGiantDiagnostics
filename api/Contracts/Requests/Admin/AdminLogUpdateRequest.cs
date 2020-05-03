using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Requests.Admin
{
    public class AdminLogUpdateRequest : LogUpdateRequest, ILogUpdateRequest
    {
        [Display(Name = "Owner ID")]
        public string OwnerId { get; set; } = null;
    }
}