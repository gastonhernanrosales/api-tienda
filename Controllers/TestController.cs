using Microsoft.AspNetCore.Mvc;

namespace WebTonyWilly.Controllers
{
    public class TestController: ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("API funcionando");
    }
}
