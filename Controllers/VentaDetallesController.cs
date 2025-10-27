using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTonyWilly.Data;
using WebTonyWilly.models;

namespace WebTonyWilly.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentaDetallesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public VentaDetallesController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDetalle>>> GetDetalleVentas()
        {
            return await _context.VentaDetalles
                .Include(dv => dv.Producto)
                .Include(dv => dv.Venta)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetalleVenta(int id)
        {
            var detalle = await _context.VentaDetalles
                .Include(dv => dv.Producto)
                .Include(dv => dv.Venta)
                .FirstOrDefaultAsync(dv => dv.Id == id);

            if (detalle == null) return NotFound();
            return Ok(detalle);
        }

        [HttpPost]
        public async Task<ActionResult<VentaDetalle>> PostDetalleVenta(VentaDetalle detalle)
        {
            _context.VentaDetalles.Add(detalle);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDetalleVenta), new { id = detalle.Id }, detalle);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetalleVenta(int id, VentaDetalle detalle)
        {
            if (id != detalle.Id) return BadRequest();
            _context.Entry(detalle).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetalleVenta(int id)
        {
            var detalle = await _context.VentaDetalles.FindAsync(id);
            if (detalle == null) return NotFound();

            _context.VentaDetalles.Remove(detalle);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}