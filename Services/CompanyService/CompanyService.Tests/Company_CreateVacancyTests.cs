using CompanyService.BLL.Services.Http.Interfaces;
using CompanyService.BLL.Services.Interfaces;
using CompanyService.DAL.Contracts;
using CompanyService.DAL.DTO;
using CompanyService.DAL.Entities;
using CompanyService.DAL.Events;
using CompanyService.DAL.HttpClients.Clients;
using CompanyService.DAL.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace CompanyService.Tests;

public class Company_CreateVacancyTests
{
    private readonly Mock<IVacancyRepository> _vacancyRepo = new();
    private readonly Mock<IDealRepository> _dealRepo = new();
    private readonly Mock<ITopicProducer<VacancyCreatedEvent>> _producer = new();
    private readonly Mock<ILogger<BLL.Services.Implementations.CompanyService>> _logger = new();

    private readonly Mock<ICompanyRepository> _companyRepo = new();
    private readonly Mock<IFilterCacheService> _filterCache = new();
    private readonly Mock<IAuthClient> _authClient = new();

    private BLL.Services.Implementations.CompanyService CreateService()
    {
        return new BLL.Services.Implementations.CompanyService(
            _vacancyRepo.Object,
            _companyRepo.Object,
            _logger.Object,
            _authClient.Object,
            _filterCache.Object,
            _dealRepo.Object,
            _producer.Object,
            Mock.Of<ITopicProducer<VacancyUpdatedEvent>>(),
            Mock.Of<ITopicProducer<VacancyDeletedEvent>>(),
            Mock.Of<ITopicProducer<VacancyFilterAddEvent>>(),
            Mock.Of<ITopicProducer<VacancyFilterDeleteEvent>>()
        );
    }

    [Fact]
    public async Task CreateVacancyAsync_Success()
    {
        // Arrange
        Guid companyId = Guid.NewGuid();
        Guid vacancyId = Guid.NewGuid();
        CreateVacancy createDto = new CreateVacancy
        (
            post: "Senior .NET Developer",
            description: "Разработка backend",
            min_salary: 180000,
            max_salary: null,
            education_id: null,
            work_format: null,
            work_hour: null,
            experience: null
        );
        
        _companyRepo
            .Setup(r => r.GetCompanyByIdAsync(companyId))
            .ReturnsAsync(new Company
            {
                UserId = companyId,
                name = "Test Company",
                email = "test@test.com",
                phoneNumber = null,
                website = null,
                latitude = null,
                longitude = null
            });


        // активный договор с лимитом 10 вакансий
        _dealRepo
            .Setup(r => r.CurrentActiveDealAsync(It.IsAny<DateOnly>(), companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Deal
            {
                tariff = new Tarrif() { vacancy_count = 10 }
            });

        _vacancyRepo
            .Setup(r => r.GetMyVacanciesCountAsync(companyId))
            .ReturnsAsync(4);

        _vacancyRepo
            .Setup(r => r.CreateVacancyAsync(createDto, companyId.ToString()))
            .ReturnsAsync(vacancyId);

        _vacancyRepo
            .Setup(r => r.GetVacancyByIdAsync(vacancyId))
            .ReturnsAsync(new VacancyDtos
            {
                id = vacancyId,
                post = createDto.post,
                company_id = companyId,
                activities = new List<TypeOfActivityResponse>() 
            });
        
        

        BLL.Services.Implementations.CompanyService service = CreateService();

        // Act
        Guid result = await service.CreateVacancyAsync(createDto, companyId.ToString());

        // Assert
        Assert.Equal(vacancyId, result);
    }

    [Fact]
    public async Task CreateVacancyAsync_Failure()
    {
        // Arrange
        Guid companyId = Guid.NewGuid();
        CreateVacancy createDto = new CreateVacancy
        (
            post: "Senior .NET Developer",
            description: "Разработка backend",
            min_salary: 180000,
            max_salary: null,
            education_id: null,
            work_format: null,
            work_hour: null,
            experience: null
        );

        _dealRepo
            .Setup(r => r.CurrentActiveDealAsync(It.IsAny<DateOnly>(), companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Deal
            {
                tariff = new Tarrif { vacancy_count = 5 }
            });

        // Уже создано 5 вакансий 
        _vacancyRepo
            .Setup(r => r.GetMyVacanciesCountAsync(companyId))
            .ReturnsAsync(5);

        BLL.Services.Implementations.CompanyService service = CreateService();

        var ex = await Assert.ThrowsAsync<Exception>(async () =>
            await service.CreateVacancyAsync(createDto, companyId.ToString()));
         
        _vacancyRepo.Verify(r => r.CreateVacancyAsync(It.IsAny<CreateVacancy>(), It.IsAny<string>()), Times.Never());

        _producer.Verify(p => p.Produce(It.IsAny<VacancyCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never());
    }
}