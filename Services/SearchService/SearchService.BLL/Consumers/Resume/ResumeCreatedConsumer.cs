using MassTransit;
using SearchService.BLL.Events;
using SearchService.BLL.Mapping;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Implementations;

namespace SearchService.BLL.Consumers;

public class ResumeCreatedConsumer : IConsumer<ResumeCreatedEvent>
{
    private readonly ResumeElasticRepository _repository;

    public ResumeCreatedConsumer(ResumeElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ResumeCreatedEvent> context)
    {
        ResumeDtos dto = context.Message.Resume;

        ResumeDocument document = dto.ToDocument();

        await _repository.IndexAsync(document.id.ToString(), document);
    }
}