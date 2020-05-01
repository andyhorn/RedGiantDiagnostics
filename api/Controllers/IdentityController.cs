using System;
using System.Linq;
using System.Threading.Tasks;
using API.Contracts;
using API.Exceptions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = Contracts.Roles.Admin)]
    [ApiController]
    [Route(Contracts.Routes.ControllerV1)]
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

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost]
        [Route(Contracts.Routes.Identity.CreateUser)]
        public async Task<IActionResult> CreateUserAsync([FromBody]UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var user = await _identityService.CreateUserAsync(request);
                var response = new UserRegistrationResponse
                {
                    UserId = user.Id,
                    Token = await _identityService.LoginAsync(request.Email, request.Password)
                };

                return Created(user.Id, response);
            }
            catch (ResourceConflictException)
            {
                return Conflict();
            }
            catch (ActionFailedException e)
            {
                return BadRequest(e.Errors);
            }
        }

        /// <summary>
        /// Updates a user's identity information. Users can update their own information;
        /// Administrators can update anyone's information.
        /// </summary>
        /// <param name="request">A UserUpdateRequest object containing the new values.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route(Contracts.Routes.Identity.UpdateUser)]
        public async Task<IActionResult> UpdateUserAsync([FromBody]UserUpdateRequest request)
        {
            // Verify the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify the user exists with the specified ID
            var exists = await _identityService.GetUserByIdAsync(request.Id) != null;
            if (!exists)
            {
                return NotFound();
            }
            // var user = await _identityService.GetUserByIdAsync(request.Id);
            // if (user == null)
            // {
            //     return NotFound();
            // }

            // Update the user data
            try
            {
                await _identityService.UpdateUserAsync(request);
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

        [AllowAnonymous]
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

            var user = await _identityService.GetUserFromToken(token);
            var userId = user.Id;

            var response = new TokenResponse(userId, token);

            return Ok(response);
        }
    }
}