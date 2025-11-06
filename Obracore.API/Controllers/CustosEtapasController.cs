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
    public class CustosEtapasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CustosEtapasController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustoEtapa>>> Get() =>
            await _context.CustosEtapas.AsNoTracking().ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<CustoEtapa>> Get(int id)
        {
            var item = await _context.CustosEtapas
                .Include(c => c.Etapa)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<CustoEtapa>> Post(CustoEtapa custo)
        {
            _context.CustosEtapas.Add(custo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = custo.Id }, custo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CustoEtapa custo)
        {
            if (id != custo.Id) return BadRequest();
            _context.Entry(custo).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.CustosEtapas.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.CustosEtapas.FindAsync(id);
            if (item == null) return NotFound();
            _context.CustosEtapas.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}