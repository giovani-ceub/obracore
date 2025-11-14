using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Obracore.Data;
using Obracore.Models;

namespace Obracore.API.Controllers
{
    // ====================================================================
    // DTOs
    // ====================================================================

    public class CustoEtapaDto
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }

    public class DocumentoEtapaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Caminho { get; set; } = string.Empty;
    }

    public class EtapaCreateUpdateDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }

        public int ObraId { get; set; }

        public string Status { get; set; } = "P";
        public int Ordem { get; set; }

        public DateTime? DtInicio { get; set; }
        public DateTime? DtFimPrevista { get; set; }
        public DateTime? DtFim { get; set; }

        public List<CustoEtapaDto> CustosEtapa { get; set; } = new();
        public List<DocumentoEtapaDto> DocumentosEtapa { get; set; } = new();
    }

    public class EtapaOutputDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Ordem { get; set; }
        public int ObraId { get; set; }

        public DateTime? DtInicio { get; set; }
        public DateTime? DtFimPrevista { get; set; }
        public DateTime? DtFim { get; set; }

        public List<CustoEtapaDto> CustosEtapa { get; set; } = new();
        public List<DocumentoEtapaDto> DocumentosEtapa { get; set; } = new();
    }

    public class EtapaUploadDto
    {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public int ObraId { get; set; }
        public string Status { get; set; }
        public int Ordem { get; set; }

        public List<IFormFile>? Documentos { get; set; }
    }

    // ====================================================================
    // CONTROLLER
    // ====================================================================

    [Route("api/[controller]")]
    [ApiController]
    public class EtapasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _uploadEtapasPath;

        public EtapasController(AppDbContext context)
        {
            _context = context;

            _uploadEtapasPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "images", "etapas"
            );
        }

        // ====================================================================
        // GET ALL
        // ====================================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EtapaOutputDto>>> GetAll()
        {
            var etapas = await _context.Etapas
                .Include(e => e.CustosEtapa)
                .Include(e => e.DocumentosEtapa)
                .ToListAsync();

            return etapas.Select(MapToOutputDto).ToList();
        }

        // ====================================================================
        // GET BY ID
        // ====================================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<EtapaOutputDto>> GetEtapa(int id)
        {
            var etapa = await _context.Etapas
                .Include(e => e.CustosEtapa)
                .Include(e => e.DocumentosEtapa)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (etapa == null) return NotFound();

            return MapToOutputDto(etapa);
        }

        // ====================================================================
        // MÉTODO DE UPLOAD — usado pelo POST /upload
        // ====================================================================
        private async Task<string> SalvarDocumentoEtapaAsync(IFormFile arquivo)
        {
            if (!Directory.Exists(_uploadEtapasPath))
                Directory.CreateDirectory(_uploadEtapasPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(arquivo.FileName)}";
            var path = Path.Combine(_uploadEtapasPath, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await arquivo.CopyToAsync(stream);

            // Caminho exposto ao frontend
            return $"images/etapas/{fileName}";
        }

        // ====================================================================
        // POST PADRÃO (sem upload)
        // ====================================================================
        [HttpPost]
        public async Task<ActionResult<EtapaOutputDto>> Create(EtapaCreateUpdateDto dto)
        {
            if (!await _context.Obras.AnyAsync(o => o.Id == dto.ObraId))
                return NotFound($"Obra {dto.ObraId} não encontrada.");

            var etapa = new Etapa
            {
                Nome = dto.Titulo,
                Descricao = dto.Descricao,
                Status = dto.Status,
                Ordem = dto.Ordem,
                ObraId = dto.ObraId,
                DtInicio = dto.DtInicio,
                DtFimPrevista = dto.DtFimPrevista,
                DtFim = dto.DtFim
            };

            foreach (var c in dto.CustosEtapa)
                etapa.CustosEtapa.Add(new CustoEtapa { Descricao = c.Descricao, Valor = c.Valor, DtRegistro = DateTime.UtcNow });

            foreach (var d in dto.DocumentosEtapa)
                etapa.DocumentosEtapa.Add(new DocumentoEtapa { Titulo = d.Titulo, Caminho = d.Caminho, DtCriacao = DateTime.UtcNow });

            _context.Etapas.Add(etapa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEtapa), new { id = etapa.Id }, MapToOutputDto(etapa));
        }

        // ====================================================================
        // PUT
        // ====================================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EtapaCreateUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID inválido.");

            var etapa = await _context.Etapas
                .Include(e => e.CustosEtapa)
                .Include(e => e.DocumentosEtapa)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (etapa == null)
                return NotFound("Etapa não encontrada.");

            etapa.Nome = dto.Titulo;
            etapa.Descricao = dto.Descricao;
            etapa.Status = dto.Status;
            etapa.Ordem = dto.Ordem;
            etapa.DtInicio = dto.DtInicio;
            etapa.DtFimPrevista = dto.DtFimPrevista;

            etapa.CustosEtapa.Clear();
            dto.CustosEtapa.ForEach(c =>
                etapa.CustosEtapa.Add(new CustoEtapa
                {
                    Descricao = c.Descricao,
                    Valor = c.Valor,
                    DtRegistro = DateTime.UtcNow
                })
            );

            etapa.DocumentosEtapa.Clear();
            dto.DocumentosEtapa.ForEach(d =>
                etapa.DocumentosEtapa.Add(new DocumentoEtapa
                {
                    Titulo = d.Titulo,
                    Caminho = d.Caminho,
                    DtCriacao = DateTime.UtcNow
                })
            );

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ====================================================================
        // DELETE
        // ====================================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var etapa = await _context.Etapas.FindAsync(id);
            if (etapa == null) return NotFound();

            _context.Etapas.Remove(etapa);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ====================================================================
        // POST COM UPLOAD REAL
        // ====================================================================
        [HttpPost("upload")]
        public async Task<IActionResult> CreateComUpload([FromForm] EtapaUploadDto dto)
        {
            var etapa = new Etapa
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                ObraId = dto.ObraId,
                Ordem = dto.Ordem,
                Status = dto.Status
            };

            _context.Etapas.Add(etapa);
            await _context.SaveChangesAsync();

            if (dto.Documentos != null)
            {
                foreach (var file in dto.Documentos)
                {
                    var caminho = await SalvarDocumentoEtapaAsync(file);

                    _context.DocumentosEtapas.Add(new DocumentoEtapa
                    {
                        EtapaId = etapa.Id,
                        Titulo = file.FileName,
                        Caminho = caminho
                    });
                }

                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetEtapa), new { id = etapa.Id }, MapToOutputDto(etapa));
        }

        // ====================================================================
        // UPLOAD DE DOCUMENTOS PARA UMA ETAPA EXISTENTE
        // ====================================================================
        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadDocumentos(int id)
        {
            var etapa = await _context.Etapas.FindAsync(id);
            if (etapa == null)
                return NotFound($"Etapa {id} não encontrada.");

            var arquivos = Request.Form.Files;
            if (arquivos == null || arquivos.Count == 0)
                return BadRequest("Nenhum arquivo enviado.");

            // garantir pasta
            if (!Directory.Exists(_uploadEtapasPath))
                Directory.CreateDirectory(_uploadEtapasPath);

            var extensoesPermitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx"
            };

            foreach (var arquivo in arquivos)
            {
                var ext = Path.GetExtension(arquivo.FileName);
                if (string.IsNullOrEmpty(ext) || !extensoesPermitidas.Contains(ext))
                    continue;

                var nomeArquivo = $"{Guid.NewGuid()}{ext}";
                var caminhoFisico = Path.Combine(_uploadEtapasPath, nomeArquivo);

                using (var stream = new FileStream(caminhoFisico, FileMode.Create))
                    await arquivo.CopyToAsync(stream);

                var caminhoDb = $"images/etapas/{nomeArquivo}";

                var documento = new DocumentoEtapa
                {
                    EtapaId = etapa.Id,
                    Titulo = arquivo.FileName,
                    Caminho = caminhoDb,
                    DtCriacao = DateTime.UtcNow
                };

                _context.DocumentosEtapas.Add(documento);
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        // ====================================================================
        // MAP
        // ====================================================================
        private static EtapaOutputDto MapToOutputDto(Etapa e)
        {
            return new EtapaOutputDto
            {
                Id = e.Id,
                Titulo = e.Nome,
                Descricao = e.Descricao,
                Status = e.Status,
                Ordem = e.Ordem,
                ObraId = e.ObraId,

                DtInicio = e.DtInicio,
                DtFimPrevista = e.DtFimPrevista,
                DtFim = e.DtFim,

                CustosEtapa = e.CustosEtapa.Select(c => new CustoEtapaDto
                {
                    Id = c.Id,
                    Descricao = c.Descricao,
                    Valor = c.Valor
                }).ToList(),

                DocumentosEtapa = e.DocumentosEtapa.Select(d => new DocumentoEtapaDto
                {
                    Id = d.Id,
                    Titulo = d.Titulo,
                    Caminho = d.Caminho
                }).ToList()
            };
        }
    }
}
