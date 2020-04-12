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
    }
}