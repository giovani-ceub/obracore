using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Obracore.Data;
using Obracore.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Obracore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObrasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _uploadPath;

        public ObrasController(AppDbContext context)
        {
            _context = context;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "obras");
        }

        // GET: api/obras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Obra>>> Get()
        {
            return await _context.Obras.AsNoTracking().ToListAsync();
        }

        // GET: api/obras/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Obra>> Get(int id)
        {
            var item = await _context.Obras
                .Include(o => o.Etapas)
                .Include(o => o.DocumentosObras)
                .Include(o => o.CustosObra)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (item == null)
                return NotFound();

            return item;
        }

        [HttpGet("me")]
        [Authorize] // Protege o endpoint
        public async Task<ActionResult<IEnumerable<Obra>>> GetObrasDoUsuario()
        {
            // Verifica se o usuário é Administrador
            var isAdmin = User.IsInRole("Administrador");
            // Obtém o ID do usuário logado do token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var usuarioId))
            {
                // Isso só deve acontecer se o token for malformado, pois o [Authorize] já validou a autenticação.
                return Unauthorized("ID do usuário logado não encontrado.");
            }

            IQueryable<Obra> query = _context.Obras;

            if (!isAdmin)
            {
                query = query
                    .Include(o => o.UsuariosObras)
                    .Where(o => o.UsuariosObras.Any(uo => uo.UsuarioId == usuarioId));
            }

            // Filtra as obras através da tabela de relacionamento (UsuariosObras)
            var obras = await query
                .AsNoTracking()
                // 💡 Projeção para a Obra (Model), retornando apenas campos primitivos.
                .Select(o => new Obra 
                {
                    Id = o.Id,
                    Nome = o.Nome,
                    Descricao = o.Descricao,
                    DtInicio = o.DtInicio,
                    DtFimPrevista = o.DtFimPrevista,
                    DtFim = o.DtFim,
                    DtCriacao = o.DtCriacao,
                    DtEdicao = o.DtEdicao,
                    StatusObra = o.StatusObra,
                    FotoCapa = o.FotoCapa
                    // **NÃO INCLUA ICollections AQUI!**
                })
                .ToListAsync();

            return obras;
        }

        // POST com suporte a upload de imagem
        [HttpPost]
        [Authorize]
        [RequestSizeLimit(10_000_000)]
        public async Task<ActionResult<Obra>> Post([FromForm] ObraUploadDto dto)
        {
            if (dto == null)
                return BadRequest();

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            
                // Verifica se o ID foi encontrado e é um número (Garante que o [Authorize] funcionou)
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var usuarioId))
                {
                    // Se a requisição não passou pelo [Authorize] ou o token está inválido
                    return Unauthorized("A criação de obras requer um usuário autenticado."); 
                }

            var obra = new Obra
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                DtInicio = dto.DtInicio,
                DtFimPrevista = dto.DtFimPrevista,
                StatusObra = dto.StatusObra,
                DtCriacao = DateTime.UtcNow
            };

            // Upload da imagem
            if (dto.Foto != null)
            {
                obra.FotoCapa = await SalvarImagemAsync(dto.Foto);
            }

            _context.Obras.Add(obra);
            await _context.SaveChangesAsync();

            var usuarioObra = new UsuarioObra
            {
                ObraId = obra.Id, // Usa o ID gerado pelo banco
                UsuarioId = usuarioId // Usa o ID extraído do token
            };

            _context.UsuariosObras.Add(usuarioObra);
            // Salva o relacionamento
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = obra.Id }, obra);
        }

        // PUT padrão (sem imagem)
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Obra obra)
        {
            if (id != obra.Id)
                return BadRequest();

            var existente = await _context.Obras.FindAsync(id);
            if (existente == null)
                return NotFound();

            existente.Nome = obra.Nome;
            existente.Descricao = obra.Descricao;
            existente.StatusObra = obra.StatusObra;
            existente.DtInicio = obra.DtInicio;
            existente.DtFimPrevista = obra.DtFimPrevista;
            existente.DtEdicao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ PUT para atualizar dados + imagem
        [HttpPut("{id}/with-image")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> PutWithImage(int id, [FromForm] ObraUploadDto dto)
        {
            var obra = await _context.Obras.FindAsync(id);
            if (obra == null)
                return NotFound();

            obra.Nome = dto.Nome;
            obra.Descricao = dto.Descricao;
            obra.StatusObra = dto.StatusObra;
            obra.DtInicio = dto.DtInicio;
            obra.DtFimPrevista = dto.DtFimPrevista;
            obra.DtEdicao = DateTime.UtcNow;

            // Atualiza imagem se enviada
            if (dto.Foto != null)
            {
                // Remove imagem antiga
                if (!string.IsNullOrEmpty(obra.FotoCapa))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", obra.FotoCapa);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                obra.FotoCapa = await SalvarImagemAsync(dto.Foto);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Obras.FindAsync(id);
            if (item == null)
                return NotFound();

            if (!string.IsNullOrEmpty(item.FotoCapa))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.FotoCapa);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Obras.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🔧 Método auxiliar centralizado
        private async Task<string> SalvarImagemAsync(IFormFile arquivo)
        {
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(arquivo.FileName)}";
            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return $"images/obras/{fileName}";
        }
    }

    // DTO auxiliar
    public class ObraUploadDto
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFimPrevista { get; set; }
        public string? StatusObra { get; set; }
        public IFormFile? Foto { get; set; }
    }
}
