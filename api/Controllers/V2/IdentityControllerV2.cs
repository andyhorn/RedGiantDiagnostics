using System;
using System.Threading.Tasks;
using API.Contracts;
using API.Exceptions;
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
            // Validate the token string
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Authorization token required");
            }

            // Remove the Bearer title
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

        [HttpPost, Route(Contracts.Routes.Identity.V2.Login)]
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

        [HttpPut, Route(Contracts.Routes.Identity.V2.Update)]
        public async Task<IActionResult> Update([FromBody]UserUpdateRequest updateRequest)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify a user exists with the given ID
            var exists = await _identityService.UserExistsWithIdAsync(updateRequest.Id);
            if (!exists)
            {
                // If no user exists, return NotFound
                return NotFound();
            }

            // Update the user data
            try
            {
                await _identityService.UpdateUserAsync(updateRequest);
            }
            catch (ActionFailedException)
            {
                // If the update fails, return a server error status code
                return StatusCode(500);
            }

            // If everything succeeds, return an Ok
            return Ok();
        }
    }
}