using CompanyService.DAL.Entities;

namespace CompanyService.DAL.Repositories.Interfaces;

public interface IDealRepository
{
    public Task<Guid> CreateDeal(Deal deal, CancellationToken cancellationToken = default);
    public Task<Deal?> GetDealById(int dealId, CancellationToken cancellationToken = default);
    public Task<List<Deal?>> GetDealsByCompanyId(Guid companyId, CancellationToken cancellationToken = default);
    public Task<Deal?> CurrentActiveDealAsync(DateOnly currentDate,Guid companyId, CancellationToken cancellationToken = default);
}