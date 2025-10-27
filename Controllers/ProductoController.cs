using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTonyWilly.Data;
using WebTonyWilly.Dtos;
using WebTonyWilly.models;

namespace WebTonyWilly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ProductoController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Producto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Producto
              .Include(p => p.Categoria)
              .ToListAsync();
        }

        // GET: api/Producto/5
        [HttpGet("GetProducto/{id}")]
        public async Task<IActionResult> GetProducto(int id)
        {
            var producto = await _context.Producto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }


        // POST: api/Producto
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            
            // Aseguramos que solo se relacione con la categoría existente
            var categoria = await _context.Categoria.FindAsync(producto.CategoriaId);
            if (categoria == null)
                return BadRequest("Categoría no encontrada.");

            producto.Categoria = categoria; // 👈 Vincula la existente, no crea nueva
            _context.Producto.Add(producto);
            await _context.SaveChangesAsync();


            // 🔹 Recargamos el producto desde la DB con su categoría incluida
            var productoConCategoria = await _context.Producto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == producto.Id);

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, productoConCategoria);
        }

        // PUT: api/Producto/5
        [HttpPut("PutProducto/{id}")]
        public async Task<IActionResult> PutProducto(int id, ProductoUpdateDto producto)
        {
            var productoExistente = await _context.Producto
              .Include(p => p.Categoria) // 👈 Incluimos la categoría actual
              .FirstOrDefaultAsync(p => p.Id == id);

            if (productoExistente == null)
                return NotFound();

            // Actualizamos solo los campos permitidos
            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripcion = producto.Descripcion;
            productoExistente.Precio = producto.Precio;
            productoExistente.Stock = producto.Stock;
            productoExistente.ImageUrl = producto.ImageUrl;
            productoExistente.CodigoBarras = producto.CodigoBarras;
            if (producto.CategoriaId.HasValue)
            {
                var categoria = await _context.Categoria.FindAsync(producto.CategoriaId.Value);
                if (categoria == null)
                    return BadRequest("Categoría no encontrada.");

                productoExistente.Categoria = categoria;
                productoExistente.CategoriaId = producto.CategoriaId.Value;
            }
            await _context.SaveChangesAsync();
            
            // 🔹 Recargamos con la categoría incluida
            var actualizado = await _context.Producto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            // 🔹 Devolvemos el producto actualizado (con categoría incluida)
            return Ok(actualizado);
            
        }

        // DELETE: api/Producto/5
        [HttpDelete("DeleteProducto/{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Producto.FindAsync(id);
            if (producto == null) return NotFound();

            _context.Producto.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
