using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Worky.Context;
using Worky.Contracts;
using Worky.DTO;
using Worky.Migrations;

namespace Worky.Controllers;


[Authorize(Roles = "Worker", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/v1/[controller]")]
public class WorkerController : Controller
{
    
    private readonly WorkyDbContext _dbContext;
    UserManager<Users> _userManager;
    ILogger<CompanyController> _logger;

    public WorkerController(WorkyDbContext dbContext, ILogger<CompanyController> logger, UserManager<Users> userManager)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userManager = userManager;
    }
    
    [AllowAnonymous]
    [HttpGet("Vacancies")]
    public async Task<IActionResult> FilterVacancy([FromQuery] GetVacanciesRequest? request)
    {
        try
        {
            var vacanciesQuery = _dbContext.Vacancies
                .Where(vacancy =>
                    _dbContext.Feedbacks
                        .Any(f => f.vacancy_id == vacancy.id && f.status == FeedbackStatus.Accepted))
                .Join(_dbContext.Vacancy_filters, // добавляем фильтры
                    vacancy => vacancy.id,
                    filter => filter.vacancy_id,
                    (vacancy, filter) => new {vacancy, filter})
                .Join(_dbContext.typeOfActivities, // добавляем имена к фильтрам
                    arg => arg.filter.typeOfActivity_id,
                    activity => activity.id,
                    (arg, activity) => new {arg.vacancy, arg.filter, activity}
                    )
                .Join(_dbContext.companies,
                    arg => arg.vacancy.company_id,
                    company => company.id,
                    (arg, company) => new {arg.vacancy, arg.filter, arg.activity, company})
                .AsNoTracking()
                .AsQueryable(); //Коллекция резюме
            // Поиск по id
            if (request.id.HasValue)
            {
                vacanciesQuery = vacanciesQuery
                    .Where(r => r.vacancy.id == request.id);
            }
            // Минимальный опыт работы
            if ((bool)request?.min_experience.HasValue)
            {
                vacanciesQuery = vacanciesQuery
                    .Where(r => r.vacancy.experience >= request.min_experience);
            }
            // Максимальный опыт работы
            if ((bool)request?.max_experience.HasValue)
            {
                vacanciesQuery = vacanciesQuery
                    .Where(r => r.vacancy.experience <= request.max_experience);
            }
            // // Сортировка по городу
            // if (!string.IsNullOrWhiteSpace(request?.city))
            // {
            //     resumesQuery = resumesQuery
            //         .Where(r => r.resume.city.ToLower().Contains(request.city.ToLower()));
            // }
            
            // Сортировка по минимальной желаемой зарплате
            if((bool)request?.min_wantedSalary.HasValue)
            {
                vacanciesQuery = vacanciesQuery
                    .Where(r => r.vacancy.min_salary >= request.min_wantedSalary
                    || r.vacancy.max_salary >= request.min_wantedSalary
                    );
            }
            // Сортировка по максимальной желаемой зарплате
            if ((bool)request?.max_wantedSalary.HasValue)
            {
                vacanciesQuery = vacanciesQuery
                    .Where(r => r.vacancy.min_salary <= request.max_wantedSalary
                    || r.vacancy.max_salary <= request.max_wantedSalary
                    );
            }
            // Сортировка по дате
            if (!string.IsNullOrWhiteSpace(request?.income_date.ToString()))
            {
                var date = request.income_date.Value.Date;
                var nextDate = date.AddDays(1);

                vacanciesQuery = vacanciesQuery
                    .Where(r => r.vacancy.income_date >= date && r.vacancy.income_date < nextDate);
            }
            // Сортировка по образованию
            if ((bool)request?.education.HasValue)
            {
                vacanciesQuery = vacanciesQuery
                    .Where(r => r.vacancy.education_id == (ulong?)request.education);
            }
            // Сортировка по виду деятельности
            if (!string.IsNullOrWhiteSpace(request?.type))
            {
                vacanciesQuery = vacanciesQuery
                    .Where(r => r.activity.type == request.type);
            }
            // Сортировка по направлению вида деятельности
            if (request?.direction is { Count: > 0 })
            {
                var vacanciesIdDirection = vacanciesQuery
                    .Where(r => request.direction!.Contains(r.activity.direction)).Select(r => r.vacancy.id).ToHashSet();
                // resumesQuery = resumesQuery
                //     .Where(r => request.direction!.Contains(r.activity.direction));
                vacanciesQuery = vacanciesQuery
                    .Where(r => vacanciesIdDirection.Contains(r.vacancy.id));
            }
            if (!string.IsNullOrEmpty(request.SortItem))
            {
                vacanciesQuery = request.Order?.ToLower() == "desc"
                    ? request.SortItem.ToLower() switch
                    {
                        // "city" => vacanciesQuery.OrderByDescending(x => x.vacancy.city),
                        "experience" => vacanciesQuery.OrderByDescending(x => x.vacancy.experience),
                        "income_date" => vacanciesQuery.OrderByDescending(x => x.vacancy.income_date),
                        _ => vacanciesQuery.OrderByDescending(x => x.vacancy.id)
                    }
                    : request.SortItem.ToLower() switch
                    {
                        // "city" => vacanciesQuery.OrderBy(x => x.resume.city),
                        "experience" => vacanciesQuery.OrderBy(x => x.vacancy.experience),
                        "income_date" => vacanciesQuery.OrderBy(x => x.vacancy.income_date),
                        _ => vacanciesQuery.OrderBy(x => x.vacancy.id)
                    };
            }
            // Сортировка по имени компании, описанию, должности
            if (!string.IsNullOrEmpty(request.search))
            {
                vacanciesQuery = vacanciesQuery
                    .Where(v => v.company.name.ToLower().Contains(request.search.ToLower())
                    || v.vacancy.description.ToLower().Contains(request.search.ToLower())
                    || v.vacancy.post.Contains(request.search.ToLower())
                    );
            }
            var groupedResumesDtos = vacanciesQuery
                .AsEnumerable()
                .GroupBy(x => x.vacancy.id)
                .Select(group => new VacancyDtos()
                {
                    id = group.First().vacancy.id,
                    company_id = group.First().vacancy.company_id,
                    post = group.First().vacancy.post,
                    min_salary = group.First().vacancy.min_salary,
                    education_id = group.First().vacancy.education_id,
                    experience = group.First().vacancy.experience,
                    description = group.First().vacancy.description,
                    income_date = group.First().vacancy.income_date,
                    max_salary = group.First().vacancy.max_salary,
                    activities = group.Select(g => new ActivityDtos
                    {
                        id = g.activity.id,
                        direction = g.activity.direction,
                        type = g.activity.type,
                    }).Distinct().ToList(),
                    company = new CompanyDto
                    {
                        id = group.First().company.id,
                        name = group.First().company.name,
                        email = group.First().company.email,
                        phoneNumber = group.First().company?.phoneNumber,
                        website = group.First().company?.website,
                        latitude = group.First().company.office_coord?.Y.ToString(CultureInfo.InvariantCulture)!, // Широта (Y)
                        longitude =  group.First().company.office_coord?.X.ToString(CultureInfo.InvariantCulture)!  // Долгота (X)
                    }
                }).ToList();
    
            return Ok(new {resumes = groupedResumesDtos});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"An error occured while show resumes by company");
            return BadRequest(500);
        }
    }
    
    
    [AllowAnonymous]
    [HttpGet("Vacancies/Info")]
    public async Task<ActionResult> GetVacancyInfo([FromQuery] ulong vacancyId)
    {
        try
        {
            var vacanciesQuery = _dbContext.Vacancies
                .Join(_dbContext.Vacancy_filters, // добавляем фильтры
                    vacancy => vacancy.id,
                    filter => filter.vacancy_id,
                    (vacancy, filter) => new { vacancy, filter })
                .Join(_dbContext.typeOfActivities, // добавляем имена к фильтрам
                    arg => arg.filter.typeOfActivity_id,
                    activity => activity.id,
                    (arg, activity) => new { arg.vacancy, arg.filter, activity }
                )
                .Join(_dbContext.companies,
                    arg => arg.vacancy.company_id,
                    company =>  company.id,
                    (arg, company) => new { arg.vacancy, arg.filter, arg.activity, company }
                )
                .Where(r => r.vacancy.id == vacancyId)
                .AsNoTracking()
                .AsQueryable(); //Коллекция резюме

            // Достаём фото профиля
            string? userId = vacanciesQuery.AsEnumerable().Select(r => r.company.id).FirstOrDefault();
            byte[]? image = await _dbContext.Users.Where(u => u.Id == userId).Select(u => u.image)
                .FirstOrDefaultAsync();

            var groupedResumesDtos = vacanciesQuery
                .AsEnumerable()
                .GroupBy(x => x.vacancy.id)
                .Select(group => new VacancyDtos
                {
                    id = group.First().vacancy.id,
                    company_id = group.First().vacancy.company_id,
                    post = group.First().vacancy.post,
                    experience = group.First().vacancy.experience,
                    income_date = group.First().vacancy.income_date,
                    education_id = group.First().vacancy.education_id,
                    description = group.First().vacancy.description,
                    min_salary = group.First().vacancy.min_salary,
                    max_salary = group.First().vacancy.max_salary,
                    activities = group.Select(g => new ActivityDtos
                    {
                        id = g.activity.id,
                        direction = g.activity.direction,
                        type = g.activity.type,
                    }).Distinct().ToList(),
                    company = new CompanyDto
                    {
                        id = group.First().company.id,
                        name = group.First().company.name,
                        email = group.First().company.email,
                        phoneNumber = group.First().company?.phoneNumber,
                        website = group.First().company?.website,
                        latitude = group.First().company.office_coord?.Y.ToString(CultureInfo.InvariantCulture)!, // Широта (Y)
                        longitude =  group.First().company.office_coord?.X.ToString(CultureInfo.InvariantCulture)!  // Долгота (X)
                    }

                }).ToList();
            return Ok(new { resume = groupedResumesDtos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while show vacancies by worker");
            return BadRequest(500);
        }
    }
}