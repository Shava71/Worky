using FeedbackService.BLL.Events;
using FeedbackService.DAL.Entities;
using FeedbackService.DAL.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FeedbackService.BLL.Consumers;

public class ResumeCreatedEventConsumer : IConsumer<ResumeCreatedEvent>
{
    private readonly IResumeRepository _repository;
    private readonly ILogger<ResumeCreatedEventConsumer> _logger;

    public ResumeCreatedEventConsumer(IResumeRepository repository, ILogger<ResumeCreatedEventConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<ResumeCreatedEvent> context)
    {
        try
        {
            Resume resume = new Resume()
            {
                resumeId = context.Message.Resume.id,
                workerId = Guid.Parse(context.Message.Resume.worker_id)
            };
            
            await _repository.AddResumeAsync(resume);
        }
        catch (Exception e)
        { 
            _logger.LogError(e, "Error occured my ResumeCreatedEventConsumer " + e.Message);
            throw;
        }
        
    }
}