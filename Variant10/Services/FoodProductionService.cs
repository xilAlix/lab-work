using Microsoft.EntityFrameworkCore;
using Variant10.Models;

namespace Variant10.Services;

public class FoodProductionService
{
    private readonly ProductContext _context;

    public FoodProductionService(ProductContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FoodProduction>> GetAllAsync()
    {
        return await _context.FoodProductions.ToListAsync();
    }

    public async Task<FoodProduction?> GetByIdAsync(int firmId, int productId, float productionVolume)
    {
        return await _context.FoodProductions.FindAsync(firmId, productId, productionVolume);
    }

    public async Task<bool> ExistsAsync(int firmId, int productId, float productionVolume)
    {
        return await _context.FoodProductions.AnyAsync(e =>
            e.firmId == firmId &&
            e.productId == productId &&
            e.productionVolume == productionVolume);
    }

    public async Task CreateAsync(FoodProduction entity)
    {
        _context.FoodProductions.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int firmId, int productId, float productionVolume, FoodProduction entity)
    {
        _context.Entry(entity).Property(p => p.firmId).IsModified = false;
        _context.Entry(entity).Property(p => p.productId).IsModified = false;
        _context.Entry(entity).Property(p => p.productionVolume).IsModified = false;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int firmId, int productId, float productionVolume)
    {
        var entity = await GetByIdAsync(firmId, productId, productionVolume);
        if (entity != null)
        {
            _context.FoodProductions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}