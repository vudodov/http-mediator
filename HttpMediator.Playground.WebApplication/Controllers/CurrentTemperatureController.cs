using HttpMediator.Playground.WebApp;
using Microsoft.AspNetCore.Mvc;

namespace HttpMediator.Playground.WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrentTemperatureController : ControllerBase
    {

        [HttpGet]
        public string Get() => InMemoryDatabase.Database["human_friendly"];
    }
}