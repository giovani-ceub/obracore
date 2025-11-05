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
    public class UsuariosObrasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsuariosObrasController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioObra>>> Get() =>
            await _context.UsuariosObras.AsNoTracking().ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioObra>> Get(int id)
        {
            var item = await _context.UsuariosObras.FindAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioObra>> Post(UsuarioObra uo)
        {
            _context.UsuariosObras.Add(uo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = uo.Id }, uo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UsuarioObra uo)
        {
            if (id != uo.Id) return BadRequest();
            _context.Entry(uo).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UsuariosObras.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.UsuariosObras.FindAsync(id);
            if (item == null) return NotFound();
            _context.UsuariosObras.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}