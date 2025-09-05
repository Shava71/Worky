using Worky.DTO;
using Worky.Migrations;

namespace Worky.Repositories.Interfaces;

public interface IWorkerRepository
{
    Task CreateWorkerAsync(Worker worker);
    Task<WorkerDtos> GetWorkerByIdAsync(string id);
    Task UpdateWorkerAsync(Worker worker);
}