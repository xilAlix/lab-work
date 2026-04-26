using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Variant10.Models;

namespace Variant10.wwwroot
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodProductionsController : ControllerBase
    {
        private readonly ProductContext _context;

        public FoodProductionsController(ProductContext context)
        {
            _context = context;
        }

        // GET: api/FoodProductions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodProduction>>> GetFoodProductions()
        {
            return await _context.FoodProductions.ToListAsync();
        }

        // GET: api/FoodProductions/5
        [HttpGet("{firmId}/{productId}/{productionVolume}")]
        public async Task<ActionResult<FoodProduction>> GetFoodProduction(int firmId, int productId, float productionVolume)
        {
            var foodProduction = await _context.FoodProductions.FindAsync(firmId, productId, productionVolume);

            if (foodProduction == null)
            {
                return NotFound();
            }

            return foodProduction;
        }

        // PUT: api/FoodProductions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{firmId}/{productId}/{productionVolume}")]
        public async Task<IActionResult> PutFoodProduction(int firmId, int productId, float productionVolume, FoodProduction foodProduction)
        {
            if (firmId != foodProduction.firmId || productId != foodProduction.productId || productionVolume != foodProduction.productionVolume)
            {
                return BadRequest();
            }

            _context.Entry(foodProduction).Property(p => p.firmId).IsModified = false;
            _context.Entry(foodProduction).Property(p => p.productId).IsModified = false;
            _context.Entry(foodProduction).Property(p => p.productionVolume).IsModified = false;

            _context.Entry(foodProduction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodProductionExists(firmId, productId, productionVolume))
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

        // POST: api/FoodProductions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FoodProduction>> PostFoodProduction(FoodProduction foodProduction)
        {
            _context.FoodProductions.Add(foodProduction);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FoodProductionExists(foodProduction.firmId, foodProduction.productId, foodProduction.productionVolume))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFoodProduction", new { firmId = foodProduction.firmId, productId = foodProduction.productId, productionVolume = foodProduction.productionVolume }, foodProduction);
        }

        // DELETE: api/FoodProductions/5
        [HttpDelete("{firmId}/{productId}/{productionVolume}")]
        public async Task<IActionResult> DeleteFoodProduction(int firmId, int productId, float productionVolume)
        {
            var foodProduction = await _context.FoodProductions.FindAsync(firmId, productId, productionVolume);
            if (foodProduction == null)
            {
                return NotFound();
            }

            _context.FoodProductions.Remove(foodProduction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FoodProductionExists(int firmId, int productId, float productionVolume)
        {
            return _context.FoodProductions.Any(e => e.firmId == firmId && e.productId == productId && e.productionVolume == productionVolume);
        }


        // GET: api/FoodProductions/muchFirms
        [HttpGet("muchFirms")]
        public async Task<IEnumerable<FoodProduction>> GetFrequentFirms()
        {
            var frequentFirmIds = from fp in _context.FoodProductions
                                  group fp by fp.firmId into g
                                  where g.Count() > 3
                                  select g.Key;

            var frequentFirmsProductions = from fp in _context.FoodProductions
                                           where frequentFirmIds.Contains(fp.firmId)
                                           select fp;

            return await frequentFirmsProductions.ToListAsync();
        }
    }
}
