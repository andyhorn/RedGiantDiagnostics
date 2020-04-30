using System;
using System.Threading.Tasks;
using API.Contracts;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V2
{
    [ApiController, Route(Contracts.Routes.ControllerV2)]
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="request">UserUpdateRequest containing updated information</param>
        /// <returns>Ok or BadRequest</returns>
        [HttpPut, Route(Contracts.Routes.Administrator.Users.Update)]
        public async Task<IActionResult> UpdateUser([FromBody]UserUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a user from the identity store
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>NoContent, NotFound, or BadRequest</returns>
        [HttpDelete, Route(Contracts.Routes.Administrator.Users.Delete)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves user data
        /// </summary>
        /// <param name="id">The ID of the user to retrieve</param>
        /// <returns>Ok, NotFound, or BadRequest</returns>
        [HttpGet, Route(Contracts.Routes.Administrator.Users.GetById)]
        public async Task<IActionResult> GetUserById(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves user data
        /// </summary>
        /// <param name="email">The email address of the user to retrieve</param>
        /// <returns>Ok, NotFound, or BadRequest</returns>
        [HttpGet, Route(Contracts.Routes.Administrator.Users.GetByEmail)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates data about a LogFile already in the database
        /// </summary>
        /// <param name="request">LogUpdateRequest containing updated information</param>
        /// <returns>Ok, NotFound, or BadRequest</returns>
        [HttpPut, Route(Contracts.Routes.Administrator.Logs.Update)]
        public async Task<IActionResult> UpdateLog([FromBody]LogUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a log from the database
        /// </summary>
        /// <param name="id">The ID of the log to delete</param>
        /// <returns>NoContent, NotFound, or BadRequest</returns>
        [HttpDelete, Route(Contracts.Routes.Administrator.Logs.Delete)]
        public async Task<IActionResult> DeleteLog(string id)
        {
            throw new NotImplementedException();
        }
    }
}