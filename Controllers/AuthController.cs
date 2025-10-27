using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebTonyWilly.Data;
using WebTonyWilly.Dtos;
using WebTonyWilly.models;

namespace WebTonyWilly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(MyDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginDto)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.Email == loginDto.Usuario);
            if (usuario == null) return Unauthorized("Usuario no encontrado");

            bool validPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.Password);
            if (!validPassword) return Unauthorized("Contraseña incorrecta");

            // Crear JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"] ?? "MiClaveSuperSecretaDeJWT_Con32Caracteres!!");
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            // 🔹 Claims
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, usuario.Email),          // Nombre de usuario
        new Claim(ClaimTypes.Role, usuario.Rol.ToLower()), // Rol correctamente configurado
        new Claim("UserId", usuario.Id.ToString())         // Id del usuario
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                id = usuario.Id,
                Token = tokenString,
                Usuario = usuario.Email,
                Rol = usuario.Rol,
                Nombre = usuario.Nombre
            });
        }
        //registrar REGISTRA UN NUEVO USUSARIO 
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            if (_context.Usuario.Any(u => u.Email == registerDto.Email))
                return BadRequest("El usuario ya existe");

            var usuario = new Usuario
            {
                Nombre = registerDto.Name,
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Rol = "Cliente" // crea Clientes 
            };

            _context.Usuario.Add(usuario);
            _context.SaveChanges();

            return Ok("Usuario creado correctamente");
        }
        //obtener usuario logueado 
        //[HttpGet("me")]
        //[Authorize]
        //public IActionResult Me()
        //{
        //  var username = User.Identity.Name;
        //var usuario = _context.Usuario
        //  .Where(u => u.Email == username)
        //.Select(u => new { u.Nombre, u.Email, u.Rol })
        //.FirstOrDefault();

        //return Ok(usuario);
        //}
        //primer login admin
        [HttpPost("initAdmin")]
        public IActionResult InitAdmin([FromBody] RegisterDto dto)
        {
            if (_context.Usuario.Any()) return BadRequest("Usuarios ya existen");

            var usuario = new Usuario
            {
                Nombre = "Administrador",
                Email = "eterlinda@admin.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Grosales273@"),
                Rol = "Admin"
            };
            _context.Usuario.Add(usuario);
            _context.SaveChanges();
            return Ok("Admin inicial creado");
        }



    }
}
