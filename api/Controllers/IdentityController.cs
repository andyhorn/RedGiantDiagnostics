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

        public IdentityController(IIdentityService identity)
        {
            _identityService = identity;
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
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route(Contracts.Routes.Identity.CreateUser)]
        public async Task<IActionResult> CreateUserAsync([FromBody]RegisterUserRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route(Contracts.Routes.Identity.UpdateUser)]
        public async Task<IActionResult> UpdateUserAsync(string id, [FromBody]UpdateUserRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route(Contracts.Routes.Identity.DeleteUser)]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route(Contracts.Routes.Identity.Login)]
        public async Task<IActionResult> LoginAsync([FromBody]UserLoginRequest request)
        {
            throw new NotImplementedException();
        }
    }
}