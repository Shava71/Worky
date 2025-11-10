using FeedbackService.BLL.Events;
using FeedbackService.BLL.Services.Interfaces;
using FeedbackService.DAL.Entities;
using FeedbackService.DAL.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FeedbackService.BLL.Consumers;

public class VacancyCreatedEventConsumer : IConsumer<VacancyCreatedEvent>
{
    private readonly IVacancyRepository _repository;
    private readonly ILogger<VacancyCreatedEventConsumer> _logger;

    public VacancyCreatedEventConsumer(IVacancyRepository vacancyRepository, ILogger<VacancyCreatedEventConsumer> logger)
    {
        _repository = vacancyRepository;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<VacancyCreatedEvent> context)
    {
        try
        {
            Vacancy resume = new Vacancy()
            {
                vacancyId = context.Message.Vacancy.id,
                companyId = context.Message.Vacancy.company_id,
            };
            
            await _repository.AddVacancyAsync(resume);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}