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
    public class CustosObraController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CustosObraController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustoObra>>> Get() =>
            await _context.CustosObra.AsNoTracking().ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<CustoObra>> Get(int id)
        {
            var item = await _context.CustosObra
                .Include(c => c.Obra)
                .Include(c => c.Etapa)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<CustoObra>> Post(CustoObra custo)
        {
            _context.CustosObra.Add(custo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = custo.Id }, custo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CustoObra custo)
        {
            if (id != custo.Id) return BadRequest();
            _context.Entry(custo).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.CustosObra.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.CustosObra.FindAsync(id);
            if (item == null) return NotFound();
            _context.CustosObra.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}