using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTonyWilly.Data;
using WebTonyWilly.Dtos;
using WebTonyWilly.models;

namespace WebTonyWilly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnosController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TurnosController(MyDbContext context)
        {
            _context = context;
        }

        // ---------------------------
        // 1️⃣ ABRIR TURNO
        // ---------------------------
        [HttpPost("abrir")]
        public async Task<IActionResult> AbrirTurno([FromBody] TurnosCreateDto dto)
        {
            Console.WriteLine("UsuarioId recibido: " + dto.UsuarioId);
            Console.WriteLine("FondoInicial recibido: " + dto.FondoInicial);

            bool abierto = await _context.Turnos.AnyAsync(t =>
                t.UsuarioId == dto.UsuarioId && !t.EstaCerrado);

            if (abierto)
                return BadRequest("Ya tenés un turno abierto.");

            var turno = new Turno
            {
                UsuarioId = dto.UsuarioId,
                FondoInicial = dto.FondoInicial,
                Inicio = DateTime.UtcNow,
                EstaCerrado = false,

                // 🔥 INICIALIZACIÓN NECESARIA 🔥
                TotalEfectivo = 0,
                TotalMP = 0,
                EfectivoFinal = 0,
                EfectivoEsperado = dto.FondoInicial,
                Diferencia = 0
            };

            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();

            return Ok(turno);
        }
        //  CERRAR TURNO (FINAL VERSION)
        // ---------------------------
        [HttpPost("cerrar/{turnoId}")]
        public async Task<IActionResult> CerrarTurno(int turnoId, [FromBody] decimal efectivoFinal)
        {
            var turno = await _context.Turnos.FindAsync(turnoId);

            if (turno == null)
                return NotFound("Turno no encontrado.");

            if (turno.EstaCerrado)
                return BadRequest("Este turno ya está cerrado.");

            turno.Cierre = DateTime.UtcNow;

            // 🔥 Traer ventas reales del usuario en el rango del turno
            var ventas = await _context.Ventas
                .Where(v =>
                    v.UsuarioId == turno.UsuarioId &&
                    v.Fecha >= turno.Inicio &&
                    v.Fecha <= turno.Cierre &&
                    v.Estado != "cancelada"
                )
                .ToListAsync();

            // Total efectivo
            var ventasEfectivo = ventas
                .Where(v => v.MetodoPago.ToLower() == "efectivo")
                .Sum(v => v.Total);

            // Total MercadoPago (por si querés mostrarlo)
            var ventasMP = ventas
                .Where(v => v.MetodoPago.ToLower() == "mercadopago")
                .Sum(v => v.Total);

            turno.TotalEfectivo = ventasEfectivo;
            turno.TotalMP = ventasMP;

            // Calcular efectivo esperado
            turno.EfectivoEsperado = turno.FondoInicial + ventasEfectivo;

            // Guardar lo que contó el cajero
            turno.EfectivoFinal = efectivoFinal;

            // Diferencia
            turno.Diferencia = efectivoFinal - turno.EfectivoEsperado;

            turno.EstaCerrado = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Turno cerrado correctamente",
                turno
            });
        }

        // ---------------------------
        // 3️⃣ OBTENER TURNO ABIERTO DEL USUARIO
        // ---------------------------
        [HttpGet("abierto/{usuarioId}")]
        public async Task<IActionResult> GetTurnoAbierto(int usuarioId)
        {
            var turno = await _context.Turnos
                .FirstOrDefaultAsync(t => t.UsuarioId == usuarioId && !t.EstaCerrado);

            if (turno == null)
                return Ok(null);

            return Ok(turno);
        }
        // 4️⃣ LISTAR TODOS LOS TURNOS
        // ---------------------------
        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var turnos = await _context.Turnos
                .Include(t => t.Usuario) // para traer nombre del usuario
                .ToListAsync();

            return Ok(turnos);
        }
        //   GET DETALLE COMPLETO DEL TURNO
        // ---------------------------------------------
        [HttpGet("detalle/{turnoId}")]
        public async Task<IActionResult> GetDetalleTurno(int turnoId)
        {
            var turno = await _context.Turnos
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.Id == turnoId);

            if (turno == null)
                return NotFound("Turno no encontrado.");

            // Traer ventas dentro del rango
            var ventas = await _context.Ventas
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Where(v =>
                    v.UsuarioId == turno.UsuarioId &&
                    v.Fecha >= turno.Inicio &&
                    (turno.Cierre == null || v.Fecha <= turno.Cierre) &&
                    v.Estado != "cancelada"
                )
                .ToListAsync();

            // Totales por método de pago
            var totalEfectivo = ventas
                .Where(v => v.MetodoPago.ToLower() == "efectivo")
                .Sum(v => v.Total);

            var totalMp = ventas
                .Where(v => v.MetodoPago.ToLower() == "mercadopago")
                .Sum(v => v.Total);

            var totalTarjeta = ventas
                .Where(v => v.MetodoPago.ToLower() == "tarjeta")
                .Sum(v => v.Total);

            // Efectivo esperado
            decimal efectivoEsperado = turno.FondoInicial + totalEfectivo;

            return Ok(new
            {
                turno.Id,
                Cajero = turno.Usuario?.Nombre,
                Inicio = turno.Inicio,
                Fin = turno.Cierre,
                turno.FondoInicial,
                TotalEfectivo = totalEfectivo,
                TotalMP = totalMp,
                TotalTarjeta = totalTarjeta,
                EfectivoEsperado = efectivoEsperado,
                turno.EfectivoFinal,
                turno.Diferencia,
                Ventas = ventas.Select(v => new {
                    v.Id,
                    v.Total,
                    v.MetodoPago,
                    v.Fecha,
                    Items = v.Detalles.Select(d => new {
                        d.Producto.Nombre,
                        d.Cantidad,
                        Precio = d.PrecioUnitario,
                        Subtotal = d.Cantidad * d.PrecioUnitario
                    })

                })
            });
        }
    }
}

