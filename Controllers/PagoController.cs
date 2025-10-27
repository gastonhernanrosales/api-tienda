using Microsoft.AspNetCore.Mvc;
using WebTonyWilly.Data;
using WebTonyWilly.Dtos;
using WebTonyWilly.models;

[ApiController]
[Route("api/[controller]")]
public class PagoController : ControllerBase
{
    private readonly MyDbContext _context;

    public PagoController(MyDbContext context)
    {
        _context = context;
    }

    [HttpPost("CrearPago")]
    public async Task<IActionResult> CrearPago([FromBody] PagoRequest request)
    {
        if (request == null) return BadRequest("Datos de pago inválidos");

        var pago = new Pago
        {
            Monto = request.Monto,
            Metodo = request.Metodo,
            Email = request.Email,
            Estado = request.Metodo == "efectivo" ? "completado" : "pendiente",
            Fecha = DateTime.UtcNow
        };

        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();

        string qrUrl = request.Metodo == "qr" ? $"https://fake-qr-url.com/{pago.Id}" : null;

        return Ok(new { pagoId = pago.Id, qrUrl });
    }

    [HttpGet("Estado/{id}")]
    public async Task<IActionResult> Estado(int id)
    {
        var pago = await _context.Pagos.FindAsync(id);
        if (pago == null) return NotFound();
        return Ok(new { status = pago.Estado });
    }
}


