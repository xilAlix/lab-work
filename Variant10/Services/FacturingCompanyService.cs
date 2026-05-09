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
        return await _context.FacturingCompanies.FromSqlRaw("SELECT * FROM get_all_facturingcompanies()").ToListAsync();
    }

    public async Task<FacturingCompany?> GetByIdAsync(int id)
    {
        var rows = await _context.FacturingCompanies.FromSqlRaw("SELECT * FROM get_facturingcompany_by_id({0})", id).ToListAsync();
        return rows.FirstOrDefault();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.FacturingCompanies.AnyAsync(e => e.firmId == id);
    }

    public async Task CreateAsync(FacturingCompany entity)
    {
        await _context.Database.ExecuteSqlRawAsync( 
            "SELECT add_facturingcompany({0}, {1}, {2}, {3})",
            entity.firmId, entity.firmName.Trim(), entity.adress.Trim(), entity.directorSurname?.Trim() ?? ""); 
    }

    public async Task UpdateAsync(int id, FacturingCompany entity)
    {
        await _context.Database.ExecuteSqlRawAsync( 
            "SELECT update_facturingcompany({0}, {1}, {2}, {3})",
            id, entity.firmName.Trim(), entity.adress.Trim(), entity.directorSurname?.Trim() ?? ""); 
    }

    public async Task DeleteAsync(int id)
    {
        await _context.Database.ExecuteSqlRawAsync("SELECT delete_facturingcompany({0})", id);
    }

    public async Task<string> GetStatisticsAsync() 
    { 
        var statsJson = await _context.Database.SqlQueryRaw<string>("SELECT get_facturingcompanies_statistics()").ToListAsync();
        return statsJson.First();
    } 

    public async Task<string> GetLogsAsync(int limit = 50) 
    { 
        var logs = await _context.Database 
            .SqlQueryRaw<string>($"SELECT json_agg(l ORDER BY l.id DESC) FROM facturingcompanies_log l LIMIT {limit}") 
            .ToListAsync(); 
        return logs.FirstOrDefault() ?? "[]"; 
    } 
}