using System.Text;
using System.Text.Json;
using ApiGateway.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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
builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddReverseProxy().
    LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


string publicRoutesJson = File.ReadAllText("public_routes.json");
PublicRoutesConfig? publicRoutesConfig = JsonSerializer.Deserialize<PublicRoutesConfig>(publicRoutesJson);
app.Use(async (context, next) =>
{
    string? path = context.Request.Path.Value?.ToLower();

    if (publicRoutesConfig.PublicRoutes.Any(p => path.StartsWith(p)))
    {
        await next();
        return;
    }

    if (!context.User.Identity!.IsAuthenticated)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
        return;
    }

    await next();
});

app.MapReverseProxy();


app.Run();