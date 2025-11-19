using MassTransit;
using SearchService.BLL.Events;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Implementations;

namespace SearchService.BLL.Consumers;

public class ResumeDeletedConsumer : IConsumer<ResumeDeletedEvent>
{
    private readonly ResumeElasticRepository _repository;

    public ResumeDeletedConsumer(ResumeElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ResumeDeletedEvent> context)
    {
        string id = context.Message.resumeId.ToString();
        await _repository.DeleteAsync(id);
    }
}