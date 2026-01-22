using CompanyService.DAL.Contracts;
using CompanyService.DAL.Entities;

namespace CompanyService.BLL.Services.Interfaces;

public interface IDealService
{
    public Task<Guid> CreateDeal(MakeDealRequest request, Guid company_id, CancellationToken cancellationToken = default);
    public Task<Tarrif> GetTariff(int? dealId, CancellationToken cancellationToken = default);
    public Task<List<Tarrif>> GetTariff(CancellationToken cancellationToken = default);
}