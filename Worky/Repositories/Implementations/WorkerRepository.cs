using Microsoft.EntityFrameworkCore;
using Worky.Context;
using Worky.DTO;
using Worky.Migrations;

namespace Worky.Repositories.Implementations;

public class WorkerRepository
{
    private readonly WorkyDbContext _dbContext;

    public WorkerRepository(WorkyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateWorkerAsync(Worker worker)
    {
        await _dbContext.Workers.AddAsync(worker);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<WorkerDtos> GetWorkerByIdAsync(string id)
    {
        var worker = await _dbContext.Workers.FindAsync(id);
        byte[]? image = await _dbContext.Users.Where(u => u.Id == id).Select(u => u.image).FirstOrDefaultAsync();
        return new WorkerDtos
        {
            id = worker.id,
            first_name = worker.first_name,
            second_name = worker.second_name,
            surname = worker.surname,
            birthday = worker.birthday,
            image = image
        };
    }

    public async Task UpdateWorkerAsync(Worker worker)
    {
        _dbContext.Workers.Update(worker);
        await _dbContext.SaveChangesAsync();
    }
}