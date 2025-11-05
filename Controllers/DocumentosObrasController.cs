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
    public class DocumentosObrasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DocumentosObrasController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentoObra>>> Get() =>
            await _context.DocumentosObras.AsNoTracking().ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentoObra>> Get(int id)
        {
            var item = await _context.DocumentosObras
                .Include(d => d.Obra)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<DocumentoObra>> Post(DocumentoObra doc)
        {
            _context.DocumentosObras.Add(doc);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = doc.Id }, doc);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DocumentoObra doc)
        {
            if (id != doc.Id) return BadRequest();
            _context.Entry(doc).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.DocumentosObras.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.DocumentosObras.FindAsync(id);
            if (item == null) return NotFound();
            _context.DocumentosObras.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}