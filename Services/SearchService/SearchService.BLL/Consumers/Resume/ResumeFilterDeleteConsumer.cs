using MassTransit;
using SearchService.DAL.Entities;
using SearchService.DAL.Events;
using SearchService.DAL.HttpClients.Clients;
using SearchService.DAL.Repositories.Implementations;

namespace SearchService.BLL.Consumers;

public class ResumeFilterDeleteConsumer : IConsumer<ResumeFilterDeleteEvent>
{
    private readonly ResumeElasticRepository _repository;

    public ResumeFilterDeleteConsumer(ResumeElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ResumeFilterDeleteEvent> context)
    {
        string resumeId = context.Message.resume_id.ToString();
        int activityId = context.Message.activity_id;

        try
        {
            ResumeDocument document = await _repository.GetByIdAsync(resumeId);
            if (document is null)
            {

                document.activities.RemoveAll(a => a.id == activityId);

                await _repository.UpdateAsync(resumeId, document);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}