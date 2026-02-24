using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Transport;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using SearchService.Api.Extensions;
using SearchService.BLL.Consumers;
using SearchService.BLL.Events;
using SearchService.BLL.Services.Implementations;
using SearchService.BLL.Services.Interfaces;
using SearchService.DAL;
using SearchService.DAL.Embenddings;
using SearchService.DAL.Entities;
using SearchService.DAL.Events;

var builder = WebApplication.CreateBuilder(args);

// ElasticSearch con
string elasticsearchUrl = builder.Configuration.GetValue<string>("Elasticsearch:Url") 
                       ?? "http://localhost:9200";
var username = builder.Configuration.GetValue<string>("Elasticsearch:Username") 
               ?? "elastic";
var password = builder.Configuration.GetValue<string>("Elasticsearch:Password") 
               ?? "changeme";

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<SearchDbContext>(options =>
    options.UseNpgsql(connectionString)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors());

var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUrl))
    .Authentication(new BasicAuthentication(username, password))
    .DefaultIndex("default-index")
    .EnableDebugMode()
    .PrettyJson()
    .DefaultFieldNameInferrer(p => p)
    .DefaultMappingFor<VacancyDocument>(m => m
        .IndexName("vacancies")
        .IdProperty(p => p.id)
    )
    .DefaultMappingFor<ResumeDocument>(m => m
        .IndexName("resumes")
        .IdProperty(p => p.id)
    );



ElasticsearchClient client = new ElasticsearchClient(settings);


builder.Services.AddSingleton(client);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    try
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Worky", Version = "v1" });
    }
    catch (Exception ex)
    {
        Console.WriteLine("Swagger ex: " + ex.Message);
        throw;
    }
});

builder.Services.AddElasticRepositories();
builder.Services.AddElasticServices();

builder.Services.Configure<EmbeddingOptions>(
    builder.Configuration.GetSection("Embeddings"));

builder.Services.AddHttpClient<IEmbeddingService, EmbeddingHttpService>(
    (sp, client) =>
    {
        var options = sp.GetRequiredService<
            Microsoft.Extensions.Options.IOptions<EmbeddingOptions>>().Value;

        client.BaseAddress = new Uri(options.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
    });

builder.Services.AddMassTransit(config =>
{
    config.AddInMemoryInboxOutbox();
    
    config.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context); 
    });
    
    config.AddRider(rider =>
    {
        rider.AddConsumer<ResumeCreatedConsumer>();
        rider.AddConsumer<ResumeDeletedConsumer>();
        rider.AddConsumer<ResumeFilterAddConsumer>();
        rider.AddConsumer<ResumeFilterDeleteConsumer>();
        rider.AddConsumer<ResumeUpdatedConsumer>();
        
        rider.AddConsumer<VacancyCreatedConsumer>();
        rider.AddConsumer<VacancyDeletedConsumer>();
        rider.AddConsumer<VacancyFilterAddConsumer>();
        rider.AddConsumer<VacancyFilterDeleteConsumer>();
        rider.AddConsumer<VacancyUpdatedConsumer>();

        
        rider.UsingKafka((context, k) =>
        {
            IConfigurationSection kafkaSettings = builder.Configuration.GetSection("Kafka");
            string bootstrapServers = kafkaSettings["BootstrapServers"];
            string groupId = kafkaSettings["GroupId"];
            k.Host(bootstrapServers);
            
            
            k.TopicEndpoint<ResumeCreatedEvent>("resume.created", groupId, e =>
            {
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<ResumeCreatedConsumer>(context);
                e.CreateIfMissing();
            });
            k.TopicEndpoint<ResumeDeletedEvent>("resume.deleted", groupId, e =>
            {
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<ResumeDeletedConsumer>(context);
                e.CreateIfMissing();
            });
            k.TopicEndpoint<ResumeFilterAddEvent>("resume.filter.add", groupId, e =>
            {
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<ResumeFilterAddConsumer>(context);
                e.CreateIfMissing();
            });
            k.TopicEndpoint<ResumeFilterDeleteEvent>("resume.filter.delete", groupId, e =>
            {
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<ResumeFilterDeleteConsumer>(context);
                e.CreateIfMissing();
            });
            k.TopicEndpoint<ResumeUpdatedEvent>("resume.updated", groupId, e =>
            {
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<ResumeUpdatedConsumer>(context);
                e.CreateIfMissing();
            });
            
            
            
            k.TopicEndpoint<VacancyCreatedEvent>("vacancy.created", groupId, e =>
            {
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<VacancyCreatedConsumer>(context);
                e.CreateIfMissing();
            });
            k.TopicEndpoint<VacancyDeletedEvent>("vacancy.deleted", groupId, e =>
            {
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<VacancyDeletedConsumer>(context);
                e.CreateIfMissing();
            });
            k.TopicEndpoint<VacancyFilterAddEvent>("vacancy.filter.add", groupId, e =>
            {
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<VacancyFilterAddConsumer>(context);
                e.CreateIfMissing();
            });
            k.TopicEndpoint<VacancyFilterDeleteEvent>("vacancy.filter.delete", groupId, e =>
            {
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<VacancyFilterDeleteConsumer>(context);
                e.CreateIfMissing();
            });
            k.TopicEndpoint<VacancyUpdatedEvent>("vacancy.updated", groupId, e =>
            {
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<VacancyUpdatedConsumer>(context);
                e.CreateIfMissing();
            });
        });
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var app = builder.Build();

app.UseCors();
app.UseRouting();
app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkerService API V1");
    c.RoutePrefix = string.Empty;
});

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<SearchDbContext>();
    context.Database.Migrate();
}

app.Run();


