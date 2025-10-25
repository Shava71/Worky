using System.Data;
using System.Globalization;
using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Worky.Context;
using Worky.Contracts;
using Worky.DTO;
using Worky.Migrations;

namespace Worky.Controllers;

[Authorize(Roles = "Worker", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/v1/[controller]")]
public class OldWorkerController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly WorkyDbContext _dbContext;
    UserManager<Users> _userManager;
    ILogger<OldCompanyController> _logger;

    public OldWorkerController(WorkyDbContext dbContext, ILogger<OldCompanyController> logger, UserManager<Users> userManager,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userManager = userManager;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpGet("Vacancies")]
    public async Task<IActionResult> FilterVacancy([FromQuery] GetVacanciesRequest? request)
    {
        try
        {
            var vacanciesQuery = _dbContext.Vacancies
                .Where(vacancy =>
                    !_dbContext.Feedbacks
                        .Any(f => f.vacancy_id == vacancy.id && f.status == FeedbackStatus.Accepted))
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
                    company => company.id,
                    (arg, company) => new { arg.vacancy, arg.filter, arg.activity, company })
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
            if ((bool)request?.min_wantedSalary.HasValue)
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
                    .Where(r => request.direction!.Contains(r.activity.direction)).Select(r => r.vacancy.id)
                    .ToHashSet();
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
                        latitude = group.First().company.office_coord?.Y
                            .ToString(CultureInfo.InvariantCulture)!, // Широта (Y)
                        longitude = group.First().company.office_coord?.X
                            .ToString(CultureInfo.InvariantCulture)! // Долгота (X)
                    }
                }).ToList();

            return Ok(new { resumes = groupedResumesDtos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while show resumes by company");
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
                    company => company.id,
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
                        latitude = group.First().company.office_coord?.Y
                            .ToString(CultureInfo.InvariantCulture)!, // Широта (Y)
                        longitude = group.First().company.office_coord?.X
                            .ToString(CultureInfo.InvariantCulture)! // Долгота (X)
                    }
                }).ToList();
            return Ok(new { vacancy = groupedResumesDtos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while show vacancies by worker");
            return BadRequest(500);
        }
    }


    [HttpGet("MyResume")]
    public async Task<ActionResult> GetMyResume([FromQuery] ulong? resumeId)
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
                    FROM resume_cur_user v
                    LEFT JOIN Resume_filter f ON v.id = f.resume_id
                    LEFT JOIN typeOfActivity a ON f.typeOfActivity_id = a.id";

                var result = await db.QueryAsync(sqlQuery);
                var resumeDict = new Dictionary<ulong, ResumeDtos>();

                foreach (var row in result)
                {
                    ulong id = (ulong)row.id;

                    if (!resumeDict.TryGetValue(id, out var resume))
                    {
                        resume = new ResumeDtos()
                        {
                            id = row.id,
                            worker_id = row.worker_id,
                            post = row.post,
                            skill = row.skill,
                            city = row.city,
                            experience = row.experience,
                            education_id = row.education_id,
                            income_date = row.income_date,
                            wantedSalary = row.wantedSalary,
                            activities = new List<ActivityDtos>()
                        };
                        resumeDict.Add(id, resume);
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
                        resume.activities!.Add(activity);
                    }
                }

                var resumeDto = resumeDict.Values.ToList();
                if (resumeId.HasValue) // Сортировка по id
                {
                    resumeDto = resumeDto.Where(v => v.id == resumeId).ToList();
                }

                return Ok(resumeDto);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while get my vacancy by company");
            return BadRequest(500);
        }
    }

    [HttpPost("CreateResume")]
    public async Task<ActionResult> CreateResume([FromBody] CreateResume newResume)
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

            string connectionString =
                $"Server=localhost;Database=Worky;User={user.UserName};Password={user.PasswordHash};";
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sqlQuery =
                    "INSERT INTO resume_cur_user(worker_id, post, skill, city, experience, education_id, wantedSalary,income_date,createdBy)" +
                    "values(@worker_id, @post, @skill, @city, @experience, @education_id, @wantedSalary, @income_date,substring_index(user(),'@',1));" +
                    "SELECT LAST_INSERT_ID();";
                ulong id = await db.ExecuteScalarAsync<ulong>(sqlQuery, new
                {
                    worker_id = currentIdUser, newResume.post, newResume.skill, newResume.city, newResume.experience,
                    newResume.education_id, newResume.wantedSalary, income_date = dateTime
                });

                return Ok(new { id = id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while create resume by worker");
            return BadRequest(500);
        }
    }

    [HttpPut("UpdateResume")]
    public async Task<ActionResult> UpdateResume([FromBody] UpdateResume updatedResume)
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
                    UPDATE resume_cur_user 
                    SET 
                        skill = @skill,
                        city = @city,
                        experience = @experience,
                        education_id = @education_id,
                        wantedSalary = @wantedSalary,
                        post = @post
                    WHERE id = @id";

                var parameters = new
                {
                    id = updatedResume.id,
                    skill = updatedResume.skill,
                    city = updatedResume.city,
                    experience = updatedResume.experience,
                    education_id = updatedResume.education_id,
                    wantedSalary = updatedResume.wantedSalary,
                    post = updatedResume.post
                };

                int rowsCount = await db.ExecuteAsync(updateSql, parameters);

                if (rowsCount == 0)
                    return StatusCode(500, "Failed to update resume");

                return Ok(new { message = $"resume {updatedResume.id} updated successfully" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating resume");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("DeleteResume")]
    public async Task<ActionResult> DeleteResume([FromQuery] ulong resumeId)
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
                string deleteSql = "DELETE FROM resume_cur_user WHERE id = @resumeId";
                int rowsAffected = await db.ExecuteAsync(deleteSql, new { resumeId = resumeId });

                if (rowsAffected == 0)
                    return StatusCode(500, "Failed to delete vacancy");

                return Ok($"resume {resumeId} deleted successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting resume");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("AddResumeFilter")]
    public async Task<ActionResult> AddResumeFilter([FromBody] AddFilter newFilter)
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
                    INSERT INTO Resume_filter(Resume_id,typeOfActivity_id)
                    values (
                            @resume_id,
                            @typeOfActivity_id
                    );
                    SELECT LAST_INSERT_ID();
                    ";
                    ulong cur_id = await db.ExecuteScalarAsync<ulong>(sqlQuery,
                        new { resume_id = newFilter.id, typeOfActivity_id = activity_id });
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

    [HttpDelete("DeleteResumeFilter")]
    public async Task<ActionResult> DeleteResumeFilter([FromQuery] ulong filterId)
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
                DELETE FROM Resume_filter WHERE filter_id = @filterId;
                ";
                int rowCount = await db.ExecuteAsync(sqlQuery, new { filterId = filterId });

                if (rowCount == 0)
                    return StatusCode(500, "Failed to delete resume-filter");
                return Ok("filter deleted");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while delete filter to vacancy by company");
            return BadRequest(500);
        }
    }


    [HttpGet("GetProfile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            byte[]? image = await _dbContext.Users.Where(u => u.Id == currentIdUser.ToString()).Select(u => u.image)
                .FirstOrDefaultAsync();

            Users user = _userManager.FindByIdAsync(currentIdUser.ToString()).Result!;
            Worker worker = _dbContext.Workers.FirstOrDefault(c => c.id == currentIdUser.ToString())!;
            WorkerDtos workerDto = new WorkerDtos
            {
                id = worker.id,
                second_name = worker.second_name,
                first_name = worker.first_name,
                surname = worker.surname,
                birthday = worker.birthday,
                image = image,
            };

            return Ok(new WorkerProfileDto
            {
                worker = workerDto,
                user = user,
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
                var req = _dbContext.Vacancies
                    .Where(req => req.id == request.vacancy_id)
                    .Select(req => req.company_id).FirstOrDefault();
                var company = await _userManager.FindByIdAsync(req);

                string username;
                if (company != null)
                {
                    username = company.UserName;
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
            _logger.LogError(ex, "An error occured while make feedbacks by worker");
            return BadRequest(500);
        }
    }

    [HttpDelete("DeleteFeedback")]
    public async Task<IActionResult> DeleteFeedback([FromQuery] ulong id)
    {
        // try
        // {
        //     string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //     using (IDbConnection db = new SqlConnection(connectionString))
        //     {
        //         string query = @"DELETE FROM Feedbacks WHERE id=@id";
        //         await db.ExecuteAsync(query);
        //         
        //     }
        //     return Ok($"Feedback id = {id} deleted");
        // }
        if (id == 0)
            return BadRequest(new { Message = "Fail id." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var feedback = await _dbContext.Feedbacks
            .Join(_dbContext.Resumes, feedback => feedback.resume_id, resume => resume.id,
                (feedback, resume) => new { feedback, resume })
            .Where(f => f.feedback.id == id &&
                        f.resume.worker_id == userId)
            .FirstOrDefaultAsync();

        if (feedback.feedback == null)
            return NotFound(new { Message = "Feedback not found." });

        try
        {
            _dbContext.Feedbacks.Remove(feedback.feedback);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while delete feedbacks by worker");
            return BadRequest(500);
        }
    }
}