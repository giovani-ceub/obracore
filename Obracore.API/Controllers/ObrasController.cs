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
    public class ObrasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ObrasController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Obra>>> Get() =>
            await _context.Obras.AsNoTracking().ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Obra>> Get(int id)
        {
            var item = await _context.Obras
                .Include(o => o.Etapas)
                .Include(o => o.DocumentosObras)
                .Include(o => o.CustosObra)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Obra>> Post(Obra obra)
        {
            _context.Obras.Add(obra);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = obra.Id }, obra);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Obra obra)
        {
            if (id != obra.Id) return BadRequest();
            _context.Entry(obra).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Obras.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Obras.FindAsync(id);
            if (item == null) return NotFound();
            _context.Obras.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}