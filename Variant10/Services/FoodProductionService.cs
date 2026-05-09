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

    public async Task<IEnumerable<object>> GetAllAsync()
    {
        return await _context.FoodProductions
            .Select(fp => new
            {
                firmId = fp.firmId,
                productId = fp.productId,
                firmDisplay = $"{fp.firmId} ({fp.firmIdNavigation.firmName})",
                productDisplay = $"{fp.productId} ({fp.productIdNavigation.title})",
                productionVolume = fp.productionVolume
            })
            .ToListAsync();
    }

    public async Task<FoodProduction?> GetByIdAsync(int firmId, int productId, float productionVolume)
    {
        var rows = await _context.FoodProductions.FromSqlRaw("SELECT * FROM get_foodproduction_by_id({0}, {1}, {2}::real)", firmId, productId, productionVolume).ToListAsync(); 
        return rows.FirstOrDefault();
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
        await _context.Database.ExecuteSqlRawAsync(
            "SELECT add_foodproduction({0}, {1}, {2}::real)", 
            entity.firmId, entity.productId, entity.productionVolume);
    }

    public async Task UpdateAsync(int firmId, int productId, float productionVolume, FoodProduction entity)
    {
        await _context.Database.ExecuteSqlRawAsync( 
            "SELECT update_foodproduction({0}, {1}, {2}::real)", 
            firmId, productId, productionVolume); 
    }

    public async Task DeleteAsync(int firmId, int productId, float productionVolume)
    {
        await _context.Database.ExecuteSqlRawAsync("SELECT delete_foodproduction({0}, {1}, {2}::real)", firmId, productId, productionVolume); 
    }

    public async Task<string> GetStatisticsAsync() 
    { 
        var statsJson = await _context.Database.SqlQueryRaw<string>("SELECT get_foodproductions_statistics()").ToListAsync(); 
        return statsJson.First(); 
    } 

    public async Task<string> GetLogsAsync(int limit = 50)
    { 
        var logs = await _context.Database 
            .SqlQueryRaw<string>($"SELECT json_agg(l ORDER BY l.id DESC) FROM foodproductions_log l LIMIT {limit}") 
            .ToListAsync(); 
        return logs.FirstOrDefault() ?? "[]"; 
    } 
}