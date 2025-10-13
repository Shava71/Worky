using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Kafka;

public class KafkaProducerFactory
{
    private readonly ProducerConfig _producerConfig;

    public KafkaProducerFactory(IConfiguration configuraiton)
    {
        _producerConfig = new ProducerConfig
        {
            BootstrapServers = configuraiton["Kafka:BootstrapServers"],
            EnableIdempotence = true,
            Acks = Acks.All,
            // TransactionalId = configuraiton["Kafka:TransactionalId"] ?? "auth-producer-1",
            TransactionalId = $"authservice-{Guid.NewGuid()}",
            // дополнительные настройки
            MessageSendMaxRetries = 5,
            RetryBackoffMs = 200
        };
    }

    public IProducer<string, string> CreateProducer()
    {
        IProducer<string, string> producer = new ProducerBuilder<string, string>(_producerConfig).Build();
        producer.InitTransactions(TimeSpan.FromSeconds(10));
        return producer;
    }
}