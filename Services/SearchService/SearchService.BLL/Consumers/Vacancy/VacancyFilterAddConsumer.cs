using MassTransit;
using SearchService.DAL.Entities;
using SearchService.DAL.Events;
using SearchService.DAL.HttpClients.Clients;
using SearchService.DAL.Repositories.Implementations;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.BLL.Consumers;

public class VacancyFilterAddConsumer : IConsumer<VacancyFilterAddEvent>
{
    private readonly IVacancyElasticRepository _repository;

    public VacancyFilterAddConsumer(IVacancyElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<VacancyFilterAddEvent> context)
    {
        string vacancyId = context.Message.vacancy_id.ToString();
        List<TypeOfActivityResponse> activityResponse = context.Message.activities;

        try
        {
            VacancyDocument document = await _repository.GetByIdAsync(vacancyId);
            if (document is null)
            {
                return;
            }

            List<Activity> newActivities = activityResponse.Select(a => new Activity
            {
                id = a.id,
                direction = a.direction,
                type = a.type,
            }).ToList();
            
            document.activities.AddRange(newActivities);
            
            await _repository.UpdateAsync(vacancyId, document);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}