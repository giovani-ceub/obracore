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

        // POST com suporte a upload de imagem
        [HttpPost]
        [RequestSizeLimit(10_000_000)]
        public async Task<ActionResult<Obra>> Post([FromForm] ObraUploadDto dto)
        {
            if (dto == null)
                return BadRequest();

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
