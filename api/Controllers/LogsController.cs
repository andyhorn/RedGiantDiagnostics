// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using API.Contracts;
// using API.Entities;
// using API.Models;
// using API.Services;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Newtonsoft.Json;

// namespace API.Controllers
// {
//     [ApiController]
//     [Route(Routes.ControllerV1)]
//     public class LogsController : ControllerBase
//     {
//         private readonly ILogsService _logsService;
//         private readonly IFileService _fileService;
//         public LogsController(ILogsService logsService, IFileService fileService)
//         {
//             _logsService = logsService;
//             _fileService = fileService;
//         }

//         [HttpGet]
//         [Route(Routes.Logs.Get)]
//         public async Task<IActionResult> Get()
//         {
//             var list = await _logsService.GetAllLogsAsync() ?? new List<LogFile>();
//             var logSummaries = list.Select(x => new LogSummaryResponse(x));
//             return Ok(logSummaries);
//         }

//         [HttpGet]
//         [Route(Routes.Logs.GetById)]
//         public async Task<IActionResult> GetById(string id)
//         {
//             if (string.IsNullOrWhiteSpace(id))
//             {
//                 return BadRequest();
//             }

//             var log = await _logsService.GetByIdAsync(id);

//             if (log == null)
//             {
//                 return NotFound();
//             }

//             return Ok(log);
//         }

//         [HttpGet]
//         [Route(Routes.Logs.GetForUser)]
//         public async Task<IActionResult> GetForUser(string userId)
//         {
//             if (string.IsNullOrWhiteSpace(userId))
//             {
//                 return BadRequest();
//             }

//             var logs = await _logsService.GetForUserAsync(userId);

//             if (logs == null)
//             {
//                 return NotFound();
//             }

//             var summaries = logs.Select(x => new LogSummaryResponse(x));

//             return Ok(summaries);
//         }

//         [HttpPut]
//         [Route(Routes.Logs.Update)]
//         public async Task<IActionResult> Update(string id, [FromBody]LogFile update)
//         {
//             if (string.IsNullOrWhiteSpace(id))
//             {
//                 return BadRequest();
//             }

//             if (update == null)
//             {
//                 return BadRequest();
//             }

//             var result = await _logsService.UpdateAsync(update);
//             return Ok(result);
//         }

//         [HttpDelete]
//         [Route(Routes.Logs.Delete)]
//         public async Task<IActionResult> Delete(string id)
//         {
//             if (string.IsNullOrWhiteSpace(id))
//             {
//                 return BadRequest();
//             }

//             var exists = await _logsService.GetByIdAsync(id) != null;

//             if (!exists)
//             {
//                 return NotFound();
//             }

//             await _logsService.DeleteAsync(id);

//             return Ok();
//         }

//         [HttpPost]
//         [Route(Routes.Logs.Save)]
//         public async Task<IActionResult> Save([FromBody]LogFile log)
//         {
//             if (log == null)
//             {
//                 return BadRequest();
//             }

//             if (!string.IsNullOrWhiteSpace(log.Id))
//             {
//                 var exists = await _logsService.GetByIdAsync(log.Id) != null;

//                 if (exists)
//                 {
//                     return Conflict();
//                 }
//             }

//             var result = await _logsService.CreateAsync(log);

//             if (result == null)
//             {
//                 return StatusCode(500);
//             }

//             return Created(result.Id, new LogSummaryResponse(result));
//         }

//         [HttpPost]
//         [Route(Routes.Logs.Upload)]
//         [DisableRequestSizeLimit]
//         public async Task<IActionResult> Upload([FromForm]IFormFile file)
//         {
//             try
//             {
//                 var log = await ParseFormData(file);

//                 var data = JsonConvert.SerializeObject(log);
//                 return Ok(data);
//             }
//             catch (FileNotFoundException)
//             {
//                 return BadRequest("No file was uploaded");
//             }
//             catch (ArgumentNullException)
//             {
//                 return StatusCode(500, "Unable to parse form data");
//             }
//             catch (Exception)
//             {
//                 return StatusCode(500, "An unknown error occurred");
//             }
//         }

//         private async Task<LogFile> ParseFormData(IFormFile form)
//         {
//             if (form == null)
//             {
//                 throw new FileNotFoundException();
//             }
            
//             var data = await _fileService.ReadFormFileAsync(form);

//             if (data == null)
//             {
//                 throw new ArgumentNullException();
//             }

//             LogFile log = null;
//             await Task.Run(() => {
//                 log = _logsService.Parse(data);
//             });

//             return log;
//         }
//     }
// }