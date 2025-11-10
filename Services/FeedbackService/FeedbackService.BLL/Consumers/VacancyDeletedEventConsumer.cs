using FeedbackService.BLL.Events;
using FeedbackService.DAL.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FeedbackService.BLL.Consumers;

public class VacancyDeletedEventConsumer : IConsumer<VacancyDeletedEvent>
{
    private readonly IVacancyRepository _repository;
    private readonly ILogger<VacancyDeletedEventConsumer> _logger;

    public VacancyDeletedEventConsumer(IVacancyRepository repository, ILogger<VacancyDeletedEventConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<VacancyDeletedEvent> context)
    {
        try
        {
            Guid resumeId = context.Message.vacancyId;
            await _repository.DeleteVacancyAsync(resumeId);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured by ResumeDeletedEventConsumer" + e.Message);
            throw;
        }
    }
}