using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Variant10.Models;

namespace Variant10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturingCompaniesController : ControllerBase
    {
        private readonly ProductContext _context;

        public FacturingCompaniesController(ProductContext context)
        {
            _context = context;
        }

        // GET: api/FacturingCompanies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacturingCompany>>> GetFacturingCompanies()
        {
            return await _context.FacturingCompanies.ToListAsync();
        }

        // GET: api/FacturingCompanies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FacturingCompany>> GetFacturingCompany(int id)
        {
            var facturingCompany = await _context.FacturingCompanies.FindAsync(id);

            if (facturingCompany == null)
            {
                return NotFound();
            }

            return facturingCompany;
        }

        // PUT: api/FacturingCompanies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFacturingCompany(int id, FacturingCompany facturingCompany)
        {
            if (id != facturingCompany.firmId)
            {
                return BadRequest();
            }

            _context.Entry(facturingCompany).Property(c => c.firmId).IsModified = false;

            _context.Entry(facturingCompany).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturingCompanyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FacturingCompanies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FacturingCompany>> PostFacturingCompany(FacturingCompany facturingCompany)
        {
            _context.FacturingCompanies.Add(facturingCompany);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FacturingCompanyExists(facturingCompany.firmId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFacturingCompany", new { id = facturingCompany.firmId }, facturingCompany);
        }

        // DELETE: api/FacturingCompanies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacturingCompany(int id)
        {
            var facturingCompany = await _context.FacturingCompanies.FindAsync(id);
            if (facturingCompany == null)
            {
                return NotFound();
            }

            _context.FacturingCompanies.Remove(facturingCompany);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacturingCompanyExists(int id)
        {
            return _context.FacturingCompanies.Any(e => e.firmId == id);
        }
    }
}
