
using CompanyService.DAL.HttpClients.Clients;

namespace CompanyService.BLL.Services.Interfaces;

public interface IFilterCacheService
{
    Task<List<TypeOfActivityResponse>?> GetFiltersByIdsAsync(List<int> ids);
}