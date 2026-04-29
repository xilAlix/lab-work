using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Variant10.Models;
using Variant10.Services;

namespace Variant10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturingCompaniesController : ControllerBase
    {
        private readonly FacturingCompanyService _factruningCompanyService;

        public FacturingCompaniesController(FacturingCompanyService factruningCompanyService)
        {
            _factruningCompanyService = factruningCompanyService;
        }

        // GET: api/FacturingCompanies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacturingCompany>>> GetFacturingCompanies()
        {
            var result = await _factruningCompanyService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/FacturingCompanies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FacturingCompany>> GetFacturingCompany(int id)
        {
            var company = await _factruningCompanyService.GetByIdAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
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

            try
            {
                await _factruningCompanyService.UpdateAsync(id, facturingCompany);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _factruningCompanyService.ExistsAsync(id))
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
            try
            {
                await _factruningCompanyService.CreateAsync(facturingCompany);
            }
            catch (DbUpdateException)
            {
                if (await _factruningCompanyService.ExistsAsync(facturingCompany.firmId))
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
            var company = await _factruningCompanyService.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            await _factruningCompanyService.DeleteAsync(id);

            return NoContent();
        }
    }
}
