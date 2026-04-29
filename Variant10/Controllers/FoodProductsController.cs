using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Variant10.Models;
using Variant10.Services;

namespace Variant10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodProductsController : ControllerBase
    {
        private readonly FoodProductService _foodProductService;

        public FoodProductsController(FoodProductService foodProductService)
        {
            _foodProductService = foodProductService;
        }

        // GET: api/FoodProducts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodProduct>>> GetFoodProducts(
            [FromQuery] int? minId = null,
            [FromQuery] string? group = null,
            [FromQuery] string? sortBy = null)
        {
            var result = await _foodProductService.GetAllFilteredAsync(minId, group, sortBy);
            return Ok(result);
        }

        // GET: api/FoodProducts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodProduct>> GetFoodProduct(int id)
        {
            var product = await _foodProductService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/FoodProducts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodProduct(int id, FoodProduct foodProduct)
        {
            if (id != foodProduct.id)
            {
                return BadRequest();
            }

            try
            {
                await _foodProductService.UpdateAsync(id, foodProduct);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _foodProductService.ExistsAsync(id))
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

        // POST: api/FoodProducts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FoodProduct>> PostFoodProduct(FoodProduct foodProduct)
        {

            try
            {
                await _foodProductService.CreateAsync(foodProduct);
            }
            catch (DbUpdateException)
            {
                if (await _foodProductService.ExistsAsync(foodProduct.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFoodProduct", new { id = foodProduct.id }, foodProduct);
        }

        // DELETE: api/FoodProducts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodProduct(int id)
        {
            var product = await _foodProductService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _foodProductService.DeleteAsync(id);
            return NoContent();
        }
    }
}
