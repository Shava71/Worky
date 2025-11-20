using MassTransit;
using SeachService.DAL.DTO;
using SearchService.BLL.Mapping;
using SearchService.DAL.Entities;
using SearchService.DAL.Events;
using SearchService.DAL.Repositories.Implementations;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.BLL.Consumers;

public class VacancyUpdatedConsumer : IConsumer<VacancyUpdatedEvent>
{
    private readonly IVacancyElasticRepository _repository;

    public VacancyUpdatedConsumer(IVacancyElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<VacancyUpdatedEvent> context)
    {
        VacancyDtos dto = context.Message.Vacancy;

        VacancyDocument document = dto.ToDocument();

        await _repository.UpdateAsync(document.id.ToString(), document);
    }
}