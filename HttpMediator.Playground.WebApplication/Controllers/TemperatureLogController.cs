using HttpMediator.Playground.WebApp;
using Microsoft.AspNetCore.Mvc;

namespace HttpMediator.Playground.WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemperatureLogController : ControllerBase
    {

        [HttpGet]
        public string Get() => string.Join(",", InMemoryDatabase.Database["temperature_log"]);
    }
}