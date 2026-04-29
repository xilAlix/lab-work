using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Variant10.Models;
using Variant10.Services;

namespace Variant10.wwwroot
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodProductionsController : ControllerBase
    {
        private readonly FoodProductionService _foodProductionService;

        public FoodProductionsController(FoodProductionService foodProductionService)
        {
            _foodProductionService = foodProductionService;
        }

        // GET: api/FoodProductions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodProduction>>> GetFoodProductions()
        {
            var result = await _foodProductionService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/FoodProductions/5
        [HttpGet("{firmId}/{productId}/{productionVolume}")]
        public async Task<ActionResult<FoodProduction>> GetFoodProduction(int firmId, int productId, float productionVolume)
        {
            var production = await _foodProductionService.GetByIdAsync(firmId, productId, productionVolume);

            if (production == null)
            {
                return NotFound();
            }

            return Ok(production);
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

            try
            {
                await _foodProductionService.UpdateAsync(firmId, productId, productionVolume, foodProduction);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _foodProductionService.ExistsAsync(firmId, productId, productionVolume))
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
            try
            {
                await _foodProductionService.CreateAsync(foodProduction);
            }
            catch (DbUpdateException)
            {
                if (await _foodProductionService.ExistsAsync(foodProduction.firmId, foodProduction.productId, foodProduction.productionVolume))
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
            var production = await _foodProductionService.GetByIdAsync(firmId, productId, productionVolume);
            if (production == null)
            {
                return NotFound();
            }

            await _foodProductionService.DeleteAsync(firmId, productId, productionVolume);
            return NoContent();
        }
    }
}
