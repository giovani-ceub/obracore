using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Obracore.Data;
using Obracore.Models;
// using BCrypt.Net; 

namespace Obracore.Controllers
{
    // 🔒 RESTRIÇÃO: Somente usuários com o perfil "Administrador" podem acessar esta Controller
    [Authorize(Roles = "Administrador")] 
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext context) => _context = context;

        // Método auxiliar para obter o ID do usuário logado
        private int ObterUsuarioIdDoToken()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0; // Se [Authorize] falhar ou não tiver ID, retorna 0.
        }

        // GET: api/Usuarios (Para a Lista Geral)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> Get()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.UsuarioPerfis)
                    .ThenInclude(up => up.Perfil)
                .AsNoTracking()
                .ToListAsync();

            // Mapeia o Model para o DTO de Resposta (ocultando a senha e mostrando nomes dos perfis)
            var dtos = usuarios.Select(u => new UsuarioResponseDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Status = u.Status,
                DtCriacao = u.DtCriacao,
                DtEdicao = u.DtEdicao,
                // Formata os nomes dos perfis para o campo PerfisDisplay do Frontend
                PerfisDisplay = string.Join(", ", u.UsuarioPerfis?.Select(up => up.Perfil?.Nome ?? "Desconhecido").ToList() ?? new List<string>())
            }).ToList();

            return Ok(dtos);
        }

        // GET: api/Usuarios/{id} (Para a Tela de Edição - deve retornar os IDs dos perfis)
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioEditDto>> Get(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioPerfis)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (usuario == null) return NotFound();

            // Mapeia para o DTO de Edição (incluindo apenas os IDs dos perfis)
            var dto = new UsuarioEditDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Status = usuario.Status,
                PerfilIds = usuario.UsuarioPerfis?.Select(up => up.PerfilId).ToList() ?? new List<int>()
            };
            
            return Ok(dto);
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> Post([FromBody] UsuarioCreateDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "E-mail já está em uso." });
            
            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                // 🔒 HASHING DE SENHA OBRIGATÓRIO:
                Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha), 
                Status = dto.Status,
                DtCriacao = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            
            // --- Lógica para salvar os perfis ---
            if (dto.PerfilIds != null && dto.PerfilIds.Any())
            {
                var perfisParaAdicionar = dto.PerfilIds
                    .Select(perfilId => new UsuarioPerfil { UsuarioId = usuario.Id, PerfilId = perfilId })
                    .ToList();
                
                _context.UsuarioPerfis.AddRange(perfisParaAdicionar);
                await _context.SaveChangesAsync();
            }
            // --- Fim da lógica de perfis ---

            // Retorna o DTO de resposta para confirmação
            var responseDto = new UsuarioResponseDto 
            { 
                Id = usuario.Id, 
                Nome = usuario.Nome, 
                Email = usuario.Email,
                DtCriacao = usuario.DtCriacao,
                Status = usuario.Status,
                // Nota: O PerfisDisplay pode não estar completo aqui sem outra consulta, 
                // mas para CreatedAtAction podemos omitir ou calcular se necessário.
            };

            return CreatedAtAction(nameof(Get), new { id = usuario.Id }, responseDto);
        }

        // PUT: api/Usuarios/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioUpdateDto dto)
        {
            var existente = await _context.Usuarios
                .Include(u => u.UsuarioPerfis) // Precisa incluir para gerenciar perfis
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (existente == null) return NotFound();

            // Validação de E-mail (se for alterado, não pode colidir com outro usuário)
            if (existente.Email != dto.Email && await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.Id != id))
            {
                return BadRequest(new { message = "O novo e-mail já está em uso por outro usuário." });
            }

            // Atualiza campos
            existente.Nome = dto.Nome;
            existente.Email = dto.Email;
            existente.Status = dto.Status;
            existente.DtEdicao = DateTime.UtcNow;

            // Atualiza senha se for fornecida
            if (!string.IsNullOrEmpty(dto.NovaSenha))
            {
                existente.Senha = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
            }

            // --- Lógica para atualizar os perfis (Substituir) ---
            
            // 1. Remove todos os perfis existentes
            _context.UsuarioPerfis.RemoveRange(existente.UsuarioPerfis);

            // 2. Adiciona os novos perfis (IDs enviados pelo frontend)
            if (dto.PerfilIds != null && dto.PerfilIds.Any())
            {
                var novosPerfis = dto.PerfilIds
                    .Select(perfilId => new UsuarioPerfil { UsuarioId = existente.Id, PerfilId = perfilId })
                    .ToList();
                
                _context.UsuarioPerfis.AddRange(novosPerfis);
            }
            // --- Fim da lógica de perfis ---

            _context.Entry(existente).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usuarios.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            
            return NoContent();
        }

        // DELETE: api/Usuarios/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Opcional: Impedir que um Administrador se exclua acidentalmente
            if (ObterUsuarioIdDoToken() == id)
            {
                return Forbid("Administrador não pode excluir sua própria conta através desta API.");
            }

            // Garante que os registros de relacionamento também sejam excluídos se não estiverem configurados para Cascade Delete
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioPerfis)
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (usuario == null) return NotFound();

            // Exclui os relacionamentos de perfil
            _context.UsuarioPerfis.RemoveRange(usuario.UsuarioPerfis);

            // Exclui o usuário
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DTOs Auxiliares

        // Para retornar dados do usuário (saída - Lista)
        public class UsuarioResponseDto
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Status { get; set; }
            public DateTime DtCriacao { get; set; }
            public DateTime? DtEdicao { get; set; }
            // Campo de exibição para a lista, contendo os nomes dos perfis
            public string? PerfisDisplay { get; set; } 
        }

        // Para retornar dados do usuário para EDIÇÃO (saída - GetById)
        public class UsuarioEditDto
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Status { get; set; }
            // Propriedade crucial para carregar as opções pré-selecionadas no formulário Blazor
            public List<int> PerfilIds { get; set; } = new List<int>(); 
        }

        // Para criar um novo usuário (entrada)
        public class UsuarioCreateDto
        {
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty; // Senha a ser hasheada
            public string Status { get; set; } = "A"; // Default
            // Lista de IDs de perfis a serem associados ao novo usuário
            public List<int> PerfilIds { get; set; } = new List<int>(); 
        }

        // Para atualizar um usuário existente (entrada)
        public class UsuarioUpdateDto
        {
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Status { get; set; }
            public string? NovaSenha { get; set; } // Opcional
            // Lista de IDs de perfis para substituir os perfis existentes
            public List<int> PerfilIds { get; set; } = new List<int>(); 
        }
    }
}