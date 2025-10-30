using AuthService.Application.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Worker;

public class OutboxPublisWorker : BackgroundService
{
    private readonly ILogger<OutboxPublisWorker> _logger;
    private readonly IOutboxPublisher _outboxPublisher;

    public OutboxPublisWorker(IOutboxPublisher outboxPublisher, ILogger<OutboxPublisWorker> logger)
    {
        _logger = logger;
        _outboxPublisher = outboxPublisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxPublisWorker starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _outboxPublisher.PublishPendingMessagesAsync(stoppingToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in OutboxPublisherWorker cycle");
            }
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}