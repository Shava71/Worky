using CompanyService.DAL.Data;
using CompanyService.DAL.Entities;
using CompanyService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Worky.Repositories.Implementations;

public class DealRepository : IDealRepository
{
    private readonly CompanyDbContext _context;

    public DealRepository(CompanyDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> CreateDeal(Deal deal, CancellationToken cancellationToken)
    {
        await _context.deal.AddAsync(deal, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return deal.id;
    }

    public async Task<Deal?> GetDealById(int dealId, CancellationToken cancellationToken)
    {
        return await _context.deal.FindAsync(dealId);
    }

    public async Task<Deal?> CurrentActiveDealAsync(DateOnly currentDate, Guid companyId, CancellationToken cancellationToken)
    {
        return await _context.deal
            .Include(d => d.tariff)
            .Where(d =>
                d.date_start <= currentDate &&
                currentDate <= d.date_end &&
                d.company_id == companyId)
            .OrderByDescending(d => d.date_start)
            .FirstOrDefaultAsync(cancellationToken);
    }
}