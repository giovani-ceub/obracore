using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Obracore.Data;
using Obracore.Models;
using Obracore.Services;
using System.IdentityModel.Tokens.Jwt;

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

        /// Realiza o login e gera o token JWT
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Senha))
                return BadRequest(new { message = "E-mail e senha são obrigatórios." });

            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioPerfis)
                .ThenInclude(up => up.Perfil)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null)
                return Unauthorized(new { message = "Usuário não encontrado." });

            if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.Senha))
                return Unauthorized(new { message = "Senha incorreta." });

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

        /// Registra novo usuário
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { message = "E-mail já está em uso." });

            var usuario = new Usuario
            {
                Nome = request.Nome,
                Email = request.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                Status = "A",
                DtCriacao = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuário registrado com sucesso." });
        }

        /// Retorna os dados do usuário autenticado a partir do token JWT
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
                return Unauthorized(new { message = "Token não informado." });

            var token = authHeader.Substring("Bearer ".Length);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Extrai o e-mail do claim "unique_name"
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Token inválido (e-mail não encontrado)." });

            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioPerfis)
                .ThenInclude(up => up.Perfil)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null)
                return Unauthorized(new { message = "Usuário não encontrado." });

            return Ok(new
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfis = usuario.UsuarioPerfis?.Select(up => up.Perfil.Nome).ToList()
            });
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
