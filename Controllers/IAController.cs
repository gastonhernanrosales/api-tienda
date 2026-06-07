using Microsoft.AspNetCore.Mvc;
using WebTonyWilly.Services;

namespace WebTonyWilly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IAController : ControllerBase
    {

        
        private readonly GeminiService _geminiService;
        public IAController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] string mensaje)
        {
            var respuesta = await _geminiService.PreguntarIA(mensaje);

            return Ok(new
            {
                respuesta
            });
        }
    }
}
