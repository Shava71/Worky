
using CompanyService.DAL.HttpClients.Clients;

namespace CompanyService.BLL.Services.Http.Interfaces;

public interface IFilterClient
{
    Task<List<TypeOfActivityResponse?>> GetFiltersByIdAsync(List<int> filterIds,
        CancellationToken cancellationToken = default);
}