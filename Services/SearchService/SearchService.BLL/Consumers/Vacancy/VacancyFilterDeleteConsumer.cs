using MassTransit;
using SearchService.DAL.Entities;
using SearchService.DAL.Events;
using SearchService.DAL.Repositories.Implementations;

namespace SearchService.BLL.Consumers;

public class VacancyFilterDeleteConsumer : IConsumer<VacancyFilterDeleteEvent>
{
    private readonly VacancyElasticRepository _repository;

    public VacancyFilterDeleteConsumer(VacancyElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<VacancyFilterDeleteEvent> context)
    {
        string vacancyId = context.Message.vacancy_id.ToString();
        int activityId = context.Message.activity_id;

        try
        {
            VacancyDocument document = await _repository.GetByIdAsync(vacancyId);
            if (document is null)
            {

                document.activities.RemoveAll(a => a.id == activityId);

                await _repository.UpdateAsync(vacancyId, document);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}