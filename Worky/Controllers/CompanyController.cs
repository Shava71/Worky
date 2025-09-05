using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using NuGet.Protocol;
using Worky.Context;
using Worky.Contracts;
using Worky.DTO;
using Worky.Migrations;
using Worky.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Worky.Services;
using ZXing;
using ZXing.QrCode;
using ZXing.Rendering;

namespace Worky.Controllers;

[Authorize(Roles = "Company", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/v1/[controller]")]
// [Authorize(Roles = "Company")]
public class CompanyController : Controller
{
    private readonly WorkyDbContext _dbContext;
    UserManager<Users> _userManager;
    ILogger<CompanyController> _logger;

    public CompanyController(WorkyDbContext dbContext, ILogger<CompanyController> logger,
        UserManager<Users> userManager)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userManager = userManager;
    }
    // GET
    // [HttpGet]
    // public IActionResult ShowResume()
    // {
    //     var Resumes = _db.Resumes.ToList();
    //     return Ok(Resumes);
    // }

    [AllowAnonymous]
    [HttpGet("Resumes")]
    public async Task<IActionResult> FilterResume([FromQuery] GetResumesRequest? request)
    {
        // var name = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // var roles = User.FindFirstValue(ClaimTypes.Role).ToList();
        // _logger.LogInformation("User: {name}, Roles: {roles}", name, string.Join(", ", roles));
        // _logger.LogInformation("User: {name}", name);
        try
        {
            var resumesQuery = _dbContext.Resumes
                .Where(resume =>
                    !_dbContext.Feedbacks
                        .Any(f => f.resume_id == resume.id && f.status == FeedbackStatus.Accepted))
                .Join(_dbContext.Resume_filters, // добавляем фильтры
                    resume => resume.id,
                    filter => filter.resume_id,
                    (resume, filter) => new { resume, filter })
                .Join(_dbContext.typeOfActivities, // добавляем имена к фильтрам
                    arg => arg.filter.typeOfActivity_id,
                    activity => activity.id,
                    (arg, activity) => new { arg.resume, arg.filter, activity }
                )
                .AsNoTracking()
                .AsQueryable(); //Коллекция резюме
            // Поиск по id
            if (request.id.HasValue)
            {
                resumesQuery = resumesQuery
                    .Where(r => r.resume.id == request.id);
            }

            // Минимальный опыт работы
            if ((bool)request?.min_experience.HasValue)
            {
                resumesQuery = resumesQuery
                    .Where(r => r.resume.experience >= request.min_experience);
            }

            // Максимальный опыт работы
            if ((bool)request?.max_experience.HasValue)
            {
                resumesQuery = resumesQuery
                    .Where(r => r.resume.experience <= request.max_experience);
            }

            // Сортировка по городу
            if (!string.IsNullOrWhiteSpace(request?.city))
            {
                resumesQuery = resumesQuery
                    .Where(r => r.resume.city.ToLower().Contains(request.city.ToLower()));
            }

            // Сортировка по минимальной желаемой зарплате
            if ((bool)request?.min_wantedSalary.HasValue)
            {
                resumesQuery = resumesQuery
                    .Where(r => r.resume.wantedSalary >= request.min_wantedSalary);
            }

            // Сортировка по максимальной желаемой зарплате
            if ((bool)request?.max_wantedSalary.HasValue)
            {
                resumesQuery = resumesQuery
                    .Where(r => r.resume.wantedSalary <= request.max_wantedSalary);
            }

            // Сортировка по дате
            if (!string.IsNullOrWhiteSpace(request?.income_date.ToString()))
            {
                var date = request.income_date.Value.Date;
                var nextDate = date.AddDays(1);

                resumesQuery = resumesQuery
                    .Where(r => r.resume.income_date >= date && r.resume.income_date < nextDate);
            }

            // Сортировка по образованию
            if ((bool)request?.education.HasValue)
            {
                resumesQuery = resumesQuery
                    .Where(r => r.resume.education_id == (ulong?)request.education);
            }

            // Сортировка по виду деятельности
            if (!string.IsNullOrWhiteSpace(request?.type))
            {
                resumesQuery = resumesQuery
                    .Where(r => r.activity.type == request.type);
            }

            // Сортировка по направлению вида деятельности
            if (request?.direction is { Count: > 0 })
            {
                var resumesIdDirection = resumesQuery
                    .Where(r => request.direction!.Contains(r.activity.direction)).Select(r => r.resume.id).ToHashSet();
                // resumesQuery = resumesQuery
                //     .Where(r => request.direction!.Contains(r.activity.direction));
                resumesQuery = resumesQuery
                    .Where(r => resumesIdDirection.Contains(r.resume.id));
            }

            // Столбец сортировки
            // Expression<Func<Resume, object>> selectorKey = request.SortItem?.ToLower() switch
            // {
            //     "city" => resume => resume.city,
            //     "experience" => resume => resume.experience,
            //     "income_date" => resume => resume.income_date,
            //     _ => resume => resume.id
            // };
            //
            // resumesQuery = request.Order == "desc"
            //     ? resumesQuery.OrderByDescending(x => selectorKey.Compile()(x.resume))
            //     : resumesQuery.OrderBy(x => selectorKey);
            if (!string.IsNullOrEmpty(request.SortItem))
            {
                resumesQuery = request.Order?.ToLower() == "desc"
                    ? request.SortItem.ToLower() switch
                    {
                        "city" => resumesQuery.OrderByDescending(x => x.resume.city),
                        "experience" => resumesQuery.OrderByDescending(x => x.resume.experience),
                        "income_date" => resumesQuery.OrderByDescending(x => x.resume.income_date),
                        _ => resumesQuery.OrderByDescending(x => x.resume.id)
                    }
                    : request.SortItem.ToLower() switch
                    {
                        "city" => resumesQuery.OrderBy(x => x.resume.city),
                        "experience" => resumesQuery.OrderBy(x => x.resume.experience),
                        "income_date" => resumesQuery.OrderBy(x => x.resume.income_date),
                        _ => resumesQuery.OrderBy(x => x.resume.id)
                    };
            }

            // var groupedResumesDtos = resumesQuery
            //     .AsEnumerable()
            //     .GroupBy(x => x.resume.id)
            //     .Select(group => new 
            //     {
            //         resume = group.Select(g => g.resume).FirstOrDefault(),
            //         activity = group.Select(g => g.activity).Distinct().OrderBy(g => g.direction).ToList()
            //     }).ToList();

            var groupedResumesDtos = resumesQuery
                .AsEnumerable()
                .GroupBy(x => x.resume.id)
                .Select(group => new ResumeDtos
                {
                    id = group.First().resume.id,
                    worker_id = group.First().resume.worker_id,
                    city = group.First().resume.city,
                    experience = group.First().resume.experience,
                    income_date = group.First().resume.income_date,
                    education_id = group.First().resume.education_id,
                    post = group.First().resume.post,
                    skill = group.First().resume.skill,
                    wantedSalary = group.First().resume.wantedSalary,
                    activities = group.Select(g => new ActivityDtos
                    {
                        id = g.activity.id,
                        direction = g.activity.direction,
                        type = g.activity.type,
                    }).Distinct().ToList()
                }).ToList();

            // var groupedResumesDtos = resumesQuery.Select(r => new ResumeDtos
            // {
            //     id = r.resume.id,
            //     worker_id = r.resume.worker_id,
            //     city = r.resume.city,
            //     experience = r.resume.experience,
            //     income_date = r.resume.income_date,
            //     education_id = r.resume.education_id,
            //     post = r.resume.post,
            //     skill = r.resume.skill,
            //     wantedSalary = r.resume.wantedSalary,
            //     activities = 
            //     
            // })

            return Ok(new { resumes = groupedResumesDtos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while show resumes by company");
            return BadRequest(500);
        }
    }


    [AllowAnonymous]
    [HttpGet("Resumes/Info")]
    public async Task<ActionResult> GetResumeInfo([FromQuery] ulong resumeId)
    {
        try
        {
            // var resumesQuery = _dbContext.Resumes
            //     .Join(_dbContext.Resume_filters, // добавляем фильтры
            //         resume => resume.id,
            //         filter => filter.resume_id,
            //         (resume, filter) => new { resume, filter })
            //     .Join(_dbContext.typeOfActivities, // добавляем имена к фильтрам
            //         arg => arg.filter.typeOfActivity_id,
            //         activity => activity.id,
            //         (arg, activity) => new { arg.resume, arg.filter, activity }
            //     )
            //     .Join(_dbContext.Workers,
            //         arg => arg.resume.worker_id,
            //         worker => worker.id,
            //         (arg, worker) => new { arg.resume, arg.filter, arg.activity, worker }
            //     )
            //     .Where(r => r.resume.id == resumeId)
            //     .AsNoTracking()
            //     .AsQueryable(); //Коллекция резюме

            var resumesQuery = //перепись под left join
                from resume in _dbContext.Resumes
                where resume.id == resumeId
                join worker in _dbContext.Workers
                    on resume.worker_id equals worker.id
                join filter in _dbContext.Resume_filters
                    on resume.id equals filter.resume_id into filterGroup
                from filter in filterGroup.DefaultIfEmpty()
                join activity in _dbContext.typeOfActivities
                    on filter.typeOfActivity_id equals activity.id into activityGroup
                from activity in activityGroup.DefaultIfEmpty()
                select new { resume, filter, activity, worker };

            //фото профиля
            string? userId = resumesQuery.AsEnumerable().Select(r => r.worker.id).FirstOrDefault();
            byte[]? image = await _dbContext.Users.Where(u => u.Id == userId).Select(u => u.image)
                .FirstOrDefaultAsync();

            var groupedResumesDtos = resumesQuery
                .AsEnumerable()
                .GroupBy(x => x.resume.id)
                .Select(group => new ResumeDtos
                {
                    id = group.First().resume.id,
                    worker_id = group.First().resume.worker_id,
                    city = group.First().resume.city,
                    experience = group.First().resume.experience,
                    income_date = group.First().resume.income_date,
                    education_id = group.First().resume.education_id,
                    post = group.First().resume.post,
                    skill = group.First().resume.skill,
                    wantedSalary = group.First().resume.wantedSalary,
                    activities = group.Where(g => g.activity != null).Select(g => new ActivityDtos
                    {
                        id = g.activity.id,
                        direction = g.activity.direction,
                        type = g.activity.type,
                    }).Distinct().ToList(),
                    worker = new WorkerDtos()
                    {
                        id = group.First().worker.id,
                        first_name = group.First().worker.first_name,
                        second_name = group.First().worker.second_name,
                        surname = group.First().worker.surname,
                        image = image,
                        birthday = group.First().worker.birthday,
                    }
                }).ToList();
            return Ok(new { resume = groupedResumesDtos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while show resumes by company");
            return BadRequest(500);
        }
    }

    [HttpGet("MyVacancy")]
    public async Task<ActionResult> GetMyVacancy([FromQuery] ulong? vacancyId)
    {
        try
        {
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Users userDB = _dbContext.Users.FirstOrDefault(u => u.Id == currentIdUser.ToString())!;
            var user = new
            {
                UserName = userDB.UserName,
                PasswordHash = userDB.PasswordHash,
            };

            string connectionString =
                $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sqlQuery = @"
                    SELECT v.*, f.*,
                            a.id AS a_id, 
                            a.type AS a_type, 
                            a.direction AS a_direction,
                            f.filter_id AS filter_id
                    FROM vacancy_cur_user v
                    LEFT JOIN Vacancy_filter f ON v.id = f.vacancy_id
                    LEFT JOIN typeOfActivity a ON f.typeOfActivity_id = a.id";

                var result = await db.QueryAsync(sqlQuery);
                var vacancyDict = new Dictionary<ulong, VacancyDtos>();

                foreach (var row in result)
                {
                    ulong id = (ulong)row.id;

                    if (!vacancyDict.TryGetValue(id, out var vacancy))
                    {
                        vacancy = new VacancyDtos
                        {
                            id = row.id,
                            company_id = row.company_id,
                            post = row.post,
                            min_salary = row.min_salary,
                            education_id = row.education_id,
                            experience = row.experience,
                            description = row.description,
                            income_date = row.income_date,
                            max_salary = row.max_salary,
                            activities = new List<ActivityDtos>()
                        };
                        vacancyDict.Add(id, vacancy);
                    }

                    if (row.a_id != null) // a.id может быть NULL из-за LEFT JOIN
                    {
                        var activity = new ActivityDtos
                        {
                            id = row.a_id,
                            type = row.a_type,
                            direction = row.a_direction,
                            filter_id = row.filter_id,
                        };
                        vacancy.activities!.Add(activity);
                    }
                }

                var vacancyDto = vacancyDict.Values.ToList();
                if (vacancyId.HasValue) // Сортировка по id
                {
                    vacancyDto = vacancyDto.Where(v => v.id == vacancyId).ToList();
                }

                return Ok(vacancyDto);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while get my vacancy by company");
            return BadRequest(500);
        }
    }

    [HttpPost("CreateVacancy")]
    public async Task<ActionResult> CreateVacancy([FromBody] CreateVacancy newVacancy)
    {
        try
        {
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Users userDB = _dbContext.Users.FirstOrDefault(u => u.Id == currentIdUser.ToString())!;
            var user = new
            {
                UserName = userDB.UserName,
                PasswordHash = userDB.PasswordHash,
            };

            DateTime dateTime = DateTime.UtcNow.Date;
            DateOnly curDate = DateOnly.FromDateTime(dateTime);
            var curDeal = _dbContext.Deals.FirstOrDefault(d => d.status == true &&
                                                               curDate >= d.date_start &&
                                                               curDate <= d.date_end &&
                                                               d.company_id == currentIdUser.ToString());
            if (curDeal == null)
            {
                return NotFound("No current deal");
            }

            int vacancyCountTarrif = _dbContext.Tarrifs.Where(t => t.id == curDeal.tariff_id)
                .Select(t => t.vacancy_count).FirstOrDefault();
            int vacancyCountUser = _dbContext.Vacancies.Count(v => v.company_id == currentIdUser.ToString());
            if (vacancyCountUser >= vacancyCountTarrif)
            {
                return BadRequest("Vacancy count is too big");
            }


            string connectionString =
                $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sqlQuery =
                    "INSERT INTO vacancy_cur_user(company_id, post, min_salary, max_salary, education_id, experience, description,income_date,createdBy)" +
                    "values(@company_id,@post,@min_salary,@max_salary,@education_id,@experience,@description,CURRENT_TIMESTAMP(),substring_index(user(),'@',1));" +
                    "SELECT LAST_INSERT_ID();";
                ulong id = await db.ExecuteScalarAsync<ulong>(sqlQuery,
                    new
                    {
                        company_id = currentIdUser, newVacancy.post, newVacancy.min_salary, newVacancy.max_salary,
                        newVacancy.education_id, newVacancy.experience, newVacancy.description
                    });

                return Ok(new { id = id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while create vacancy by company");
            return BadRequest(500);
        }
    }

    [HttpPut("UpdateVacancy")]
    public async Task<ActionResult> UpdateVacancy([FromBody] UpdateVacancy updatedVacancy)
    {
        try
        {
            // Получаем ID текущего пользователя
            Guid currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Users userDB = _dbContext.Users.FirstOrDefault(u => u.Id == currentUserId.ToString())!;

            var user = new
            {
                UserName = userDB.UserName,
                PasswordHash = userDB.PasswordHash,
            };

            string connectionString =
                $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";

            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string updateSql = @"
                    UPDATE vacancy_cur_user 
                    SET 
                        post = @post,
                        min_salary = @min_salary,
                        max_salary = @max_salary,
                        education_id = @education_id,
                        experience = @experience,
                        description = @description
                    WHERE id = @id";

                var parameters = new
                {
                    id = updatedVacancy.Id,
                    post = updatedVacancy.Post,
                    min_salary = updatedVacancy.MinSalary,
                    max_salary = updatedVacancy.MaxSalary,
                    education_id = updatedVacancy.EducationId,
                    experience = updatedVacancy.Experience,
                    description = updatedVacancy.Description
                };

                int rowsCount = await db.ExecuteAsync(updateSql, parameters);

                if (rowsCount == 0)
                    return StatusCode(500, "Failed to update vacancy");

                return Ok(new { message = "Vacancy updated successfully" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating vacancy");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("DeleteVacancy")]
    public async Task<ActionResult> DeleteVacancy([FromQuery] ulong vacancyId)
    {
        try
        {
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Users userDB = _dbContext.Users.FirstOrDefault(u => u.Id == currentIdUser.ToString())!;

            var user = new
            {
                UserName = userDB.UserName,
                PasswordHash = userDB.PasswordHash,
            };

            string connectionString =
                $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";

            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                // Удаляем вакансию
                string deleteSql = "DELETE FROM vacancy_cur_user WHERE id = @vacancyId";
                int rowsAffected = await db.ExecuteAsync(deleteSql, new { vacancyId = vacancyId });

                if (rowsAffected == 0)
                    return StatusCode(500, "Failed to delete vacancy");

                return Ok("Vacancy deleted successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting vacancy");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("AddVacancyFilter")]
    public async Task<ActionResult> AddVacancyFilter([FromBody] AddFilter newFilter)
    {
        try
        {
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Users userDB = _dbContext.Users.FirstOrDefault(u => u.Id == currentIdUser.ToString())!;
            var user = new
            {
                UserName = userDB.UserName,
                PasswordHash = userDB.PasswordHash,
            };
            string connectionString =
                $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                List<ulong> id = new List<ulong>();
                foreach (ulong activity_id in newFilter.typeOfActivity_id)
                {
                    string sqlQuery = @"
                    INSERT INTO Vacancy_filter(vacancy_id,typeOfActivity_id)
                    values (
                            @vacancy_id,
                            @typeOfActivity_id
                    );
                    SELECT LAST_INSERT_ID();
                    ";
                    ulong cur_id = await db.ExecuteScalarAsync<ulong>(sqlQuery,
                        new { vacancy_id = newFilter.id, typeOfActivity_id = activity_id });
                    id.Add(cur_id);
                }


                return Ok(new { id = id });
            }

            ;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while add filter to vacancy by company");
            return BadRequest(500);
        }
    }

    [HttpDelete("DeleteVacancyFilter")]
    public async Task<ActionResult> DeleteVacancyFilter([FromQuery] ulong filterId)
    {
        try
        {
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Users userDB = _dbContext.Users.FirstOrDefault(u => u.Id == currentIdUser.ToString())!;
            var user = new
            {
                UserName = userDB.UserName,
                PasswordHash = userDB.PasswordHash,
            };
            string connectionString =
                $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sqlQuery = @"
                DELETE FROM Vacancy_filter WHERE filter_id = @filterId;
                ";
                int rowCount = await db.ExecuteAsync(sqlQuery, new { filterId = filterId });

                if (rowCount == 0)
                    return StatusCode(500, "Failed to delete vacancy");
                return Ok("filter deleted");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while delete filter to vacancy by company");
            return BadRequest(500);
        }
    }

    [AllowAnonymous]
    [HttpGet("Tarrif")]
    public async Task<IActionResult> GetTarrif([FromQuery] ulong? tariffId)
    {
        try
        {
            var tarrifs = _dbContext.Tarrifs.ToList();

            if (tariffId.HasValue)
            {
                tarrifs = tarrifs.Where(t => t.id == tariffId).ToList();
            }

            return Ok(new { tarrifs = tarrifs });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while get tarrifs by company");
            return BadRequest(500);
        }
    }

    [HttpPost("MakeDeal")]
    public async Task<IActionResult> MakeDeal(MakeDealRequest request)
    {
        try
        {
            DateTime dateTime = DateTime.UtcNow.Date;
            DateOnly curDate = DateOnly.FromDateTime(dateTime);
            int sum = _dbContext.Tarrifs.Where(t => t.id == request.tarrif_id).Select(t => t.price).FirstOrDefault();

            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Deal newDeal = new Deal()
            {
                tariff_id = request.tarrif_id,
                company_id = currentIdUser.ToString(),
                status = true, //пока ставим статус оплаченного
                date_start = curDate,
                date_end = curDate.AddMonths(request.countMonth),
                sum = sum * request.countMonth
            };
            await _dbContext.Deals.AddAsync(newDeal);
            await _dbContext.SaveChangesAsync();
            return Ok(new { id = newDeal.id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while make deal by company");
            return BadRequest(500);
        }
    }

    [HttpGet("GetProfile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Users user = _userManager.FindByIdAsync(currentIdUser.ToString()).Result!;
            Company company = _dbContext.companies.FirstOrDefault(c => c.id == currentIdUser.ToString())!;
            CompanyDto companyDto = new CompanyDto
            {
                id = company.id,
                name = company.name,
                email = company.email,
                phoneNumber = company.phoneNumber,
                website = company.website,
                latitude = company.office_coord?.Y.ToString(CultureInfo.InvariantCulture), // Широта (Y)
                longitude = company.office_coord?.X.ToString(CultureInfo.InvariantCulture) // Долгота (X)
            };
            List<DealDto> dealsDto = _dbContext.Deals
                .Where(d => d.company_id == currentIdUser.ToString())
                .Select(d => new DealDto
                {
                    id = d.id,
                    tariff_id = d.tariff_id,
                    company_id = d.company_id,
                    status = d.status,
                    date_start = d.date_start,
                    date_end = d.date_end,
                    sum = d.sum
                }).ToList();

            return Ok(new CompanyProfileDtos
            {
                company = companyDto,
                user = user,
                deals = dealsDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while get profile by company");
            return BadRequest(500);
        }
    }

    [HttpGet("GetFeedback")]
    public async Task<IActionResult> GetFeedback([FromQuery] ulong? vacancyId)
    {
        try
        {
            // var feedbacks = _dbContext.Feedbacks
            //     .Join(_dbContext.Vacancies,
            //         f => f.vacancy_id,
            //         v => v.id,
            //         ((feedback, vacancy) => new { vacancy, feedback }))
            //     .Where(f => f.vacancy.company_id == User.FindFirstValue(ClaimTypes.NameIdentifier)!)
            //     .ToList();
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Users userDB = _dbContext.Users.FirstOrDefault(u => u.Id == currentIdUser.ToString())!;
            var user = new
            {
                UserName = userDB.UserName,
                PasswordHash = userDB.PasswordHash,
            };

            string connectionString =
                $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sqlQuery = @"
                SELECT * FROM feedback_cur_user;
                 ";

                var feedbacks = await db.QueryAsync<Feedback>(sqlQuery);

                // var newfeedbackDto = feedbacks.Select(f => new FeedbackDtos()
                // {
                //     id = f.feedback.id,
                //     resume_id = f.feedback.resume_id,
                //     vacancy_id = f.feedback.vacancy_id,
                //     status = f.feedback.status,
                // }).ToList();
                if (vacancyId.HasValue)
                {
                    feedbacks = feedbacks
                        .Where(f => f.vacancy_id == vacancyId);
                }

                var newfeedbackDto = feedbacks.Select(f => new FeedbackDtos()
                {
                    id = f.id,
                    resume_id = f.resume_id,
                    vacancy_id = f.vacancy_id,
                    status = f.status,
                }).ToList();

                return Ok(new { feedbacks = newfeedbackDto });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while get feedbacks by company");
            return BadRequest(500);
        }
    }

    [HttpPost("MakeFeedback")]
    public async Task<IActionResult> MakeFeedback([FromBody] MakeFeedbackRequest request)
    {
        try
        {
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Users userDB = _dbContext.Users.FirstOrDefault(u => u.Id == currentIdUser.ToString())!;
            var user = new
            {
                UserName = userDB.UserName,
                PasswordHash = userDB.PasswordHash,
            };

            string connectionString =
                $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                var req = _dbContext.Resumes
                    .Where(req => req.id == request.resume_id)
                    .Select(req => req.worker_id).FirstOrDefault();
                var worker = await _userManager.FindByIdAsync(req);

                string username;
                if (worker != null)
                {
                    username = worker.UserName;
                }
                else
                {
                    username = "mac";
                }

                string sqlQuery = @"
                INSERT INTO feedback_cur_user(resume_id, vacancy_id, status, createdBy1, createdBy2)
                values (@resume_id, @vacancy_id, @status, substring_index(USER(), '@',1), @createdBy2);
                SELECT LAST_INSERT_ID();
                ";
                var id = await db.ExecuteScalarAsync<ulong>(sqlQuery, new
                {
                    resume_id = request.resume_id,
                    vacancy_id = request.vacancy_id,
                    status = FeedbackStatus.InProgress.ToString(),
                    createdBy2 = username
                });

                return Ok(new { id = id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while make feedbacks by company");
            return BadRequest(500);
        }
    }

    [HttpPost("ChangeFeedbackStatus")]
    public async Task<IActionResult> AcceptFeedback([FromBody] FeedbackStatusRequest request)
    {
        try
        {
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Users userDB = _dbContext.Users.FirstOrDefault(u => u.Id == currentIdUser.ToString())!;
            var user = new
            {
                UserName = userDB.UserName,
                PasswordHash = userDB.PasswordHash,
            };

            string connectionString =
                $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sqlQuery = @"
                    UPDATE feedback_cur_user SET status = @status WHERE id = @id;
                    ";
                await db.ExecuteAsync(sqlQuery, new { status = request.status, id = request.feedback_id });
                return Ok(new { id = request.feedback_id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while accept feedbacks by company");
            return BadRequest(500);
        }
    }


    [HttpGet("receipt/{dealId}")]
    public async Task<IActionResult> GetDealReceipt(ulong dealId)
    {
        try
        {
            var deal = await _dbContext.Deals
                .Join(_dbContext.companies,
                    d => d.company_id,
                    c => c.id,
                    (d, c) => new { Deal = d, Company = c })
                .Where(x => x.Deal.id == dealId)
                .Select(x => new
                {
                    x.Deal,
                    x.Company,
                    Tariff = _dbContext.Tarrifs.FirstOrDefault(t => t.id == x.Deal.tariff_id)
                })
                .FirstOrDefaultAsync();

            if (deal == null)
                return NotFound("Договор не найден");

            Guid userId = Guid.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
            if (deal.Company.id != userId.ToString())
                return BadRequest("Вы не можете получить чек по чужому договору");

            byte[] pdfBytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(12).FontFamily("DejaVu"));

                        page.Header()
                            .Text($"Worky - Чек об оплате договора №{deal.Deal.id}")
                            .FontSize(18).Bold().AlignCenter();

                        page.Content()
                            .PaddingVertical(1)
                            .Stack(stack =>
                            {
                                stack.Item().Text("Информация о компании").Bold();
                                stack.Item().Text($"Название: {deal.Company.name ?? "Неизвестная компания"}");
                                stack.Item().Text($"Email: {deal.Company.email ?? "—"}");

                                stack.Item().PaddingTop(10).Text("Детали договора").Bold();
                                stack.Item().Text($"Дата начала: {deal.Deal.date_start:dd.MM.yyyy}");
                                stack.Item().Text($"Дата окончания: {deal.Deal.date_end:dd.MM.yyyy}");
                                stack.Item().Text($"Статус: {(deal.Deal.status ? "Оплачен" : "Не оплачен")}")
                                    .FontColor(deal.Deal.status ? Colors.Green.Darken2 : Colors.Red.Darken2);

                                stack.Item().PaddingTop(10).LineHorizontal(1);

                                stack.Item().Text("Тариф").Bold();
                                stack.Item().Text($"Название: {deal.Tariff?.name ?? "—"}");
                                stack.Item().Text($"Описание: {deal.Tariff?.description ?? "—"}");
                                stack.Item().Text($"Количество вакансий: {deal.Tariff?.vacancy_count}");
                                stack.Item().Text($"Цена за месяц: {deal.Tariff?.price} ₽");
                                stack.Item().Text($"Общая сумма: {deal.Deal.sum} ₽")
                                    .FontSize(14).Bold().AlignCenter();
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text("Спасибо за сотрудничество!")
                            .Italic()
                            .FontSize(10);
                    });
                })
                .GeneratePdf();


            var fileName = $"deal_{dealId}_receipt.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ошибка генерации чека");
        }
    }

    [HttpGet("Vacancy/flyer/")]
    public async Task<IActionResult> GetVacancyFlyer(ulong vacancyId, string url)
    {
        try
        {
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Users userDB = _dbContext.Users.FirstOrDefault(u => u.Id == currentIdUser.ToString())!;
            var user = new
            {
                UserName = userDB.UserName,
                PasswordHash = userDB.PasswordHash,
            };

            var vacanciesQuery = _dbContext.Vacancies
                .Join(_dbContext.Vacancy_filters,
                    vacancy => vacancy.id,
                    filter => filter.vacancy_id,
                    (vacancy, filter) => new { vacancy, filter })
                .Join(_dbContext.typeOfActivities,
                    arg => arg.filter.typeOfActivity_id,
                    activity => activity.id,
                    (arg, activity) => new { arg.vacancy, arg.filter, activity }
                )
                .Join(_dbContext.companies,
                    arg => arg.vacancy.company_id,
                    company => company.id,
                    (arg, company) => new { arg.vacancy, arg.filter, arg.activity, company }
                )
                .Where(r => r.vacancy.id == vacancyId)
                .AsNoTracking()
                .AsQueryable();

            string? userId = vacanciesQuery.AsEnumerable().Select(r => r.company.id).FirstOrDefault();
            if (userId != currentIdUser.ToString())
            {
                return NotFound("That's not your vacancy for create flyer");
            }


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
                        latitude = group.First().company.office_coord?.Y
                            .ToString(CultureInfo.InvariantCulture)!, // Широта (Y)
                        longitude = group.First().company.office_coord?.X
                            .ToString(CultureInfo.InvariantCulture)! // Долгота (X)
                    }
                }).First()!;

            // byte[] qrBytes = await _qrCode.CreateQRcode($"https://worky.ru/vacancies/Info?vacancyId={vacancyId}");

            byte[] flyer = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(12).FontFamily("DejaVu"));

                        page.Header()
                            .Text(
                                $"Worky - Флайер на ваканцию \"{groupedResumesDtos.post}\" от компании \"{groupedResumesDtos.company.name}\"")
                            .AlignCenter().FontSize(18).Bold();

                        page.Content().PaddingVertical(1).Column(descriptor =>
                        {
                            descriptor.Item().Text("Информация о компании").SemiBold().FontSize(14);
                            descriptor.Item().Text($"Название: {groupedResumesDtos.company.name}");
                            descriptor.Item().Text($"Email: {groupedResumesDtos.company.email ?? "—"}");
                            descriptor.Item().Text($"Телефон: {groupedResumesDtos.company.phoneNumber ?? "—"}");
                            descriptor.Item().Text($"Сайт: {groupedResumesDtos.company.website ?? "—"}");
                            descriptor.Item()
                                .Text(
                                    $"Адрес офиса: {groupedResumesDtos.company.latitude}, {groupedResumesDtos.company.longitude}");

                            descriptor.Item().PaddingTop(15).LineHorizontal(1);
                            descriptor.Item().Text("Детали вакансии").SemiBold().FontSize(14);
                            descriptor.Item().Text($"Должность: {groupedResumesDtos.post}");
                            descriptor.Item().Text($"Описание: {groupedResumesDtos.description}");
                            descriptor.Item().Text($"Минимальная зарплата: {groupedResumesDtos.min_salary} ₽");
                            descriptor.Item()
                                .Text(
                                    $"Максимальная зарплата: {groupedResumesDtos.max_salary?.ToString() ?? "Не указана"} ₽");
                            descriptor.Item().Text($"Опыт работы: {groupedResumesDtos.experience} лет");

                            descriptor.Item().PaddingTop(15).LineHorizontal(1);
                            descriptor.Item().Text("Фильтры по направлениям").SemiBold().FontSize(14);

                            if (groupedResumesDtos.activities != null && groupedResumesDtos.activities.Count > 0)
                            {
                                foreach (var activity in groupedResumesDtos.activities)
                                {
                                    descriptor.Item().Text($"• {activity.direction} ({activity.type})");
                                }
                            }

                            // descriptor.Item().Image(qrBytes);

                            descriptor.Item().Row(row =>
                            {
                                row.ConstantItem(5, Unit.Centimetre)
                                    .AspectRatio(1)
                                    .Background(Colors.White)
                                    .Svg(size =>
                                    {
                                        var writer = new QRCodeWriter();
                                        var qrCode = writer.encode(url, BarcodeFormat.QR_CODE, (int)size.Width,
                                            (int)size.Height);
                                        var renderer = new SvgRenderer { FontName = "Lato" };
                                        return renderer.Render(qrCode, BarcodeFormat.EAN_13, null).Content;
                                    });
                            });

                            // descriptor.Item()
                            //     .Image(data => data.Bytes(qrBytes))
                            //     .Width(150)
                            //     .Height(150)
                            //     .AlignCenter();
                        });

                        page.Footer()
                            .AlignCenter()
                            .Text("Created by Worky.ru")
                            .Italic()
                            .FontSize(10);
                    });
                })
                .GeneratePdf();

            var fileName = $"flyer_{vacancyId}.pdf";
            return File(flyer, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while get flyer by company");
            return BadRequest(500);
        }
    }


    [HttpGet("Statistics/json")]
    public async Task<IActionResult> GetCompanyStatisticsJson([FromQuery] int start_year, [FromQuery] int start_month,
        [FromQuery] int end_year, [FromQuery] int end_month)
    {
        try
        {
            Guid currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            string companyId = currentUserId.ToString();

            var startDate = new DateTime(start_year, start_month, 1);
            var endDay = DateTime.DaysInMonth(end_year, end_month);
            var endDate = new DateTime(end_year, end_month, endDay);

            // Получаем все вакансии компании за период
            var companyVacancies = await _dbContext.Vacancies
                .Where(v => v.company_id == companyId)
                .Select(v => v.id)
                .ToListAsync();

            if (!companyVacancies.Any())
            {
                return Ok(new
                {
                    VacancyCount = 0,
                    TotalFeedbacks = 0,
                    AcceptedFeedbacks = 0,
                    RejectedFeedbacks = 0,
                    AvgFeedbackPerVacancy = 0.0,
                    Period = $"{startDate:MMMM yyyy} - {endDate:MMMM yyyy}",
                    AcceptedWorkers = new List<WorkerDtos>()
                });
            }

            // Все отклики по этим вакансиям
            var feedbacks = await (
                from f in _dbContext.Feedbacks
                join v in _dbContext.Vacancies on f.vacancy_id equals v.id
                join r in _dbContext.Resumes on f.resume_id equals r.id
                join w in _dbContext.Workers on r.worker_id equals w.id
                join u in _dbContext.Users on w.id equals u.Id
                where companyVacancies.Contains(f.vacancy_id)
                      && f.income_date >= DateOnly.FromDateTime(startDate) &&
                      f.income_date <= DateOnly.FromDateTime(endDate)
                select new
                {
                    Feedback = f,
                    Vacancy = v,
                    Resume = r,
                    Worker = w,
                    User = u
                }
            ).ToListAsync();

            DateTime today = DateTime.Now;
            DateOnly dateOnly = new DateOnly(today.Year, today.Month, today.Day);

            // Собираем список принятых кандидатов (работников)
            var acceptedWorkers = feedbacks
                .Where(f => f.Feedback.status == FeedbackStatus.Accepted)
                .Select(f => new WorkerDtos()
                {
                    id = f.Resume.worker.id,
                    first_name = f.Resume.worker.first_name,
                    second_name = f.Resume.worker.second_name,
                    surname = f.Resume.worker.surname,

                    email = f.User.Email,
                    phone = f.User.PhoneNumber,
                    age = (dateOnly.Year - f.Worker.birthday.Year)
                })
                .DistinctBy(w => w.id)
                .ToList();

            var totalFeedbacks = feedbacks.Count;
            var acceptedFeedbacks = feedbacks.Count(f => f.Feedback.status == FeedbackStatus.Accepted);
            var rejectedFeedbacks = feedbacks.Count(f => f.Feedback.status == FeedbackStatus.Cancelled);

            double avgFeedbackPerVacancy = Math.Round((double)totalFeedbacks / companyVacancies.Count, 2);

            return Ok(new
            {
                // fed = feedbacks.Select(f => f.Feedback),
                // vac = companyVacancies
                VacancyCount = companyVacancies.Count,
                TotalFeedbacks = totalFeedbacks,
                AcceptedFeedbacks = acceptedFeedbacks,
                RejectedFeedbacks = rejectedFeedbacks,
                AvgFeedbackPerVacancy = avgFeedbackPerVacancy,
                Period = $"{startDate:MMMM yyyy} - {endDate:MMMM yyyy}",
                AcceptedWorkers = acceptedWorkers,

                // log = feedbacks
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении JSON-статистики компании");
            return BadRequest("Произошла ошибка при загрузке статистики.");
        }
    }

    [HttpGet("Statistics/pdf")]
    public async Task<IActionResult> GetCompanyStatisticsPdf([FromQuery] int start_year, [FromQuery] int start_month,
        [FromQuery] int end_year, [FromQuery] int end_month)
    {
        try
        {
            // Вызываем JSON-метод для получения данных
            var jsonResult =
                await GetCompanyStatisticsJson(start_year, start_month, end_year, end_month) as OkObjectResult;

            if (jsonResult?.Value == null)
            {
                return StatusCode(500, "Не удалось получить данные статистики");
            }

            dynamic data = jsonResult.Value;

            byte[] pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Заголовок
                    page.Header()
                        .Text($"Статистика компании за {data.Period}")
                        .FontSize(18).Bold().AlignCenter();

                    // Содержание
                    page.Content()
                        .Column(column =>
                        {
                            column.Item().Text($"Количество опубликованных вакансий: {data.VacancyCount}")
                                .FontSize(14);

                            column.Item().Text($"Общее количество откликов: {data.TotalFeedbacks}")
                                .FontSize(14);

                            column.Item().Text($"Принятые отклики: {data.AcceptedFeedbacks}")
                                .FontSize(14);

                            column.Item().Text($"Отклоненные отклики: {data.RejectedFeedbacks}")
                                .FontSize(14);

                            column.Item().Text($"Среднее количество откликов на вакансию: {data.AvgFeedbackPerVacancy}")
                                .FontSize(14);

                            column.Item().PaddingTop(20).Text("Принятые сотрудники:")
                                .FontSize(16).Bold();

                            if (data.AcceptedWorkers.Count > 0)
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("ФИО").SemiBold().FontSize(12);
                                        header.Cell().Text("Email").SemiBold().FontSize(12);
                                        header.Cell().Text("Телефон").SemiBold().FontSize(12);
                                        header.Cell().Text("Возраст").SemiBold().FontSize(12);
                                    });

                                    foreach (var worker in data.AcceptedWorkers)
                                    {
                                        table.Cell().Text($"{worker.first_name} {worker.second_name} {worker.surname}");
                                        table.Cell().Text($"{worker.email}");
                                        table.Cell().Text($"{worker.phone}");
                                        table.Cell().Text($"{worker.age.ToString()}");
                                    }
                                });
                            }
                            else
                            {
                                column.Item().PaddingTop(10)
                                    .Text("За этот период никто не был принят.")
                                    .Italic()
                                    .FontSize(12);
                            }
                        });

                    // Футер
                    page.Footer()
                        .AlignCenter()
                        .Text("Worky - платформа трудоустройства")
                        .FontSize(10)
                        .Italic();
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"statistics_{data.Period}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при генерации PDF-статистики");
            return StatusCode(500, "Внутренняя ошибка сервера при генерации PDF.");
        }
    }
}