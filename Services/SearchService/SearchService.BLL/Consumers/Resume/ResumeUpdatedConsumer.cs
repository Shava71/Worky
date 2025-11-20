using Confluent.Kafka;
using MassTransit;
using SearchService.BLL.Events;
using SearchService.BLL.Mapping;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Implementations;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.BLL.Consumers;

public class ResumeUpdatedConsumer : IConsumer<ResumeUpdatedEvent>
{
    private readonly IResumeElasticRepository _repository;

    public ResumeUpdatedConsumer(IResumeElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ResumeUpdatedEvent> context)
    {
        ResumeDtos dto = context.Message.Resume;

        ResumeDocument document = dto.ToDocument();

        await _repository.UpdateAsync(document.id.ToString(), document);
    }
}