using System;
using API.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route(Routes.Controller)]
    public class LogsController : ControllerBase
    {
        [HttpGet]
        [Route(Routes.Logs.Get)]
        public IActionResult Get()
        {
            return Ok("Hello world!");
        }

        [HttpGet]
        [Route(Routes.Logs.GetById)]
        public IActionResult GetById(string id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route(Routes.Logs.GetForUser)]
        public IActionResult GetForUser(string userId)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route(Routes.Logs.Update)]
        public IActionResult Update(string id)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route(Routes.Logs.Delete)]
        public IActionResult Delete(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route(Routes.Logs.Save)]
        public IActionResult Save()
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