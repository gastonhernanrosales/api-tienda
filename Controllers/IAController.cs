using Microsoft.AspNetCore.Mvc;
using WebTonyWilly.Services;

namespace WebTonyWilly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IAController : ControllerBase
    {
        private readonly OpenAIService _openAIService;

        public IAController(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] string mensaje)
        {
            var respuesta = await _openAIService.PreguntarIA(mensaje);

            return Ok(new
            {
                respuesta
            });
        }
    }
}
