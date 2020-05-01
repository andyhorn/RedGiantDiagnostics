using System;
using System.Threading.Tasks;
using API.Contracts;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V2
{
    [ApiController]
    [Route(Contracts.Routes.ControllerV2)]
    public class IdentityControllerV2 : ControllerBase
    {
        private IIdentityService _identityService;

        public IdentityControllerV2(IIdentityService identity)
        {
            _identityService = identity;
        }

        [HttpGet, Route(Contracts.Routes.Identity.V2.Get)]
        public async Task<IActionResult> Get([FromHeader(Name = "Authorization")]string token)
        {
            throw new NotImplementedException();
        }

        [HttpPost, Route(Contracts.Routes.Identity.V2.Login)]
        public async Task<IActionResult> Login([FromBody]UserLoginRequest loginRequest)
        {
            throw new NotImplementedException();
        }

        [HttpPut, Route(Contracts.Routes.Identity.V2.Update)]
        public async Task<IActionResult> Update([FromBody]UserUpdateRequest updateRequest)
        {
            throw new NotImplementedException();
        }
    }
}