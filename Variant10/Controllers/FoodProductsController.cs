using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Variant10.Models;
using Variant10.Services;

namespace Variant10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodProductsController : ControllerBase
    {
        private readonly ProductContext _context;
        private readonly FoodProductService _foodProductService;

        public FoodProductsController(ProductContext context, FoodProductService foodProductService)
        {
            _context = context;
            _foodProductService = foodProductService;
        }

        // GET: api/FoodProducts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodProduct>>> GetFoodProducts(
            [FromQuery] int? minId = null,
            [FromQuery] string? group = null,
            [FromQuery] string? sortBy = null)
        {
            var products = await _foodProductService.GetFilteredProductsAsync(minId, group, sortBy);
            return Ok(products);
        }

        // GET: api/FoodProducts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodProduct>> GetFoodProduct(int id)
        {
            var foodProduct = await _context.FoodProducts.FindAsync(id);

            if (foodProduct == null)
            {
                return NotFound();
            }

            return foodProduct;
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

            _context.Entry(foodProduct).Property(p => p.id).IsModified = false;

            _context.Entry(foodProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodProductExists(id))
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
            _context.FoodProducts.Add(foodProduct);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FoodProductExists(foodProduct.id))
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
            var foodProduct = await _context.FoodProducts.FindAsync(id);
            if (foodProduct == null)
            {
                return NotFound();
            }

            _context.FoodProducts.Remove(foodProduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FoodProductExists(int id)
        {
            return _context.FoodProducts.Any(e => e.id == id);
        }
    }
}
