using WorkerService.DAL.HttpClients.Clients;

namespace WorkerService.BLL.Services.Http.Interfaces;

public interface IFilterClient
{
    Task<List<TypeOfActivityResponse?>> GetFiltersByIdAsync(List<int> filterIds,
        CancellationToken cancellationToken = default);
}