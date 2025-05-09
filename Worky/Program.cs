using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Worky.Context;
using Worky.Migrations;
using Worky.Services;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WorkyDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();
// builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
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
        Console.WriteLine("Swagger ex: "+ex.Message);
        throw;
    }
});

builder.Services.AddScoped<WorkyDbContext>();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
// builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
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
builder.Services.AddIdentity<Users, Roles>().AddEntityFrameworkStores<WorkyDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddSingleton<IJwtService, JwtService>();


builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
    });
});

var app = builder.Build();

// //Инициализация ролей в бд:
// using (var scope = app.Services.CreateScope())
// {
//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Roles>>();
//     var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("RoleInitialize");
//     await RoleInitialize.Initialize(roleManager, logger);
// }

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
app.UseStaticFiles();

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// app.MapRazorPages();


app.Run();