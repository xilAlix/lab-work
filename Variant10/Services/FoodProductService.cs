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

    public async Task<IEnumerable<FoodProduct>> GetFilteredProductsAsync(int? minId, string? group, string? sortBy)
    {
        IQueryable<FoodProduct> query = _context.FoodProducts.AsQueryable();

        if (minId.HasValue)
            query = query.Where(p => p.id >= minId.Value);

        if (!string.IsNullOrWhiteSpace(group))
            query = query.Where(p => p.productGroup.ToLower().Contains(group.ToLower()));

        switch (sortBy?.ToLower())
        {
            case "id_asc": query = query.OrderBy(p => p.id); break;
            case "id_desc": query = query.OrderByDescending(p => p.id); break;
            case "title_asc": query = query.OrderBy(p => p.title); break;
            case "title_desc": query = query.OrderByDescending(p => p.title); break;
        }

        return await query.ToListAsync();
    }
}