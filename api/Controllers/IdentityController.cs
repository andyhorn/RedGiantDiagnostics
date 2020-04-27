using System.Threading.Tasks;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route(Contracts.Routes.Controller)]
    public class IdentityController : ControllerBase
    {
        private IIdentityService _identityService;
        private ITokenService _tokenService;

        public IdentityController(IIdentityService identity, ITokenService token)
        {
            _identityService = identity;
            _tokenService = token;
        }

        public async Task<IActionResult> Get()
        {
            var userList = await _identityService.GetAllUsersAsync();
            return Ok(userList);
        }
    }
}