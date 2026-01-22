using CompanyService.DAL.Entities;

namespace CompanyService.DAL.Repositories.Interfaces;

public interface ITariffRepository
{
    public Task<Tarrif?> GetTariff(int? tarrif_id, CancellationToken cancellationToken);
    public Task<List<Tarrif?>> GetTariff(CancellationToken cancellationToken);
}