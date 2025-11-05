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
    public class FinancasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public FinancasController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Financa>>> Get() =>
            await _context.Financas.AsNoTracking().ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Financa>> Get(int id)
        {
            var item = await _context.Financas
                .Include(f => f.Obra)
                .Include(f => f.Etapa)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Financa>> Post(Financa f)
        {
            _context.Financas.Add(f);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = f.Id }, f);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Financa f)
        {
            if (id != f.Id) return BadRequest();
            _context.Entry(f).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Financas.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Financas.FindAsync(id);
            if (item == null) return NotFound();
            _context.Financas.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}