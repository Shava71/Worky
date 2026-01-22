using CompanyService.DAL.Data;
using CompanyService.DAL.Entities;
using CompanyService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Worky.Repositories.Implementations;

public class TariffRepository : ITariffRepository
{
    private readonly CompanyDbContext _context;

    public TariffRepository(CompanyDbContext companyDbContext)
    {
        _context = companyDbContext;
    }
    
    public async Task<Tarrif?> GetTariff(int? tarrif_id, CancellationToken cancellationToken)
    {
        return await _context.tariff.FirstOrDefaultAsync(t => t.id == tarrif_id, cancellationToken);
    }

    public async Task<List<Tarrif?>> GetTariff(CancellationToken cancellationToken)
    {
        return await _context.tariff.ToListAsync(cancellationToken);
    }
}