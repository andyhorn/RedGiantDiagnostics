using System;
using System.Threading.Tasks;
using API.Contracts;
using API.Exceptions;
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
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var user = await _identityService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            
            return Ok(user);
        }

        [HttpGet]
        [Route(Contracts.Routes.Identity.GetUserByEmail)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email address is required.");
            }

            var user = await _identityService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        [Route(Contracts.Routes.Identity.CreateUser)]
        public async Task<IActionResult> CreateUserAsync([FromBody]RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _identityService.CreateUserAsync(request.Email, request.Password);

            return Created(user.Id, user);
        }

        [HttpPut]
        [Route(Contracts.Routes.Identity.UpdateUser)]
        public async Task<IActionResult> UpdateUserAsync(string id, [FromBody]UpdateUserRequest request)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _identityService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user = user.Update(request);

            try
            {
                await _identityService.UpdateUserAsync(user);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
            
            return Ok();
        }

        [HttpDelete]
        [Route(Contracts.Routes.Identity.DeleteUser)]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID is required.");
            }

            try
            {
                await _identityService.DeleteUserAsync(id);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }

            return NoContent();
        }

        [HttpPost]
        [Route(Contracts.Routes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody]UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string token = string.Empty;
            try
            {
                token = await _identityService.LoginAsync(request.Email, request.Password);
            }
            catch (ArgumentException)
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            return Ok(token);
        }
    }
}