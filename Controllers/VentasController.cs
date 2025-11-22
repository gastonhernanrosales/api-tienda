using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTonyWilly.Data;
using WebTonyWilly.Dtos;
using WebTonyWilly.models;

namespace WebTonyWilly.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly MyDbContext _context;

        public VentasController(MyDbContext context)
        {
            _context = context;
        }

        // ✅ Registrar una venta (solo si el pago fue confirmado)
        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarVenta([FromBody] VentaRequest dto)
        {
            try
            {
                // 1️⃣ Verificar que el pago exista y esté aprobado
                var pago = await _context.Pagos.FirstOrDefaultAsync(p => p.Id == dto.PagoId);

                if (pago == null)
                    return BadRequest("No se encontró el pago asociado.");

                // 🔹 Chequear Status del pago (guardado en DB)
                if (pago.Estado != "approved" && pago.Estado != "completado")
                    return BadRequest("El pago no fue confirmado. No se puede registrar la venta.");

                // 2️⃣ Crear la venta
                var venta = new Venta
                {
                    Fecha = DateTime.UtcNow,
                    Total = dto.Total,
                    MetodoPago = pago.Metodo,
                    PagoId = pago.Id
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                // 3️⃣ Registrar detalle de la venta y actualizar stock
                foreach (var item in dto.Detalles)
                {
                    var detalle = new VentaDetalle
                    {
                        VentaId = venta.Id,
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario
                    };

                    _context.VentaDetalles.Add(detalle);

                    // Actualizar stock
                    var producto = await _context.Producto.FindAsync(item.ProductoId);
                    if (producto != null)
                    {
                        producto.Stock -= item.Cantidad;
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Venta registrada exitosamente",
                    ventaId = venta.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar la venta: {ex.Message}");
            }
        }

        // ✅ Obtener todas las ventas
        [HttpGet]
        public async Task<IActionResult> GetVentas()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .Include(v => v.Usuario) // ✅ Esto incluye el cajero que hizo la venta
                .ToListAsync();

            return Ok(ventas);
        }
        [HttpPost("Anular/{id}")]
        [Authorize(Roles = "admin")] // Solo admins pueden anular
        public async Task<IActionResult> AnularVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound("Venta no encontrada.");

            if (venta.Estado == "cancelada")
                return BadRequest("La venta ya está anulada.");

            // Marcar venta como cancelada
            venta.Estado = "cancelada";

            // Restaurar stock de productos
            foreach (var detalle in venta.Detalles)
            {
                var producto = detalle.Producto;
                if (producto != null)
                {
                    producto.Stock += detalle.Cantidad;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Venta anulada correctamente." });
        }
        //obtener venta de un usuario
        [HttpGet("GetByUser/{userId}")]
        public async Task<IActionResult> GetVentasByUser(int userId)
        {
            var ventas = await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(v => v.UsuarioId == userId)
                .ToListAsync();

            if (!ventas.Any())
                return NotFound("No se encontraron ventas para este usuario.");

            return Ok(ventas);
        }

        // ✅ Obtener una venta por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound("Venta no encontrada");

            return Ok(venta);
        }
        [HttpPost("ConfirmarPago")]
        [Authorize]
        public async Task<IActionResult> ConfirmarPago([FromBody] ConfirmarVentaDto dto)
        {
            try
            {
                // 1️⃣ Buscar el pago
                var pago = await _context.Pagos.FindAsync(dto.PagoId);
                if (pago == null)
                    return BadRequest(new { error = "Pago no encontrado." });

                // 2️⃣ Verificar que el pago esté aprobado
                if (pago.Estado != "approved" && pago.Estado != "completado")
                    return BadRequest(new { error = "El pago aún no fue aprobado." });

                // 3️⃣ Revisar si ya se registró la venta
                if (await _context.Ventas.AnyAsync(v => v.PagoId == dto.PagoId))
                    return BadRequest(new { error = "La venta ya fue registrada." });

                // 4️⃣ Crear la venta
                var venta = new Venta
                {
                    Fecha = DateTime.UtcNow,
                    Total = dto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario),
                    MetodoPago = dto.MetodoPago,
                    PagoId = pago.Id,
                    UsuarioId = dto.UsuarioId
                };
                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                // 5️⃣ Registrar detalles y actualizar stock
                foreach (var item in dto.Detalles)
                {
                    var producto = await _context.Producto.FindAsync(item.ProductoId);
                    if (producto == null)
                        return BadRequest(new { error = $"Producto con ID {item.ProductoId} no existe." });

                    // ⚠️ Validar stock suficiente
                    if (producto.Stock < item.Cantidad)
                        return BadRequest(new
                        {
                            error = $"Stock insuficiente para el producto '{producto.Nombre}'. " +
                                    $"Stock actual: {producto.Stock}, solicitado: {item.Cantidad}."
                        });

                    // ✅ Crear detalle
                    var detalle = new VentaDetalle
                    {
                        VentaId = venta.Id,
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario
                    };
                    _context.VentaDetalles.Add(detalle);

                    // ✅ Descontar stock
                    producto.Stock -= item.Cantidad;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Pago confirmado y venta registrada exitosamente",
                    ventaId = venta.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        // 🔥 Obtener ventas de un usuario dentro de un turno
        [HttpGet("GetByUserAndTurno")]
        public async Task<IActionResult> GetVentasByUserAndTurno(
            int userId,
            DateTime desde,
            DateTime hasta)
        {
            var ventas = await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .Where(v => v.UsuarioId == userId &&
                            v.Fecha >= desde &&
                            v.Fecha <= hasta)
                .ToListAsync();

            return Ok(ventas);
        }

    }
}