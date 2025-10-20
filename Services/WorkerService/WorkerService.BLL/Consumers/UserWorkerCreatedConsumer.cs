using MassTransit;
using Microsoft.Extensions.Logging;
using WorkerService.BLL.Events;
using WorkerService.DAL.Entities;
using WorkerService.DAL.Repositories.Interfaces;

namespace WorkerService.BLL.Consumers;

public class UserWorkerCreatedConsumer : IConsumer<UserWorkerCreatedEvent>
{
    private readonly ILogger<UserWorkerCreatedConsumer> _logger;
    private readonly IWorkerRepository _workerRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UserWorkerCreatedConsumer(
        ILogger<UserWorkerCreatedConsumer> logger,
        IWorkerRepository workerRepository,
        IPublishEndpoint publishEndpoint
        )
    {
        _logger = logger;
        _workerRepository = workerRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<UserWorkerCreatedEvent> context)
    {
        UserWorkerCreatedEvent message = context.Message;

        try
        {
            Worker? worker = await _workerRepository.GetWorkerByIdAsync(Guid.Parse(message.UserId));
            if (worker != null)
            {
                _logger.LogInformation($"Worker with id {message.UserId} has been created.");
                return;
            }

            Worker newWorker = new Worker()
            {
                UserId = Guid.Parse(message.UserId),
                birthday = message.birthday,
                first_name = message.first_name,
                surname = message.surname,
                second_name = message.second_name,
            };
            
            await _workerRepository.CreateWorkerAsync(newWorker);
            _logger.LogInformation("Worker successfully created: {UserId}", message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await _publishEndpoint.Publish(new UserWorkerCreateFailedEvent //rollback
            {
                UserId = message.UserId,
                Reason = ex.Message
            });
        }
    }
}