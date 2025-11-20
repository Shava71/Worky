using System.Text;
using System.Text.Json;
using CompanyService.Api.Extentions;
using CompanyService.BLL.Consumers;
using CompanyService.DAL.Data;
using CompanyService.DAL.Events;
using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddDbContext<CompanyDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
var signingKey = new SymmetricSecurityKey(key);
builder.Services.AddSingleton(signingKey);
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        // options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // options.SaveToken = true;
        // options.RequireHttpsMetadata = false;   
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = builder.Configuration["Jwt:Audience"],
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.Headers.Add("Trace-Id", context.HttpContext.TraceIdentifier);
                return context.Response.WriteAsJsonAsync(new { message = "Необходимо авторизироваться" },
                    JsonSerializerOptions.Default);
            },
            OnForbidden = context =>
            {
                context.NoResult();
                context.Response.StatusCode = 403;
                context.Response.Headers.Add("Trace-Id", context.HttpContext.TraceIdentifier);
                return context.Response.WriteAsJsonAsync(new { message = "У вас нет доступа к данной странице" },
                    JsonSerializerOptions.Default);
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    try
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Worky", Version = "v1" });

        //Добавляем схему авторизации
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Введите токен в формате: Bearer {токен}",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        //Добавляем требование авторизации по умолчанию
        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine("Swagger ex: " + ex.Message);
        throw;
    }
});

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<UserCompanyCreatedConsumer>();
    
    config.AddEntityFrameworkOutbox<CompanyDbContext>(o =>
    {
        // o.QueryDelay = TimeSpan.FromSeconds(30);
        o.UsePostgres().UseBusOutbox();
    });
    
    config.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
    
    config.AddRider(rider =>
    {
        rider.AddProducer<UserCompanyCreateFailedEvent>("user.company.createfailed");
        rider.AddProducer<VacancyCreatedEvent>("vacancy.created");
        rider.AddProducer<VacancyUpdatedEvent>("vacancy.updated");
        rider.AddProducer<VacancyDeletedEvent>("vacancy.deleted");
        
        rider.AddProducer<VacancyFilterAddEvent>("vacancy.filter.add");
        rider.AddProducer<VacancyFilterDeleteEvent>("vacancy.filter.delete");
        
        rider.AddConsumer<UserCompanyCreatedConsumer>();
        
        rider.UsingKafka((context, k) =>
        {
            IConfigurationSection kafkaSettings = builder.Configuration.GetSection("Kafka");
            string bootstrapServers = kafkaSettings["BootstrapServers"];
            k.Host(bootstrapServers);
            
            
            k.TopicEndpoint<UserCompanyCreatedEvent>("user.company.created", "company-service-group", e =>
            {
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                e.ConfigureConsumer<UserCompanyCreatedConsumer>(context);
                e.CreateIfMissing();
            });
        });
    });
});
    
builder.Services.AddStackExchangeRedisCache(options =>
{
    string con = builder.Configuration.GetConnectionString("Redis");
    options.Configuration = con;
    options.InstanceName = "СompanyService";
});

builder.Services.AddCompanyService();
builder.Services.AddExternalHttpClient(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<CompanyDbContext>();
    context.Database.Migrate();
}

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



app.Run();

