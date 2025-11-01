using FeedbackService.DAL.Data;
using FeedbackService.DAL.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using WorkerService.BLL.Events;

namespace FeedbackService.BLL.Consumers;

public class ResumeDeletedEventConsumer : IConsumer<ResumeDeletedEvent>
{
    private readonly IResumeRepository _repository;
    private readonly ILogger<ResumeDeletedEventConsumer> _logger;

    public ResumeDeletedEventConsumer(IResumeRepository repository, ILogger<ResumeDeletedEventConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<ResumeDeletedEvent> context)
    {
        try
        {
            Guid resumeId = context.Message.resumeId;
            await _repository.DeleteResumeAsync(resumeId);
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured by ResumeDeletedEventConsumer" + e.Message);
            throw;
        }
    }
}