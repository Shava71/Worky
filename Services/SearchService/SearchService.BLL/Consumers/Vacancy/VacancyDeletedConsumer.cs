using MassTransit;
using SearchService.DAL.Events;
using SearchService.DAL.Repositories.Implementations;

namespace SearchService.BLL.Consumers;

public class VacancyDeletedConsumer : IConsumer<VacancyDeletedEvent>
{
    private readonly VacancyElasticRepository _repository;

    public VacancyDeletedConsumer(VacancyElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<VacancyDeletedEvent> context)
    {
        string id = context.Message.vacancyId.ToString();
        await _repository.DeleteAsync(id);
    }
}