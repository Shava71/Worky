using MassTransit;
using SearchService.BLL.Events;
using SearchService.BLL.Mapping;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Implementations;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.BLL.Consumers;

public class ResumeCreatedConsumer : IConsumer<ResumeCreatedEvent>
{
    private readonly IResumeElasticRepository _repository;

    public ResumeCreatedConsumer(IResumeElasticRepository repository)
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