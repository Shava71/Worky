using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AuthService.Api.Extentions;
using AuthService.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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

// DbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
var signingKey = new SymmetricSecurityKey(key);
builder.Services.AddSingleton(signingKey);
builder.Services.AddAuthServices();


builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService API V1");
    c.RoutePrefix = string.Empty;
});

app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
}

app.Run();
