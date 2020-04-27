using System;
using System.Threading.Tasks;
using API.Contracts;
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

        [HttpGet]
        [Route(Contracts.Routes.Identity.GetAllUsers)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var userList = await _identityService.GetAllUsersAsync();
            return Ok(userList);
        }

        [HttpGet]
        [Route(Contracts.Routes.Identity.GetUserById)]
        public async Task<IActionResult> GetUserById(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route(Contracts.Routes.Identity.CreateUser)]
        public async Task<IActionResult> CreateUser([FromBody]RegisterUserRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route(Contracts.Routes.Identity.UpdateUser)]
        public async Task<IActionResult> UpdateUser([FromBody]UpdateUserRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route(Contracts.Routes.Identity.DeleteUser)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            throw new NotImplementedException();
        }
    }
}