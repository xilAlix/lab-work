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
        IQueryable<FoodProduct> query = _context.FoodProducts.AsQueryable();

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
        return await _context.FoodProducts.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.FoodProducts.AnyAsync(e => e.id == id);
    }

    public async Task CreateAsync(FoodProduct entity)
    {
        _context.FoodProducts.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, FoodProduct entity)
    {
        _context.Entry(entity).Property(p => p.id).IsModified = false;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.FoodProducts.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}