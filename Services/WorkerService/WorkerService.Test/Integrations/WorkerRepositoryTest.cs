using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WorkerService.DAL.Data;
using WorkerService.DAL.Entities;
using WorkerService.DAL.Repositories.Implementations;

namespace WorkerService.Test.Integrations;

public class WorkerRepositoryTest
{
    private readonly WorkerDbContext _context;
    private readonly WorkerRepository _repository;

    public WorkerRepositoryTest()
    {
        DbContextOptions<WorkerDbContext> options = new DbContextOptionsBuilder<WorkerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // изолированная база
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        _context = new WorkerDbContext(options);
        _repository = new WorkerRepository(_context);
    }

    [Fact]
    public async Task CreateWorker_ShouldAddWorkerToDatabase()
    {
        // Arrange
        Worker worker = new Worker
        {
            UserId = Guid.NewGuid(),
            first_name = "Иван",
            second_name = "Иванов",
            surname = "Иванович",
            birthday = new DateOnly(1990, 1, 1)
        };

        // Act
        await _repository.CreateWorkerAsync(worker);
        await _context.SaveChangesAsync();

        // Assert
        Worker found = await _context.Worker.FirstOrDefaultAsync(w => w.UserId == worker.UserId);
        found.Should().NotBeNull();
        found!.first_name.Should().Be("Иван");
    }
}