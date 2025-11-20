using MassTransit;
using SearchService.BLL.Events;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Implementations;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.BLL.Consumers;

public class ResumeDeletedConsumer : IConsumer<ResumeDeletedEvent>
{
    private readonly IResumeElasticRepository _repository;

    public ResumeDeletedConsumer(IResumeElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ResumeDeletedEvent> context)
    {
        string id = context.Message.resumeId.ToString();
        await _repository.DeleteAsync(id);
    }
}