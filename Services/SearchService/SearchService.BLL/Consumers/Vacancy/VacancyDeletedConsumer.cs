using MassTransit;
using SearchService.DAL.Events;
using SearchService.DAL.Repositories.Implementations;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.BLL.Consumers;

public class VacancyDeletedConsumer : IConsumer<VacancyDeletedEvent>
{
    private readonly IVacancyElasticRepository _repository;

    public VacancyDeletedConsumer(IVacancyElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<VacancyDeletedEvent> context)
    {
        string id = context.Message.vacancyId.ToString();
        await _repository.DeleteAsync(id);
    }
}