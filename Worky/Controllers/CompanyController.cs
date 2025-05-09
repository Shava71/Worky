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

    public CompanyController(WorkyDbContext dbContext, ILogger<CompanyController> logger, UserManager<Users> userManager)
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
                    (resume, filter) => new {resume, filter})
                .Join(_dbContext.typeOfActivities, // добавляем имена к фильтрам
                    arg => arg.filter.typeOfActivity_id,
                    activity => activity.id,
                    (arg, activity) => new {arg.resume, arg.filter, activity}
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
            if((bool)request?.min_wantedSalary.HasValue)
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
    
            return Ok(new {resumes = groupedResumesDtos});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"An error occured while show resumes by company");
            return BadRequest(500);
        }
    }
    
    
    [AllowAnonymous]
    [HttpGet("Resumes/Info")]
    public async Task<ActionResult> GetResumeInfo([FromQuery] ulong resumeId)
    {
        try
        {
            var resumesQuery = _dbContext.Resumes
                .Join(_dbContext.Resume_filters, // добавляем фильтры
                    resume => resume.id,
                    filter => filter.resume_id,
                    (resume, filter) => new { resume, filter })
                .Join(_dbContext.typeOfActivities, // добавляем имена к фильтрам
                    arg => arg.filter.typeOfActivity_id,
                    activity => activity.id,
                    (arg, activity) => new { arg.resume, arg.filter, activity }
                )
                .Join(_dbContext.Workers,
                    arg => arg.resume.worker_id,
                    worker => worker.id,
                    (arg, worker) => new { arg.resume, arg.filter, arg.activity, worker }
                )
                .Where(r => r.resume.id == resumeId)
                .AsNoTracking()
                .AsQueryable(); //Коллекция резюме

            // Достаём фото профиля
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
                    activities = group.Select(g => new ActivityDtos
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
                        age = group.First().worker.age,
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

            string connectionString = $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
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
                curDate >= d.date_start&&
                curDate <= d.date_end &&
                d.company_id == currentIdUser.ToString());
            if (curDeal == null)
            {
                return NotFound("No current deal");
            }
            
            int vacancyCountTarrif = _dbContext.Tarrifs.Where(t => t.id == curDeal.tariff_id).Select(t => t.vacancy_count).FirstOrDefault();
            int vacancyCountUser = _dbContext.Vacancies.Count(v => v.company_id == currentIdUser.ToString());
            if (vacancyCountUser >= vacancyCountTarrif)
            {
                return BadRequest("Vacancy count is too big");
            }
            

            string connectionString = $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sqlQuery =
                    "INSERT INTO vacancy_cur_user(company_id, post, min_salary, max_salary, education_id, experience, description,income_date,createdBy)" +
                    "values(@company_id,@post,@min_salary,@max_salary,@education_id,@experience,@description,CURRENT_TIMESTAMP(),substring_index(user(),'@',1));" +
                    "SELECT LAST_INSERT_ID();";
                ulong id = await db.ExecuteScalarAsync<ulong>(sqlQuery, new {company_id = currentIdUser, newVacancy.post, newVacancy.min_salary, newVacancy.max_salary, newVacancy.education_id, newVacancy.experience,newVacancy.description});
                
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

            string connectionString = $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";

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

            string connectionString = $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
        
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
            string connectionString = $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
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
                    ulong cur_id = await db.ExecuteScalarAsync<ulong>(sqlQuery, new {vacancy_id = newFilter.id, typeOfActivity_id = activity_id});
                    id.Add(cur_id);
                }
               
                
                return Ok(new { id = id });
            };
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
            string connectionString = $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sqlQuery = @"
                DELETE FROM Vacancy_filter WHERE filter_id = @filterId;
                ";
                int rowCount = await db.ExecuteAsync(sqlQuery, new {filterId = filterId});
                
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
            return Ok(new { tarrifs = tarrifs});
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
            return Ok(new { id = newDeal.id});
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
            company company = _dbContext.companies.FirstOrDefault(c => c.id == currentIdUser.ToString())!;
            CompanyDto companyDto = new CompanyDto
            {
                id = company.id,
                name = company.name,
                email = company.email,
                phoneNumber = company.phoneNumber,
                website = company.website,
                latitude = company.office_coord?.Y.ToString(CultureInfo.InvariantCulture), // Широта (Y)
                longitude = company.office_coord?.X.ToString(CultureInfo.InvariantCulture)  // Долгота (X)
               
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

            string connectionString = $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
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

                return Ok(new {feedbacks = newfeedbackDto});
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

            string connectionString = $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
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
                
                return Ok(new {id = id});    
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
                await db.ExecuteAsync(sqlQuery, new {status = request.status, id = request.feedback_id });
                return Ok(new { id = request.feedback_id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while accept feedbacks by company");
            return BadRequest(500);
        }
    }
}