using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Obracore.Data;
using Obracore.Models;

namespace Obracore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DocumentosController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Documento>>> Get() =>
            await _context.Documentos.AsNoTracking().ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Documento>> Get(int id)
        {
            var item = await _context.Documentos
                .Include(d => d.Obra)
                .Include(d => d.EnviadoPor)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Documento>> Post(Documento doc)
        {
            _context.Documentos.Add(doc);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = doc.Id }, doc);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Documento doc)
        {
            if (id != doc.Id) return BadRequest();
            _context.Entry(doc).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Documentos.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Documentos.FindAsync(id);
            if (item == null) return NotFound();
            _context.Documentos.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}