using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Contracts;
using API.Contracts.Requests;
using API.Contracts.Requests.Admin;
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
    [Authorize(Policy = Contracts.Policies.AdministrativeAccessPolicy)]
    [Route(Contracts.Routes.ControllerV2)]
    [Route(Contracts.Routes.ControllerV1)]
    public class AdminController : ControllerBase
    {
        private IIdentityService _identityService;
        private ILogsService _logsService;

        public AdminController(IIdentityService identity, ILogsService logs)
        {
            _identityService = identity;
            _logsService = logs;
        }

        [HttpGet(Contracts.Routes.Administrator.Users.GetAll)]
        public async Task<IActionResult> GetAllUsers()
        {
            // Instantiate a list of UserDataResponse object
            var userList = new List<UserDataResponse>();

            // Retrieve the list of users
            var users = _identityService.GetAllUsers().ToList();

            // Validate the users object
            if (users != null && users.Count() > 0)
            {
                // Create a UserDataResponse object from each user
                // and add it to the response list
                foreach (var user in users)
                {
                    var response = new UserDataResponse(user);
                    response.Roles = (await _identityService.GetUserRolesAsync(user)).ToArray();
                    userList.Add(response);
                }
            }

            // Return the list in an Ok result
            return Ok(userList);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="request">UserRegistrationRequest containing new user data</param>
        /// <returns>Created with the new user's ID or BadRequest</returns>
        [HttpPost(Contracts.Routes.Administrator.Users.Register)]
        public async Task<IActionResult> RegisterUser([FromBody]AdminUserRegistrationRequest request)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify no user exists with the same email address
            var exists = await _identityService.UserExistsWithEmailAsync(request.Email);
            if (exists)
            {
                return Conflict();
            }

            // Create the new user object
            var newUser = new IdentityUser
            {
                Email = request.Email,
                UserName = request.Email
            };

            // Save the new user to the identity store
            try
            {
                newUser = await _identityService.CreateUserAsync(newUser, request.Password);
            }
            catch (ActionFailedException)
            {
                return StatusCode(500);
            }

            // Verify the user object was created
            if (newUser == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var response = new UserDataResponse(newUser);
            return Created(newUser.Id, response);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="request">UserUpdateRequest containing updated information</param>
        /// <returns>Ok or BadRequest</returns>
        [HttpPut(Contracts.Routes.Administrator.Users.Update)]
        public async Task<IActionResult> UpdateUser([FromRoute]string id, [FromBody]AdminUserUpdateRequest request)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify a user exists with the given ID
            var exists = await _identityService.UserExistsWithIdAsync(id);
            if (!exists)
            {
                return NotFound();
            }

            // Update the user object
            var user = await _identityService.GetUserByIdAsync(id);
            // user.Map<AdminUserUpdateRequest>(request);

            if (!string.IsNullOrEmpty(request.Email)) 
            {
                user.Email = request.Email;
                user.UserName = request.Email;
            }

            // Save the updated user object to the identity store
            try
            {
                await _identityService.UpdateUserAsync(user);
            }
            catch (ActionFailedException)
            {
                // If the update failed, return a 500 status code
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // If everything succeeds, return an OK
            return Ok();
        }

        [HttpPost(Contracts.Routes.Administrator.Users.SetPassword)]
        public async Task<IActionResult> SetUserPassword([FromBody]AdminPasswordResetRequest request)
        {
            // Validate the Model State
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify the user exists
            var exists = await _identityService.UserExistsWithIdAsync(request.UserId);
            if (!exists) 
            {
                return NotFound();
            }

            var user = await _identityService.GetUserByIdAsync(request.UserId);
            await _identityService.SetUserPasswordAsync(user, request.NewPassword);

            return Ok();
        }

        /// <summary>
        /// Delete a user from the identity store
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>NoContent, NotFound, or BadRequest</returns>
        [Authorize(Policy = Contracts.Policies.SelfOwnedResourceExclusionPolicy)]
        [HttpDelete(Contracts.Routes.Administrator.Users.Delete)]
        public async Task<IActionResult> DeleteUser([FromRoute]string id)
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID is required");
            }

            // Verify a user exists with the given ID
            var exists = await _identityService.UserExistsWithIdAsync(id);
            if (!exists)
            {
                return NotFound();
            }

            // Delete the user object
            try
            {
                await _identityService.DeleteUserAsync(id);
            }
            catch (ActionFailedException)
            {
                // If the delete fails, return an error status code
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // If everything succeeds, return a No Content
            return NoContent();
        }

        [Authorize(Policy = Contracts.Policies.SelfOwnedResourceExclusionPolicy)]
        [HttpPut(Contracts.Routes.Administrator.Users.Roles)]
        public async Task<IActionResult> SetUserRoles([FromRoute]string id, [FromBody]AdminUserRolesUpdateRequest request)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID is required");
            }

            // Verify the user exists
            var exists = await _identityService.UserExistsWithIdAsync(id);
            if (!exists)
            {
                return NotFound();
            }

            // Retrieve the user object
            var user = await _identityService.GetUserByIdAsync(id);

            // Update the user's roles
            try
            {
                await _identityService.SetUserRolesAsync(user, request.Roles);
            }
            catch (ActionFailedException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        [HttpGet(Contracts.Routes.Administrator.Users.Roles)]
        public async Task<IActionResult> GetUserRoles([FromRoute]string id)
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID is required");
            }

            // Verify the user exists
            var exists = await _identityService.UserExistsWithIdAsync(id);
            if (!exists)
            {
                return NotFound();
            }

            // Retrieve the user's roles
            var user = await _identityService.GetUserByIdAsync(id);
            var roles = await _identityService.GetUserRolesAsync(user);

            return Ok(roles);
        }

        /// <summary>
        /// Retrieves user data
        /// </summary>
        /// <param name="id">The ID of the user to retrieve</param>
        /// <returns>Ok, NotFound, or BadRequest</returns>
        [HttpGet(Contracts.Routes.Administrator.Users.GetById)]
        public async Task<IActionResult> GetUserById([FromRoute]string id)
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID is required");
            }

            // Verify a user exists with the given ID
            var exists = await _identityService.UserExistsWithIdAsync(id);
            if (!exists)
            {
                return NotFound();
            }

            // Retrieve the user object
            var user = await _identityService.GetUserByIdAsync(id);

            // Return an OK containing the user data
            return Ok(new UserDataResponse(user));
        }

        /// <summary>
        /// Retrieves user data
        /// </summary>
        /// <param name="email">The email address of the user to retrieve</param>
        /// <returns>Ok, NotFound, or BadRequest</returns>
        [HttpGet(Contracts.Routes.Administrator.Users.GetByEmail)]
        public async Task<IActionResult> GetUserByEmail([FromRoute]string email)
        {
            // Validate the email string
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email address is required");
            }

            // Verify a user exists with the given email address
            var exists = await _identityService.UserExistsWithEmailAsync(email);
            if (!exists)
            {
                return NotFound();
            }

            // Search for the user object with the given email address
            var user = await _identityService.GetUserByEmailAsync(email);

            // Return an OK with the user object
            return Ok(new UserDataResponse(user));
        }

        /// <summary>
        /// Retrieves a list of log summaries for all logs in the database
        /// </summary>
        /// <returns>IList of LogSummary</returns>
        [HttpGet(Contracts.Routes.Administrator.Logs.GetAll)]
        public async Task<IActionResult> GetAllLogs()
        {
            // Instantiate a list of log summaries
            var list = new List<LogSummaryResponse>();

            // Retrieve all logs in the database
            var logs = await _logsService.GetAllLogsAsync();
            
            // If no logs were returned or the list was null, 
            // return an Ok with an empty list of log summaries
            if (logs == null || logs.Count() == 0)
            {
                return Ok(list);
            }
            
            // Create a summary for each log in the list
            foreach (var log in logs)
            {
                var summary = new LogSummaryResponse(log);
                list.Add(summary);
            }

            // Return the list of log summaries
            return Ok(list);
        }

        /// <summary>
        /// Updates data about a LogFile already in the database
        /// </summary>
        /// <param name="request">LogUpdateRequest containing updated information</param>
        /// <returns>Ok, NotFound, or BadRequest</returns>
        [HttpPut(Contracts.Routes.Administrator.Logs.Update)]
        public async Task<IActionResult> UpdateLog([FromRoute]string id, [FromBody]LogUpdateRequest request)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify a log exists with the given ID, return a NotFound if none exist
            var exists = await _logsService.LogExists(id);
            if (!exists)
            {
                return NotFound();
            }

            // Update the log data
            try
            {
                await _logsService.UpdateAsync(id, request);
            }
            catch
            {
                // If the update fails, return a 500 status code
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // If everything succeeds, return an Ok
            return Ok();
        }

        /// <summary>
        /// Deletes a log from the database
        /// </summary>
        /// <param name="id">The ID of the log to delete</param>
        /// <returns>NoContent, NotFound, or BadRequest</returns>
        [HttpDelete(Contracts.Routes.Administrator.Logs.Delete)]
        public async Task<IActionResult> DeleteLog([FromRoute]string id)
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID is required");
            }

            // Verify a log exists with the given ID, return a NotFound if none exist
            var exists = await _logsService.LogExists(id);
            if (!exists)
            {
                return NotFound();
            }

            // Delete the log
            try
            {
                await _logsService.DeleteAsync(id);
            }
            catch (Exception)
            {
                // If the delete fails, return a 500 status code
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // If everything succeeds, return a NoContent
            return NoContent();
        }
    }
}