using System;
using System.Linq;
using System.Threading.Tasks;
using API.Contracts;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V2
{
    [ApiController]
    [Authorize]
    [Route(Contracts.Routes.ControllerV2)]
    [Route(Contracts.Routes.ControllerV1)]
    public class LogsController : ControllerBase
    {
        private ILogsService _logsService;
        private IFileService _fileService;

        public LogsController(ILogsService logs, IFileService files)
        {
            _logsService = logs;
            _fileService = files;
        }

        /// <summary>
        /// Retrieves a LogFile by its ID
        /// </summary>
        /// <param name="id">A string containing the ID of the log to return</param>
        /// <returns>LogFile or NotFound</returns>
        [AllowAnonymous]
        [HttpGet(Contracts.Routes.Logs.V2.GetById)]
        // [Route(Contracts.Routes.Logs.V2.GetById)]
        public async Task<IActionResult> GetById(string id)
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID is required");
            }

            // Verify that a log exists with the given ID
            var exists = await _logsService.LogExists(id);
            if (!exists)
            {
                // If a log doesn't exist, return NotFound
                return NotFound();
            }

            // Retrieve and return the log data
            var log = await _logsService.GetByIdAsync(id);
            return Ok(log);
        }

        /// <summary>
        /// Returns a new LogFile object parsed from a file uploaded through a form
        /// </summary>
        /// <param name="formFile">The uploaded log file</param>
        /// <returns>A new LogFile object or BadRequest</returns>
        [AllowAnonymous]
        [HttpPost(Contracts.Routes.Logs.V2.Upload)]
        public async Task<IActionResult> Upload([FromForm]IFormFile formFile)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Read the form file data
            string data = string.Empty;
            try
            {
                data = await _fileService.ReadFormFileAsync(formFile);
            }
            catch
            {
                // If reading the form file fails, return a server error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // Parse the data into a LogFile
            LogFile log = null;
            try
            {
                // Launch the parse on a background thread
                await Task.Run(() => {
                    log = _logsService.Parse(data);
                });
            }
            catch
            {
                // If the parse failed, return a server error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // If everything succeeds, return the parsed log data
            return Ok(log);
        }

        /// <summary>
        /// Saves a parsed LogFile object to the database
        /// </summary>
        /// <param name="save">The LogFile object to save</param>
        /// <returns>The same LogFile object, updated with a unique ID</returns>
        [HttpPost(Contracts.Routes.Logs.V2.Save)]
        public async Task<IActionResult> Save([FromBody]LogFile save, [FromServices]ITokenService _tokenService, [FromHeader(Name = "Authorization")]string jwt)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if this log already has an ID and, if it does, verify that
            // it does not conflict with any existing logs
            if (!string.IsNullOrEmpty(save.Id))
            {
                var exists = await _logsService.LogExists(save.Id);
                if (exists)
                {
                    return Conflict("A log already exists with that ID");
                }
            }

            // Save the log data to the store
            LogFile savedLog = null;
            var userId = _tokenService.GetUserId(jwt.Substring("Bearer ".Length));
            save.OwnerId = userId;
            try
            {
                savedLog = await _logsService.CreateAsync(save);
            }
            catch
            {
                // If the save fails, return a server error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Created(savedLog.Id, savedLog);
        }

        /// <summary>
        /// Overwrites an existing LogFile in the database with the new/updated information
        /// </summary>
        /// <param name="update">The LogFile object to save to the database</param>
        /// <returns>OK, BadRequest, or Conflict</returns>
        [Authorize(Policy = Contracts.Policies.ResourceOwnerPolicy)]
        [HttpPut(Contracts.Routes.Logs.V2.Update)]
        public async Task<IActionResult> UpdateLog([FromBody]LogUpdateRequest update)
        {
            // Validate the ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify a log exists with the given ID
            var exists = await _logsService.LogExists(update.Id);
            if (!exists)
            {
                // If no existing log was found, return NotFound
                return NotFound();
            }

            // Retrieve the log from the store
            var log = await _logsService.GetByIdAsync(update.Id);

            // Map the updated log info <-- This should be moved into the logs service
            if (!string.IsNullOrEmpty(update.Comments))
            {
                log.Comments = update.Comments;
            }

            if (!string.IsNullOrEmpty(update.OwnerId))
            {
                log.OwnerId = update.OwnerId;
            }

            if (!string.IsNullOrEmpty(update.Title))
            {
                log.Title = update.Title;
            }

            // Update the log data
            try
            {
                await _logsService.UpdateAsync(log);
            }
            catch
            {
                // If the update fails, return a server error
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // If everything succeeds, return OK
            return Ok();
        }

        /// <summary>
        /// Deletes an existing log from the database
        /// </summary>
        /// <param name="id">The ID of the log to delete</param>
        /// <returns>NoContent, NotFound, or BadRequest</returns>
        [Authorize(Policy = Contracts.Policies.ResourceOwnerPolicy)]
        [HttpDelete(Contracts.Routes.Logs.V2.Delete)]
        public async Task<IActionResult> DeleteLog(string id)
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID is required");
            }

            // Verify a log exists with the given ID
            var exists = await _logsService.LogExists(id);
            if (!exists)
            {
                return NotFound();
            }

            // Delete the log from the store
            try
            {
                await _logsService.DeleteAsync(id);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // If everything succeeds, return NoContent
            return NoContent();
        }
    }
}