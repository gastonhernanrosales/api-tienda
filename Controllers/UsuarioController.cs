using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTonyWilly.Data;
using WebTonyWilly.Dtos;
using WebTonyWilly.models;

namespace WebTonyWilly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UsuarioController(MyDbContext context)
        {
            _context = context;
        }
        //obtener usuarios GET
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuario.ToListAsync();
        }
        //obtener usuarios por id GET

        [HttpGet("GetUsuario/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);

        }
        //crear un usuario POST 
        [HttpPost("createUser")]
        [Authorize(Roles = "admin")]
        public IActionResult CreateUser([FromBody] RegisterDto dto)
        {
            if (_context.Usuario.Any(u => u.Email == dto.Email))
                return BadRequest("Usuario ya existe");

            var usuario = new Usuario
            {
                Nombre = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = dto.Role ?? "Cajero" // Admin puede decidir rol
            };

            _context.Usuario.Add(usuario);
            _context.SaveChanges();

            return Ok("Usuario creado correctamente");
        }


        //EDITAR UN USUARIO PUT

        [HttpPut("PutUsuario/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            var userInDb = await _context.Usuario.FindAsync(id); // buscamos el usuario en la DB
            if (userInDb == null) return NotFound();

            // Actualizamos solo los campos editables
            userInDb.Nombre = usuario.Nombre;
            userInDb.Email = usuario.Email;
            userInDb.Rol = usuario.Rol;

            // Cambiamos la contraseña solo si el usuario escribió algo
            if (!string.IsNullOrEmpty(usuario.Password))
            {
                userInDb.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
        // ELEIMINAR USUARIO DELETE
        [HttpDelete("DeleteUsuario/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
