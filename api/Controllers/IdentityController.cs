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

        public IActionResult Get()
        {
            return Ok("hello world!");
        }
    }
}