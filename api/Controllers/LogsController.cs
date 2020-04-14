using System;
using System.Threading.Tasks;
using API.Contracts;
using API.Contracts.Requests;
using API.Models;
using API.Services;
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
        public IActionResult Get()
        {
            return Ok("Hello world!");
        }

        [HttpGet]
        [Route(Routes.Logs.GetById)]
        public Task<IActionResult> GetById(string id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route(Routes.Logs.GetForUser)]
        public Task<IActionResult> GetForUser(string userId)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route(Routes.Logs.Update)]
        public Task<IActionResult> Update(string id, [FromBody]ILogUpdateRequest update)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route(Routes.Logs.Delete)]
        public Task<IActionResult> Delete(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route(Routes.Logs.Save)]
        public Task<IActionResult> Save([FromBody]ILogFile log)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route(Routes.Logs.Upload)]
        public IActionResult Upload()
        {
            throw new NotImplementedException();
        }
    }
}