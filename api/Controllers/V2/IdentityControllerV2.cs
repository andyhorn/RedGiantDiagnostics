using System;
using System.Threading.Tasks;
using API.Contracts;
using API.Contracts.Requests;
using API.Exceptions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V2
{
    [ApiController]
    [Authorize(Policy = Contracts.Policies.ResourceOwnerPolicy)]
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
        /// <param name=""Authorization"">JWT from the request headers</param>
        /// <returns>IdentityUser object for the logged-in user</returns>
        [AllowAnonymous]
        [HttpGet(Contracts.Routes.Identity.V2.Get)]
        public async Task<IActionResult> Get([FromHeader(Name = "Authorization")]string token)
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
            return Ok(user);
        }

        /// <summary>
        /// Logs a user in via email and password
        /// </summary>
        /// <param name="loginRequest">UserLoginRequest object containing email and password</param>
        /// <returns>BadRequest, NotFound, Unauthorized, or Ok with a JWT</returns>
        [AllowAnonymous]
        [HttpPost(Contracts.Routes.Identity.V2.Login)]
        public async Task<IActionResult> Login([FromBody]UserLoginRequest loginRequest)
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

            // Login and retrieve a token
            var token = await _identityService.LoginAsync(loginRequest.Email, loginRequest.Password);
            if (string.IsNullOrEmpty(token))
            {
                // If the login failed, return Unauthorized
                return Unauthorized();
            }

            // Return Ok with the token and user ID
            var user = await _identityService.GetUserByEmailAsync(loginRequest.Email);
            var response = new TokenResponse(user.Id, token);

            return Ok(response);
        }

        /// <summary>
        /// Allows a logged-in user to update their user info
        /// </summary>
        /// <param name="id">The ID of the user to update</param>
        /// <param name="updateRequest">The UserUpdateRequest object containing changed information</param>
        /// <returns>BadRequest, NotFound, or Ok</returns>
        [HttpPut(Contracts.Routes.Identity.V2.Update)]
        public async Task<IActionResult> Update([FromRoute]string id, [FromBody]UserUpdateRequest updateRequest)
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
            user.Map<UserUpdateRequest>(updateRequest);

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

        [HttpPost(Contracts.Routes.Identity.V2.ChangePassword)]
        public async Task<IActionResult> ChangePassword([FromBody]PasswordChangeRequest request, [FromHeader(Name = "Authorization")]string jwt)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retrieve the user object
            var user = await _identityService.GetUserFromToken(jwt);

            // Verify the existence of the user
            if (user == null)
            {
                return NotFound();
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