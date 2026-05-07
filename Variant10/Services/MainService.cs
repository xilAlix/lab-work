using Microsoft.EntityFrameworkCore;
using Variant10.Models;
using Variant10.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Variant10.Services;

public class MainService
{
    private readonly ProductContext _context;
    private readonly FacturingCompanyService _companyService;
    private readonly FoodProductService _productService;
    private readonly FoodProductionService _productionService;

    public MainService(
        ProductContext context,
        FacturingCompanyService companyService,
        FoodProductService productService,
        FoodProductionService productionService)
    {
        _context = context;
    }

    public async Task<IEnumerable<FacturingCompany>> GetCompaniesWithBakeryProductsAsync()
    {
        var query = from company in _context.FacturingCompanies
                    where (from fp in _context.FoodProductions
                           join product in _context.FoodProducts on fp.productId equals product.id
                           where fp.firmId == company.firmId
                                 && product.productGroup == "Хлебобулочные"
                           select fp).Any()
                    select company;

        return await query.ToListAsync();
    }
}