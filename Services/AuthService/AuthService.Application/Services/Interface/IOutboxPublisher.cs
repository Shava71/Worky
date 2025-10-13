namespace AuthService.Application.Services;

public interface IOutboxPublisher
{
    Task PublishPendingMessagesAsync(CancellationToken cancellationToken);
}