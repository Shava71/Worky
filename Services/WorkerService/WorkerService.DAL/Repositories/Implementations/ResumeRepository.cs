using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using WorkerService.DAL.Contracts;
using WorkerService.DAL.Data;
using WorkerService.DAL.Data.DbConnection.Interface;
using WorkerService.DAL.DTO;
using WorkerService.DAL.Entities;
using WorkerService.DAL.Models;
using WorkerService.DAL.Repositories.Interfaces;


namespace WorkerService.DAL.Repositories.Implementations;

public class ResumeRepository : IResumeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
        private readonly WorkerDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ResumeRepository> _logger;

        public ResumeRepository(WorkerDbContext dbContext, IConfiguration configuration, ILogger<ResumeRepository> logger, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ResumeDtos>> GetResumesAsync(GetResumesRequest request)
    {
        try
        {
            using IDbConnection _connection = _connectionFactory.CreateConnection();
            
            var sortItem = request.SortItem?.ToLower();
            var order = request.Order?.ToLower() == "desc" ? "DESC" : "ASC";

            // Определяем поле сортировки
            string orderBy = sortItem switch
            {
                "city" => "r.city",
                "experience" => "r.experience",
                "income_date" => "r.income_date",
                _ => "r.id"
            };

            var sql = $"""
                SELECT 
                    r.id AS resume_id,
                    r.worker_id,
                    r.city,
                    r.experience,
                    r.income_date,
                    r.education_id,
                    r.post,
                    r.skill,
                    r."wantedSalary",
                    f.filter_id,
                    f."typeOfActivity_id"
                FROM "Resume" r
                LEFT JOIN "Resume_filter" f ON r.id = f.resume_id
                WHERE ( @id::uuid IS NULL OR r.id = @id )
                  AND ( @min_experience::int IS NULL OR r.experience >= @min_experience )
                  AND ( @max_experience::int IS NULL OR r.experience <= @max_experience )
                  AND ( @city::text IS NULL OR LOWER(r.city) LIKE LOWER(CONCAT('%', @city, '%')) )
                  AND ( @min_wantedSalary::int IS NULL OR r."wantedSalary" >= @min_wantedSalary )
                  AND ( @max_wantedSalary::int IS NULL OR r."wantedSalary" <= @max_wantedSalary )
                  AND ( @education::int IS NULL OR r.education_id = @education )
                  AND ( @income_date::timestamp IS NULL OR DATE(r.income_date) = @income_date )
                ORDER BY {orderBy} {order};
            """;

            var result = await _connection.QueryAsync(sql, new
            {
                id = request.id,
                min_experience = request.min_experience,
                max_experience = request.max_experience,
                city = request.city,
                min_wantedSalary = request.min_wantedSalary,
                max_wantedSalary = request.max_wantedSalary,
                education = request.education,
                income_date = request.income_date?.Date
            });

            var grouped = result
                .GroupBy(r => r.resume_id)
                .Select(group => new ResumeDtos
                {
                    id = group.Key,
                    worker_id = group.First().worker_id?.ToString(),
                    city = group.First().city,
                    experience = group.First().experience,
                    income_date = group.First().income_date,
                    education_id = group.First().education_id,
                    post = group.First().post,
                    skill = group.First().skill,
                    wantedSalary = group.First().wantedSalary,
                    activities = group
                        .Where(g => g.typeOfActivity_id != null)
                        .Select(g => new ActivityDtos
                        {
                            id = g.typeOfActivity_id,
                            direction = null,
                            type = null
                        }).ToList()
                })
                .ToList();

            return grouped;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetResumesAsync");
            throw;
        }
    }

        public async Task<ResumeDtos> GetResumeByIdAsync(Guid id)
        {
            var resume = await _dbContext.Resume
                .AsNoTracking()
                .Include(r => r.worker)
                .Include(r => r.resume_filters)
                .ThenInclude(f => f.resume)
                .Include(r => r.education)
                .Where(r => r.id == id)
                .Select(r => new ResumeDtos
                {
                    id = r.id,
                    worker_id = r.worker.UserId.ToString(),
                    city = r.city,
                    experience = r.experience,
                    income_date = r.income_date,
                    education_id = r.education_id,
                    post = r.post,
                    skill = r.skill,
                    wantedSalary = r.wantedSalary,
                    activities = r.resume_filters.Select(f => new ActivityDtos
                    {
                        id = f.typeOfActivity_id,
                        direction = "", // допилить в bll
                        type = ""
                    }).ToList(),
                    worker = new WorkerDtos
                    {
                        id = r.worker.UserId.ToString(),
                        first_name = r.worker.first_name,
                        second_name = r.worker.second_name,
                        surname = r.worker.surname,
                        birthday = r.worker.birthday,
                        //image = image // допилить на уровне bll
                    }
                })
                .FirstOrDefaultAsync();

            return resume;
        }

        public async Task<Guid> CreateResumeAsync(CreateResume resume, string workerId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new NpgsqlConnection(connectionString))
            {
                string sql = """
                INSERT INTO "Resume" (id, worker_id, post, skill, city, experience, education_id, "wantedSalary", income_date) 
                VALUES (@id, @worker_id, @post, @skill, @city, @experience, @education_id, @wantedSalary, @income_date)
                 returning id;
                """;
                _logger.LogInformation($"WorkerId = {workerId}");
                _logger.LogInformation(sql);
                _logger.LogInformation($"{resume.city}, {resume.experience}, {resume.education_id}, {resume.post}, {resume.skill}, {resume.wantedSalary}");
                return await db.ExecuteScalarAsync<Guid>(sql, new
                {
                    id = Guid.NewGuid(),
                    worker_id = Guid.Parse(workerId),
                    resume.post,
                    resume.skill,
                    resume.city,
                    resume.experience,
                    resume.education_id,
                    resume.wantedSalary,
                    income_date = DateTime.UtcNow.Date
                });
            }
        }

        public async Task UpdateResumeAsync(UpdateResume resume)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new NpgsqlConnection(connectionString))
            {
                string sql = """
                             UPDATE "Resume" SET skill = @skill, city = @city, experience = @experience, education_id = @education_id, "wantedSalary" = @wantedSalary, post = @post WHERE id = @id
                             """;
                      
                await db.ExecuteAsync(sql, resume);
            }
        }

        public async Task DeleteResumeAsync(Guid id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new NpgsqlConnection(connectionString))
            {
                string sql = """
                    DELETE FROM "Resume" WHERE id = @id
                    """;
                await db.ExecuteAsync(sql, new { id });
            }
        }

        public async Task<IEnumerable<Guid>> AddResumeFiltersAsync(AddFilter filter)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new NpgsqlConnection(connectionString))
            {
                List<Guid> ids = new List<Guid>();
                foreach (ulong activityId in filter.typeOfActivity_id)
                {
                    string sql = """
                        INSERT INTO "Resume_filter" (filter_id, resume_id, "typeOfActivity_id") VALUES (@filter_id ,@resume_id, @typeOfActivity_id) Returning filter_id;
                        """;
                    Guid curId = await db.ExecuteScalarAsync<Guid>(sql, new {filter_id = Guid.NewGuid(), resume_id = filter.id, typeOfActivity_id = activityId });
                    ids.Add(curId);
                }
                return ids;
            }
        }

        public async Task DeleteResumeFilterAsync(Guid filterId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new NpgsqlConnection(connectionString))
            {
                string sql = """
                    DELETE FROM "Resume_filter" WHERE filter_id = @filterId
                    """;
                await db.ExecuteAsync(sql, new { filterId });
            }
        }
        
        public async Task<IEnumerable<ResumeDtos>> GetMyResumesAsync(string workerId, Guid? resumeId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new NpgsqlConnection(connectionString))
            {
                string sql = """
                    SELECT r.*, rf.*, rf.filter_id AS filter_id
                    FROM "Resume" r
                    LEFT JOIN "Resume_filter" rf ON r.id = rf.resume_id
                    WHERE r.worker_id = @workerId;
                """;
                if (resumeId.HasValue) sql += " AND r.id = @resumeId";
                
                Guid parsedWorkerId = Guid.Parse(workerId);
                var result = await db.QueryAsync(sql, new { workerId = parsedWorkerId });
                var resumeDict = new Dictionary<Guid, ResumeDtos>();

                foreach (var row in result)
                {
                    Guid id = row.id;
                    if (!resumeDict.TryGetValue(id, out var res))
                    {
                        res = new ResumeDtos
                        {
                            id = id,
                            worker_id = row.worker_id.ToString(),
                            post = row.post,
                            skill = row.skill,
                            city = row.city,
                            experience = row.experience,
                            education_id = row.education_id,
                            income_date = row.income_date,
                            wantedSalary = row.wantedSalary,
                            activities = new List<ActivityDtos>()
                        };
                        resumeDict.Add(id, res);
                    }
                    // if (row.a_id != null)
                    // {
                    //     res.activities.Add(new ActivityDtos { id = row.a_id, type = row.a_type, direction = row.a_direction });
                    // }
                }
                return resumeDict.Values;
            }
        }
}