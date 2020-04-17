using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using API.Contracts;
using API.Contracts.Requests;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route(Routes.Controller)]
    public class LogsController : ControllerBase
    {
        private readonly ILogsService _logsService;
        public LogsController(ILogsService service)
        {
            _logsService = service;
        }

        [HttpGet]
        [Route(Routes.Logs.Get)]
        public async Task<IActionResult> Get()
        {
            var list = await _logsService.GetAllLogsAsync();
            return Ok(list);
        }

        [HttpGet]
        [Route(Routes.Logs.GetById)]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var log = await _logsService.GetByIdAsync(id);

            if (log == null)
            {
                return NotFound();
            }

            return Ok(log);
        }

        [HttpGet]
        [Route(Routes.Logs.GetForUser)]
        public async Task<IActionResult> GetForUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest();
            }

            var logs = await _logsService.GetForUserAsync(userId);

            if (logs == null)
            {
                return NotFound();
            }

            return Ok(logs);
        }

        [HttpPut]
        [Route(Routes.Logs.Update)]
        public async Task<IActionResult> Update(string id, [FromBody]LogUpdateRequest update)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            if (update == null)
            {
                return BadRequest();
            }

            var result = await _logsService.UpdateAsync(update.Log);
            return Ok(result);
        }

        [HttpDelete]
        [Route(Routes.Logs.Delete)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var exists = await _logsService.GetByIdAsync(id) != null;

            if (!exists)
            {
                return NotFound();
            }

            await _logsService.DeleteAsync(id);

            return Ok();
        }

        [HttpPost]
        [Route(Routes.Logs.Save)]
        public async Task<IActionResult> Save([FromBody]LogSaveRequest save)
        {
            if (save == null || save.Log == null)
            {
                return BadRequest("Log data cannot be null");
            }

            var log = save.Log;

            if (!string.IsNullOrWhiteSpace(log.Id))
            {
                var exists = await _logsService.GetByIdAsync(log.Id) != null;

                if (exists)
                {
                    return Conflict();
                }
            }

            var result = await _logsService.CreateAsync(log);

            if (result == null)
            {
                return StatusCode(500);
            }

            return Created(result.Id, result);
        }

        private string ReadFileData(Stream fileStream)
        {
            byte[] data = new byte[fileStream.Length];

            fileStream.Read(data, 0, (int)fileStream.Length);

            var str = Encoding.UTF8.GetString(data, 0, (int)fileStream.Length);

            return str;
        }

        [HttpPost]
        [Route(Routes.Logs.Upload)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload([FromForm]IFormFile file)
        {
            if (file == null)
            {
                return BadRequest();
            }

            string data = string.Empty;

            using (var stream = file.OpenReadStream())
            {
                await Task.Run(() => {
                    data = ReadFileData(stream);
                });
            }

            ILogFile log = null;

            try
            {
                await Task.Run(() => {
                    log = _logsService.Parse(data);
                });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            if (log == null)
            {
                return StatusCode(500);
            }

            return Ok(log);
        }
    }
}