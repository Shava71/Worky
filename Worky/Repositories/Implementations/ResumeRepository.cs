using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Worky.Context;
using Worky.Contracts;
using Worky.DTO;
using Worky.Migrations;

namespace Worky.Repositories.Implementations;

public class ResumeRepository
{
    private readonly WorkyDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public ResumeRepository(WorkyDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<IEnumerable<ResumeDtos>> GetResumesAsync(GetResumesRequest request)
        {
            var resumesQuery = _dbContext.Resumes
                .Where(resume => !_dbContext.Feedbacks.Any(f => f.resume_id == resume.id && f.status == FeedbackStatus.Accepted))
                .Join(_dbContext.Resume_filters, resume => resume.id, filter => filter.resume_id, (resume, filter) => new { resume, filter })
                .Join(_dbContext.typeOfActivities, arg => arg.filter.typeOfActivity_id, activity => activity.id, (arg, activity) => new { arg.resume, arg.filter, activity })
                .AsNoTracking()
                .AsQueryable();

            if (request.id.HasValue) resumesQuery = resumesQuery.Where(r => r.resume.id == request.id);
            if (request.min_experience.HasValue) resumesQuery = resumesQuery.Where(r => r.resume.experience >= request.min_experience);
            if (request.max_experience.HasValue) resumesQuery = resumesQuery.Where(r => r.resume.experience <= request.max_experience);
            if (!string.IsNullOrWhiteSpace(request.city)) resumesQuery = resumesQuery.Where(r => r.resume.city.ToLower().Contains(request.city.ToLower()));
            if (request.min_wantedSalary.HasValue) resumesQuery = resumesQuery.Where(r => r.resume.wantedSalary >= request.min_wantedSalary);
            if (request.max_wantedSalary.HasValue) resumesQuery = resumesQuery.Where(r => r.resume.wantedSalary <= request.max_wantedSalary);
            if (request.income_date.HasValue)
            {
                var date = request.income_date.Value.Date;
                var nextDate = date.AddDays(1);
                resumesQuery = resumesQuery.Where(r => r.resume.income_date >= date && r.resume.income_date < nextDate);
            }
            if (request.education.HasValue) resumesQuery = resumesQuery.Where(r => r.resume.education_id == (ulong?)request.education);
            if (!string.IsNullOrWhiteSpace(request.type)) resumesQuery = resumesQuery.Where(r => r.activity.type == request.type);
            if (request.direction?.Count > 0)
            {
                var resumesIdDirection = resumesQuery.Where(r => request.direction.Contains(r.activity.direction)).Select(r => r.resume.id).ToHashSet();
                resumesQuery = resumesQuery.Where(r => resumesIdDirection.Contains(r.resume.id));
            }

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
            
            // Strange AI-generated code
            // if (!string.IsNullOrEmpty(request.search))
            // {
            //     resumesQuery = resumesQuery.Where(v => v.resume.post.ToLower().Contains(request.search.ToLower()) || v.resume.skill.ToLower().Contains(request.search.ToLower()));
            // }

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
                    activities = group.Select(g => new ActivityDtos { id = g.activity.id, direction = g.activity.direction, type = g.activity.type }).Distinct().ToList()
                }).ToList();

            return groupedResumesDtos;
        }

        public async Task<ResumeDtos> GetResumeByIdAsync(ulong id)
        {
            var resumesQuery = from resume in _dbContext.Resumes
                               where resume.id == id
                               join worker in _dbContext.Workers on resume.worker_id equals worker.id
                               join filter in _dbContext.Resume_filters on resume.id equals filter.resume_id into filterGroup
                               from filter in filterGroup.DefaultIfEmpty()
                               join activity in _dbContext.typeOfActivities on filter.typeOfActivity_id equals activity.id into activityGroup
                               from activity in activityGroup.DefaultIfEmpty()
                               select new { resume, filter, activity, worker };

            string? userId = resumesQuery.AsEnumerable().Select(r => r.worker.id).FirstOrDefault();
            byte[]? image = await _dbContext.Users.Where(u => u.Id == userId).Select(u => u.image).FirstOrDefaultAsync();

            var grouped = resumesQuery.AsEnumerable().GroupBy(x => x.resume.id).Select(group => new ResumeDtos
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
                activities = group.Where(g => g.activity != null).Select(g => new ActivityDtos { id = g.activity.id, direction = g.activity.direction, type = g.activity.type }).Distinct().ToList(),
                worker = new WorkerDtos
                {
                    id = group.First().worker.id,
                    first_name = group.First().worker.first_name,
                    second_name = group.First().worker.second_name,
                    surname = group.First().worker.surname,
                    image = image,
                    birthday = group.First().worker.birthday
                }
            }).FirstOrDefault();

            return grouped;
        }

        public async Task<ulong> CreateResumeAsync(CreateResume resume, string workerId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = "INSERT INTO Resumes (worker_id, post, skill, city, experience, education_id, wantedSalary, income_date) VALUES (@worker_id, @post, @skill, @city, @experience, @education_id, @wantedSalary, @income_date); SELECT LAST_INSERT_ID();";
                return await db.ExecuteScalarAsync<ulong>(sql, new
                {
                    worker_id = workerId,
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
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Resumes SET skill = @skill, city = @city, experience = @experience, education_id = @education_id, wantedSalary = @wantedSalary, post = @post WHERE id = @id";
                await db.ExecuteAsync(sql, resume);
            }
        }

        public async Task DeleteResumeAsync(ulong id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM Resumes WHERE id = @id";
                await db.ExecuteAsync(sql, new { id });
            }
        }

        public async Task<IEnumerable<ulong>> AddResumeFiltersAsync(AddFilter filter)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                List<ulong> ids = new List<ulong>();
                foreach (ulong activityId in filter.typeOfActivity_id)
                {
                    string sql = "INSERT INTO Resume_filter (resume_id, typeOfActivity_id) VALUES (@resume_id, @typeOfActivity_id); SELECT LAST_INSERT_ID();";
                    ulong curId = await db.ExecuteScalarAsync<ulong>(sql, new { resume_id = filter.id, typeOfActivity_id = activityId });
                    ids.Add(curId);
                }
                return ids;
            }
        }

        public async Task DeleteResumeFilterAsync(ulong filterId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM Resume_filter WHERE filter_id = @filterId";
                await db.ExecuteAsync(sql, new { filterId });
            }
        }

        public async Task<IEnumerable<ResumeDtos>> GetMyResumesAsync(string workerId, ulong? resumeId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT r.*, rf.*, a.id AS a_id, a.type AS a_type, a.direction AS a_direction, rf.filter_id AS filter_id
                    FROM Resumes r
                    LEFT JOIN Resume_filter rf ON r.id = rf.resume_id
                    LEFT JOIN typeOfActivity a ON rf.typeOfActivity_id = a.id
                    WHERE r.worker_id = @workerId";
                if (resumeId.HasValue) sql += " AND r.id = @resumeId";

                var result = await db.QueryAsync(sql, new { workerId });
                var resumeDict = new Dictionary<ulong, ResumeDtos>();

                foreach (var row in result)
                {
                    ulong id = (ulong)row.id;
                    if (!resumeDict.TryGetValue(id, out var res))
                    {
                        res = new ResumeDtos
                        {
                            id = id,
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
                        resumeDict.Add(id, res);
                    }
                    if (row.a_id != null)
                    {
                        res.activities.Add(new ActivityDtos { id = row.a_id, type = row.a_type, direction = row.a_direction });
                    }
                }
                return resumeDict.Values;
            }
        }
}