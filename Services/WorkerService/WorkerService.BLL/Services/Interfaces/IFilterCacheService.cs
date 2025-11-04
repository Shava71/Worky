using WorkerService.DAL.HttpClients.Clients;

namespace WorkerService.BLL.Services.Interfaces;

public interface IFilterCacheService
{
    Task<List<TypeOfActivityResponse>?> GetFiltersByIdsAsync(List<int> ids);
}