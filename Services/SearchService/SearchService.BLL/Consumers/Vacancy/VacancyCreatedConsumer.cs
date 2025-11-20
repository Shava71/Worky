using MassTransit;
using SeachService.DAL.DTO;
using SearchService.BLL.Mapping;
using SearchService.DAL.Entities;
using SearchService.DAL.Events;
using SearchService.DAL.Repositories.Implementations;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.BLL.Consumers;

public class VacancyCreatedConsumer : IConsumer<VacancyCreatedEvent>
{
    private readonly IVacancyElasticRepository _repository;

    public VacancyCreatedConsumer(IVacancyElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<VacancyCreatedEvent> context)
    {
        VacancyDtos dto = context.Message.Vacancy;

        VacancyDocument document = dto.ToDocument();

        await _repository.IndexAsync(document.id.ToString(), document);
    }
}