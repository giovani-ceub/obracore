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
    public class UsuarioPerfisController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsuarioPerfisController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioPerfil>>> Get() =>
            await _context.UsuarioPerfis.AsNoTracking().ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioPerfil>> Get(int id)
        {
            var item = await _context.UsuarioPerfis.FindAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioPerfil>> Post(UsuarioPerfil up)
        {
            _context.UsuarioPerfis.Add(up);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = up.Id }, up);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UsuarioPerfil up)
        {
            if (id != up.Id) return BadRequest();
            _context.Entry(up).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UsuarioPerfis.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.UsuarioPerfis.FindAsync(id);
            if (item == null) return NotFound();
            _context.UsuarioPerfis.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}