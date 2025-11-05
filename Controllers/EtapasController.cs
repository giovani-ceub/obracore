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
    public class EtapasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EtapasController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Etapa>>> Get() =>
            await _context.Etapas.AsNoTracking().ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Etapa>> Get(int id)
        {
            var item = await _context.Etapas
                .Include(e => e.CustosEtapa)
                .Include(e => e.DocumentosEtapa)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Etapa>> Post(Etapa etapa)
        {
            _context.Etapas.Add(etapa);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = etapa.Id }, etapa);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Etapa etapa)
        {
            if (id != etapa.Id) return BadRequest();
            _context.Entry(etapa).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Etapas.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Etapas.FindAsync(id);
            if (item == null) return NotFound();
            _context.Etapas.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}