using Microsoft.EntityFrameworkCore;
using Variant10.Models;

namespace Variant10.Services;

public class FoodProductService
{
    private readonly ProductContext _context;

    public FoodProductService(ProductContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FoodProduct>> GetAllFilteredAsync(int? minId, string? group, string? sortBy)
    {
        IQueryable<FoodProduct> query = _context.FoodProducts.FromSqlRaw("SELECT * FROM get_all_foodproducts()").AsQueryable();

        if (minId.HasValue)
        {
            query = query.Where(p => p.id >= minId.Value);
        }

        if (!string.IsNullOrWhiteSpace(group))
        {
            query = query.Where(p => p.productGroup.ToLower().Contains(group.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "id_asc":
                    query = query.OrderBy(p => p.id);
                    break;
                case "id_desc":
                    query = query.OrderByDescending(p => p.id);
                    break;
                case "title_asc":
                    query = query.OrderBy(p => p.title);
                    break;
                case "title_desc":
                    query = query.OrderByDescending(p => p.title);
                    break;
            }
        }

        return await query.ToListAsync();
    }

    public async Task<FoodProduct?> GetByIdAsync(int id)
    {
        var rows = await _context.FoodProducts.FromSqlRaw("SELECT * FROM get_foodproduct_by_id({0})", id).ToListAsync();
        return rows.FirstOrDefault();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.FoodProducts.AnyAsync(e => e.id == id);
    }

    public async Task CreateAsync(FoodProduct entity)
    {
        await _context.Database.ExecuteSqlRawAsync( 
            "SELECT add_foodproduct({0}, {1}, {2}, {3})",
            entity.id, entity.title.Trim(), entity.productGroup.Trim(), entity.packageType?.Trim() ?? ""); 
    }

    public async Task UpdateAsync(int id, FoodProduct entity)
    {
        await _context.Database.ExecuteSqlRawAsync( 
            "SELECT update_foodproduct({0}, {1}, {2}, {3})", 
            id, entity.title.Trim(), entity.productGroup.Trim(), entity.packageType?.Trim() ?? ""); 
    }

    public async Task DeleteAsync(int id)
    {
        await _context.Database.ExecuteSqlRawAsync("SELECT delete_foodproduct({0})", id); 
    }

    public async Task<string> GetStatisticsAsync() 
    { 
        var statsJson = await _context.Database.SqlQueryRaw<string>("SELECT get_foodproducts_statistics()").ToListAsync();
        return statsJson.First(); 
    } 

    public async Task<string> GetLogsAsync(int limit = 50) 
    { 
        var logs = await _context.Database 
            .SqlQueryRaw<string>($"SELECT json_agg(l ORDER BY l.id DESC) FROM foodproducts_log l LIMIT {limit}") 
            .ToListAsync();
        return logs.FirstOrDefault() ?? "[]"; 
    }
}