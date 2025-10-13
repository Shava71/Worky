using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Kafka;
using AuthService.Infrastructure.Outbox;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Services;

public class KafkaOutboxPublisher : IOutboxPublisher
{
    private readonly AuthDbContext _dbContext;
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaOutboxPublisher> _logger;

    public KafkaOutboxPublisher(AuthDbContext dbContext, KafkaProducerFactory kafkaProducerFactory, ILogger<KafkaOutboxPublisher> logger)
    {
        _dbContext = dbContext;
        _producer = kafkaProducerFactory.CreateProducer();
        _logger = logger;
    }
    
    public async Task PublishPendingMessagesAsync(CancellationToken cancellationToken)
    {
        List<OutboxMessage> pending = await _dbContext.OutboxMessage
            .Where(o => o.Sent == false)
            .OrderBy(o => o.OccurredAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        if (!pending.Any())
        {
            return;
        }
        _producer.BeginTransaction();
        
        bool success = true;

        foreach (OutboxMessage msg in pending)
        {
            try
            {
                await _producer.ProduceAsync(
                    msg.Topic,
                    new Message<string, string> {
                        Key = msg.Id.ToString(),
                        Value = msg.Payload },
                    cancellationToken
                );
                msg.MarkAsSent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish Outbox message {Id}", msg.Id);
                success = false;
                break;
            }

            if (success)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                _producer.CommitTransaction();
            }
            else
            {
                _producer.AbortTransaction();
            }
        }
    }
}