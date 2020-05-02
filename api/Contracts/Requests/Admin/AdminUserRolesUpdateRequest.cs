using System.Collections.Generic;

namespace API.Contracts.Requests.Admin
{
    public class AdminUserRolesUpdateRequest
    {
        public List<string> RemoveRoles { get; set; }
        public List<string> AddRoles { get; set; }
    }
}