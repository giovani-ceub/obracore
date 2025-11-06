using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Obracore.Data;
using Obracore.Models;
using Obracore.Services;

namespace Obracore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Realiza o login de um usuário e gera um token JWT
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Senha))
                return BadRequest(new { message = "E-mail e senha são obrigatórios." });

            // 🔍 Busca o usuário com perfis associados
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioPerfis)
                .ThenInclude(up => up.Perfil)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null)
                return Unauthorized(new { message = "Usuário não encontrado." });

            // 🔐 Verifica senha com BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
                return Unauthorized(new { message = "Senha incorreta." });

            // 🔑 Gera token JWT
            var token = _tokenService.GenerateToken(usuario);

            return Ok(new
            {
                token,
                usuario = new
                {
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email,
                    Perfis = usuario.UsuarioPerfis?
                        .Select(up => up.Perfil.Nome)
                        .ToList()
                }
            });
        }

        /// <summary>
        /// Registra um novo usuário (público)
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { message = "E-mail já está em uso." });

            var usuario = new Usuario
            {
                Nome = request.Nome,
                Email = request.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(request.Senha), // ✅ Criptografa senha
                Status = "A",
                DtCriacao = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuário registrado com sucesso." });
        }
    }

    // DTOs auxiliares
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
