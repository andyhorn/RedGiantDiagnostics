using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Contracts;
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

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="request">UserRegistrationRequest containing new user data</param>
        /// <returns>Created with the new user's ID or BadRequest</returns>
        [HttpPost, Route(Contracts.Routes.Administrator.Users.Register)]
        public async Task<IActionResult> RegisterUser([FromBody]UserRegistrationRequest request)
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
            IdentityUser user = null;
            try
            {
                user = await _identityService.CreateUserAsync(request);
            }
            catch (ActionFailedException)
            {
                return StatusCode(500);
            }

            // Verify the user object was created
            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Created(user.Id, user);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="request">UserUpdateRequest containing updated information</param>
        /// <returns>Ok or BadRequest</returns>
        [HttpPut, Route(Contracts.Routes.Administrator.Users.Update)]
        public async Task<IActionResult> UpdateUser([FromBody]UserUpdateRequest request)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify a user exists with the given ID
            var exists = await _identityService.UserExistsWithIdAsync(request.Id);
            if (!exists)
            {
                return NotFound();
            }

            // Update the user object
            try
            {
                await _identityService.UpdateUserAsync(request);
            }
            catch (ActionFailedException)
            {
                // If the update failed, return a 500 status code
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // If everything succeeds, return an OK
            return Ok();
        }

        /// <summary>
        /// Delete a user from the identity store
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>NoContent, NotFound, or BadRequest</returns>
        [HttpDelete, Route(Contracts.Routes.Administrator.Users.Delete)]
        public async Task<IActionResult> DeleteUser(string id)
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

        /// <summary>
        /// Retrieves user data
        /// </summary>
        /// <param name="id">The ID of the user to retrieve</param>
        /// <returns>Ok, NotFound, or BadRequest</returns>
        [HttpGet, Route(Contracts.Routes.Administrator.Users.GetById)]
        public async Task<IActionResult> GetUserById(string id)
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
            return Ok(user);
        }

        /// <summary>
        /// Retrieves user data
        /// </summary>
        /// <param name="email">The email address of the user to retrieve</param>
        /// <returns>Ok, NotFound, or BadRequest</returns>
        [HttpGet, Route(Contracts.Routes.Administrator.Users.GetByEmail)]
        public async Task<IActionResult> GetUserByEmail(string email)
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
            return Ok(user);
        }

        [HttpGet]
        [Route(Contracts.Routes.Administrator.Logs.GetAll)]
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
        [HttpPut, Route(Contracts.Routes.Administrator.Logs.Update)]
        public async Task<IActionResult> UpdateLog([FromBody]LogUpdateRequest request)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify a log exists with the given ID, return a NotFound if none exist
            var exists = await _logsService.LogExists(request.Id);
            if (!exists)
            {
                return NotFound();
            }

            // Retrieve the log
            var log = await _logsService.GetByIdAsync(request.Id);

            // Map the updated data
            if (!string.IsNullOrEmpty(request.OwnerId))
            {
                log.OwnerId = request.OwnerId;
            }

            if (!string.IsNullOrEmpty(request.Title))
            {
                log.Title = request.Title;
            }

            if (!string.IsNullOrEmpty(request.Comments))
            {
                log.Comments = request.Comments;
            }

            // Update the log data
            try
            {
                await _logsService.UpdateAsync(log);
            }
            catch (Exception)
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
        [HttpDelete, Route(Contracts.Routes.Administrator.Logs.Delete)]
        public async Task<IActionResult> DeleteLog(string id)
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