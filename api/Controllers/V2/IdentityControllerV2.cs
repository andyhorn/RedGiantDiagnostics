using System;
using System.Linq;
using System.Threading.Tasks;
using API.Contracts;
using API.Contracts.Requests;
using API.Contracts.Responses;
using API.Exceptions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V2
{
    [ApiController]

    [Route(Contracts.Routes.ControllerV2)]
    [Route(Contracts.Routes.ControllerV1)]
    public class IdentityController : ControllerBase
    {
        private IIdentityService _identityService;

        public IdentityController(IIdentityService identity)
        {
            _identityService = identity;
        }

        /// <summary>
        /// Allows a logged-in user to retrieve their own user data
        /// </summary>
        /// <param name="token">JWT from the request headers</param>
        /// <returns>IdentityUser object for the logged-in user</returns>
        [AllowAnonymous]
        [HttpGet(Contracts.Routes.Identity.V2.Get)]
        public async Task<IActionResult> Get([FromHeader(Name = "Authorization")] string token)
        {
            // Validate the token string
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Authorization token required");
            }

            // Remove the Bearer title
            if (token.Contains("Bearer "))
                token = token.Substring("Bearer ".Length);

            // Validate the token
            var isValid = await _identityService.ValidateTokenAsync(token);
            if (!isValid)
            {
                return Unauthorized();
            }

            // Retrieve the user from the token
            var user = await _identityService.GetUserFromToken(token);
            if (user == null)
            {
                return NotFound();
            }

            // If everything succeeds, return the user data
            var response = new UserDataResponse(user);
            response.Roles = (await _identityService.GetUserRolesAsync(user)).ToArray();
            return Ok(response);
        }

        /// <summary>
        /// Logs a user in via email and password
        /// </summary>
        /// <param name="loginRequest">UserLoginRequest object containing email and password</param>
        /// <returns>BadRequest, NotFound, Unauthorized, or Ok with a JWT</returns>
        [AllowAnonymous]
        [HttpPost(Contracts.Routes.Identity.V2.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify that a user exists with the given email
            var exists = await _identityService.UserExistsWithEmailAsync(loginRequest.Email);
            if (!exists)
            {
                // If no user was found, return NotFound
                return NotFound();
            }

            // Verify the user is an active "User"
            var user = await _identityService.GetUserByEmailAsync(loginRequest.Email);
            var roles = await _identityService.GetUserRolesAsync(user);
            if (!roles.Contains(Contracts.Roles.User))
            {
                return Unauthorized();
            }

            // Login and retrieve a token
            var token = string.Empty;
            try
            {
                token = await _identityService.LoginAsync(loginRequest.Email, loginRequest.Password);
            }
            catch (ArgumentException)
            {
                if (string.IsNullOrEmpty(token))
                {
                    // If the login failed, return Unauthorized
                    return Unauthorized();
                }
            }

            // Return Ok with the token and user ID
            // var user = await _identityService.GetUserByEmailAsync(loginRequest.Email);
            var response = new TokenResponse(user.Id, token);

            return Ok(response);
        }

        /// <summary>
        /// Allows a logged-in user to update their user info
        /// </summary>
        /// <param name="id">The ID of the user to update</param>
        /// <param name="updateRequest">The UserUpdateRequest object containing changed information</param>
        /// <returns>BadRequest, NotFound, or Ok</returns>
        [Authorize(Policy = Contracts.Policies.IsSelfPolicy)]
        [HttpPut(Contracts.Routes.Identity.V2.Update)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UserUpdateRequest updateRequest)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID is required");
            }

            // Verify a user exists with the given ID
            var exists = await _identityService.UserExistsWithIdAsync(id);
            if (!exists)
            {
                // If no user exists, return NotFound
                return NotFound();
            }

            // Update the user data
            var user = await _identityService.GetUserByIdAsync(id);

            if (!string.IsNullOrEmpty(updateRequest.Email))
            {
                user.Email = updateRequest.Email;
                user.UserName = updateRequest.Email;
            }

            // Save the updated data to the identity store
            try
            {
                await _identityService.UpdateUserAsync(user);
            }
            catch (ActionFailedException)
            {
                // If the update fails, return a server error status code
                return StatusCode(500);
            }

            // If everything succeeds, return an Ok
            return Ok();
        }

        [Authorize(Policy = Contracts.Policies.IsSelfPolicy)]
        [HttpPost(Contracts.Routes.Identity.V2.ChangePassword)]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest request, [FromHeader(Name = "Authorization")] string jwt)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retrieve the user object
            if (jwt.Contains("Bearer"))
            {
                jwt = jwt.Substring("Bearer ".Length);
            }

            var user = await _identityService.GetUserFromToken(jwt);

            // Verify the existence of the user
            if (user == null)
            {
                return NotFound();
            }

            // Verify the current password
            try
            {
                await _identityService.LoginAsync(user.Email, request.CurrentPassword);
            }
            catch (ArgumentException)
            {
                return Unauthorized("Invalid current password");
            }

            // Set the new password
            try
            {
                await _identityService.SetUserPasswordAsync(user, request.NewPassword);
            }
            catch (ActionFailedException)
            {
                // If the change fails, return a server error code
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // Return Ok
            return Ok();
        }
    }
}