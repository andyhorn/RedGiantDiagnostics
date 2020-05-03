using System.Collections.Generic;

namespace API.Contracts.Requests.Admin
{
    public class AdminUserRolesUpdateRequest
    {
        public IEnumerable<string> Roles { get; set; }
    }
}