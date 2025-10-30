using System.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WorkerService.DAL.Contracts;
using WorkerService.DAL.Data;
using WorkerService.DAL.Data.DbConnection.Interface;
using WorkerService.DAL.Entities;
using WorkerService.DAL.Repositories.Implementations;
using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;

namespace WorkerService.Test.Integrations;

public class ResumeRepositoryTest
{
    
    private readonly WorkerDbContext _context;
    private readonly ResumeRepository _repository;

    public ResumeRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<WorkerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        _context = new WorkerDbContext(options);

        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ResumeRepository>();
        var dbcon = new Mock<IDbConnectionFactory>();


        _repository = new ResumeRepository(_context, config, logger, dbcon.Object);
    }
    
    [Fact]
    public async Task GetResumesAsync_ShouldReturnEmpty_WhenNoResumeFiltersExist()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var createTablesSql = @"
            CREATE TABLE ""Resume"" (
                id TEXT PRIMARY KEY,
                worker_id TEXT,
                post TEXT,
                skill TEXT,
                city TEXT,
                experience INTEGER,
                education_id INTEGER,
                wantedSalary INTEGER,
                income_date TEXT
            );
            CREATE TABLE ""Resume_filter"" (
                filter_id TEXT PRIMARY KEY,
                resume_id TEXT,
                typeOfActivity_id INTEGER
            );
        ";
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = createTablesSql;
            await cmd.ExecuteNonQueryAsync();
        }

        var insertSql = """
            INSERT INTO "Resume" (id, worker_id, post, skill, city, experience, education_id, wantedSalary, income_date)
            VALUES (@id, @worker_id, @post, @skill, @city, @experience, @education_id, @wantedSalary, @income_date);
        """;
        var resumeId = Guid.NewGuid();
        await connection.ExecuteAsync(insertSql, new
        {
            id = resumeId,
            worker_id = Guid.NewGuid(),
            post = "Backend Developer",
            skill = "C#, ASP.NET Core",
            city = "Moscow",
            experience = 3,
            education_id = 1,
            wantedSalary = 150000,
            income_date = DateTime.UtcNow
        });
        
        var dbcon = new Mock<IDbConnectionFactory>();
        dbcon.Setup(f => f.CreateConnection()).Returns(connection);

        var config = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<ResumeRepository>>();
        var repo = new ResumeRepository(_context, config.Object, logger.Object, dbcon.Object);

        var request = new GetResumesRequest(
            id: null,
            min_experience: null,
            max_experience: null,
            education: null,
            city: null,
            income_date: null,
            min_wantedSalary: null,
            max_wantedSalary: null,
            Order: null,
            SortItem: null,
            type: null,
            direction: null
        );

        // Act
        var result = await repo.GetResumesAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().city.Should().Be("Moscow");
    }

    [Fact]
    public async Task GetResumesAsync_ShouldReturnResumes_WhenFiltersExist()
    {
        // Arrange
        var dbContext = _context;

        var resumeId = Guid.NewGuid();
        var resume = new Resume
        {
            id = resumeId,
            worker_id = Guid.NewGuid(),
            post = "QA Engineer",
            skill = "Manual, Selenium",
            city = "SPB",
            experience = 2,
            education_id = 1,
            wantedSalary = 100000,
            income_date = DateTime.UtcNow
        };
        dbContext.Resume.Add(resume);

        var filter = new Resume_filter
        {
            filter_id = Guid.NewGuid(),
            resume_id = resumeId,
            typeOfActivity_id = 1
        };
        dbContext.Resume_filter.Add(filter);

        await dbContext.SaveChangesAsync();

        var logger = new Mock<ILogger<ResumeRepository>>();
        var config = new Mock<IConfiguration>();
        var dbcon = new Mock<IDbConnectionFactory>();
        var repo = new ResumeRepository(dbContext, config.Object, logger.Object, dbcon.Object);

        var request = new GetResumesRequest();

        // Act
        var result = await repo.GetResumesAsync(request);

        // Assert
        result.Should().NotBeEmpty("так как у резюме есть связанный фильтр");
        result.Should().ContainSingle();
        result.First().post.Should().Be("QA Engineer");
    }
    
    [Fact]
    public async Task GetResumeByIdAsync_ShouldReturnResume_WhenExists()
    {
        // Arrange
        var worker = new Worker
        {
            UserId = Guid.NewGuid(),
            first_name = "Иван",
            second_name = "Иванов",
            surname = "Петров",
            birthday = new DateOnly(1990, 1, 1)
        };

        var resume = new Resume
        {
            id = Guid.NewGuid(),
            worker = worker,
            post = "Разработчик",
            skill = "C#, SQL",
            city = "Москва",
            experience = 3,
            wantedSalary = 150000,
            income_date = DateTime.UtcNow
        };

        _context.Worker.Add(worker);
        _context.Resume.Add(resume);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetResumeByIdAsync(resume.id);

        // Assert
        result.Should().NotBeNull();
        result!.post.Should().Be("Разработчик");
        result.worker.first_name.Should().Be("Иван");
        result.city.Should().Be("Москва");
    }
    
    [Fact]
    public async Task CreateResumeAsync_ShouldInsertAndReturnId()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Port=5436;Database=workerdb;Username=workeruser;Password=workerpass"
            })
            .Build();

        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ResumeRepository>();
        var dbcon = new Mock<IDbConnectionFactory>();
        
        var repository = new ResumeRepository(_context, config, logger, dbcon.Object);

        var resume = new CreateResume(
            post: "QA Engineer",
            skill: "Testing, Selenium",
            city: "Kazan",
            experience: 2,
            education_id: 1,
            wantedSalary: 80000
        );

        // Act
        Guid guid = Guid.Parse("7db17c00-d045-49da-827b-22ed52d66919");
        var newId = await repository.CreateResumeAsync(resume, guid.ToString());

        // Assert
        newId.Should().NotBe(Guid.Empty);
    }
}