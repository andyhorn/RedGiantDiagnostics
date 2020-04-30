using System;
using System.Threading.Tasks;
using API.Contracts;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V2
{
    [ApiController, Route(Contracts.Routes.ControllerV2)]
    public class LogsControllerV2 : ControllerBase
    {
        private ILogsService _logsService;

        public LogsControllerV2(ILogsService logs)
        {
            _logsService = logs;
        }

        /// <summary>
        /// Retrieves a LogFile by its ID
        /// </summary>
        /// <param name="id">A string containing the ID of the log to return</param>
        /// <returns>LogFile or NotFound</returns>
        [HttpGet, Route(Contracts.Routes.Logs.V2.GetById)]
        public async Task<IActionResult> GetById(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a new LogFile object parsed from a file uploaded through a form
        /// </summary>
        /// <param name="formFile">The uploaded log file</param>
        /// <returns>A new LogFile object or BadRequest</returns>
        [HttpPost, Route(Contracts.Routes.Logs.V2.Upload)]
        public async Task<IActionResult> Upload([FromForm]IFormFile formFile)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a parsed LogFile object to the database, attached to the current
        /// user as the owner
        /// </summary>
        /// <param name="save">The LogFile object to save</param>
        /// <returns>The same LogFile object, updated with an ID and OwnerId</returns>
        [HttpPost, Route(Contracts.Routes.Logs.V2.Save)]
        public async Task<IActionResult> Save([FromBody]LogFile save)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Overwrites an existing LogFile in the database with the new/updated information
        /// </summary>
        /// <param name="update">The LogFile object to save to the database</param>
        /// <returns>OK, BadRequest, or Conflict</returns>
        [HttpPut, Route(Contracts.Routes.Logs.V2.Update)]
        public async Task<IActionResult> UpdateLog([FromBody]LogUpdateRequest update)
        {
            throw new NotImplementedException();
        }

        [HttpDelete, Route(Contracts.Routes.Logs.V2.Delete)]
        public async Task<IActionResult> DeleteLog(string id)
        {
            throw new NotImplementedException();
        }
    }
}