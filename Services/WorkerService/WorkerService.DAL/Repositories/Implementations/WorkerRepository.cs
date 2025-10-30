using Microsoft.EntityFrameworkCore;
using WorkerService.DAL.Data;
using WorkerService.DAL.DTO;
using WorkerService.DAL.Entities;
using WorkerService.DAL.Repositories.Interfaces;


namespace WorkerService.DAL.Repositories.Implementations;

public class WorkerRepository : IWorkerRepository
{
    private readonly WorkerDbContext _dbContext;

    public WorkerRepository(WorkerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateWorkerAsync(Worker worker)
    {
        await _dbContext.Worker.AddAsync(worker);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Worker> GetWorkerByIdAsync(Guid id)
    {
        Worker worker = await _dbContext.Worker.FindAsync(id);
        return worker;
        //byte[]? image = await _dbContext.Users.Where(u => u.Id == id).Select(u => u.image).FirstOrDefaultAsync();
        // return new WorkerDtos
        // {
        //     id = worker.UserId.ToString(),
        //     first_name = worker.first_name,
        //     second_name = worker.second_name,
        //     surname = worker.surname,
        //     birthday = worker.birthday,
        //     image = image
        // };
    }

    public async Task UpdateWorkerAsync(Worker worker)
    {
        _dbContext.Worker.Update(worker);
        await _dbContext.SaveChangesAsync();
    }
}