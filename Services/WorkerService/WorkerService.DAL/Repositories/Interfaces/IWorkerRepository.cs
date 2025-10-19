using WorkerService.DAL.DTO;
using WorkerService.DAL.Entities;


namespace WorkerService.DAL.Repositories.Interfaces;

public interface IWorkerRepository
{
    Task CreateWorkerAsync(Worker worker);
    Task<WorkerDtos> GetWorkerByIdAsync(string id);
    Task UpdateWorkerAsync(Worker worker);
}