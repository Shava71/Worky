using CompanyService.DAL.Entities;
using CompanyService.DAL.Events;
using CompanyService.DAL.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;


namespace CompanyService.BLL.Consumers;

public class UserCompanyCreatedConsumer : IConsumer<UserCompanyCreatedEvent>
{
    private readonly ILogger<UserCompanyCreatedConsumer> _logger;
    private readonly ICompanyRepository _companyRepository;
    private readonly ITopicProducer<UserCompanyCreateFailedEvent> _publishEndpoint;

    public UserCompanyCreatedConsumer(
        ILogger<UserCompanyCreatedConsumer> logger,
        ICompanyRepository companyRepository,
        ITopicProducer<UserCompanyCreateFailedEvent> publishEndpoint
        )
    {
        _logger = logger;
        _companyRepository = companyRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<UserCompanyCreatedEvent> context)
    {
        UserCompanyCreatedEvent message = context.Message;

        try
        {
            Company? worker = await _companyRepository.GetCompanyByIdAsync(Guid.Parse(message.UserId));
            if (worker != null)
            {
                _logger.LogInformation($"Company with id {message.UserId} has been created.");
                return;
            }

            Company newWorker = new Company()
            {
                UserId = Guid.Parse(message.UserId),
                name = message.name,
                email = message.email_info,
                phoneNumber = message.phone_info,
                longitude = message.longitude,
                latitude = message.latitude,
                website = message.website,
            };
            
            await _companyRepository.CreateCompanyAsync(newWorker);
            _logger.LogInformation("Company successfully created: {UserId}", message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await _publishEndpoint.Produce(new UserCompanyCreateFailedEvent() //rollback
            {
                UserId = message.UserId,
                Reason = ex.Message
            });
        }
    }
}