using Confluent.Kafka;
using MassTransit;
using SearchService.DAL.Entities;
using SearchService.DAL.Events;
using SearchService.DAL.HttpClients.Clients;
using SearchService.DAL.Repositories.Implementations;

namespace SearchService.BLL.Consumers;

public class ResumeFilterAddConsumer : IConsumer<ResumeFilterAddEvent>
{
    private readonly ResumeElasticRepository _repository;

    public ResumeFilterAddConsumer(ResumeElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ResumeFilterAddEvent> context)
    {
        string resumeId = context.Message.resume_id.ToString();
        List<TypeOfActivityResponse> activityResponse = context.Message.activities;

        try
        {
            ResumeDocument document = await _repository.GetByIdAsync(resumeId);
            if (document is null)
            {
                return;
            }

            List<Activity> newActivities = activityResponse.Select(a => new Activity
            {
                id = a.id,
                direction = a.direction,
                type = a.type,
            }).ToList();
            
            document.activities.AddRange(newActivities);
            
            await _repository.UpdateAsync(resumeId, document);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}