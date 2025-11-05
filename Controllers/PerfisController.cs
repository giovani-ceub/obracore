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
    public class PerfisController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PerfisController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Perfil>>> Get() =>
            await _context.Perfis.AsNoTracking().ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Perfil>> Get(int id)
        {
            var item = await _context.Perfis.FindAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<Perfil>> Post(Perfil perfil)
        {
            _context.Perfis.Add(perfil);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = perfil.Id }, perfil);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Perfil perfil)
        {
            if (id != perfil.Id) return BadRequest();
            _context.Entry(perfil).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Perfis.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Perfis.FindAsync(id);
            if (item == null) return NotFound();
            _context.Perfis.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}