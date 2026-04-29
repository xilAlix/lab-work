using Microsoft.EntityFrameworkCore;
using Variant10.Models;

namespace Variant10.Services;

public class FacturingCompanyService
{
    private readonly ProductContext _context;

    public FacturingCompanyService(ProductContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FacturingCompany>> GetAllAsync()
    {
        return await _context.FacturingCompanies.ToListAsync();
    }

    public async Task<FacturingCompany?> GetByIdAsync(int id)
    {
        return await _context.FacturingCompanies.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.FacturingCompanies.AnyAsync(e => e.firmId == id);
    }

    public async Task CreateAsync(FacturingCompany entity)
    {
        _context.FacturingCompanies.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, FacturingCompany entity)
    {
        _context.Entry(entity).Property(c => c.firmId).IsModified = false;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.FacturingCompanies.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}