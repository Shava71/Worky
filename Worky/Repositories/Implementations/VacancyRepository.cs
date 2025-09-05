using System.Data;
using System.Globalization;
using Dapper;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Worky.Context;
using Worky.Contracts;
using Worky.DTO;
using Worky.Migrations;

namespace Worky.Repositories.Implementations;

public class VacancyRepository
{
    private readonly WorkyDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public VacancyRepository(WorkyDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<IEnumerable<VacancyDtos>> GetVacanciesAsync(GetVacanciesRequest request)
        {
            var vacanciesQuery = _dbContext.Vacancies
                .Where(vacancy => !_dbContext.Feedbacks.Any(f => f.vacancy_id == vacancy.id && f.status == FeedbackStatus.Accepted))
                .Join(_dbContext.Vacancy_filters, vacancy => vacancy.id, filter => filter.vacancy_id, (vacancy, filter) => new { vacancy, filter })
                .Join(_dbContext.typeOfActivities, arg => arg.filter.typeOfActivity_id, activity => activity.id, (arg, activity) => new { arg.vacancy, arg.filter, activity })
                .Join(_dbContext.companies, arg => arg.vacancy.company_id, company => company.id, (arg, company) => new { arg.vacancy, arg.filter, arg.activity, company })
                .AsNoTracking()
                .AsQueryable();

            if (request.id.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.id == request.id);
            if (request.min_experience.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.experience >= request.min_experience);
            if (request.max_experience.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.experience <= request.max_experience);
            if (request.min_wantedSalary.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.min_salary >= request.min_wantedSalary || r.vacancy.max_salary >= request.min_wantedSalary);
            if (request.max_wantedSalary.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.min_salary <= request.max_wantedSalary || r.vacancy.max_salary <= request.max_wantedSalary);
            if (request.income_date.HasValue)
            {
                var date = request.income_date.Value.Date;
                var nextDate = date.AddDays(1);
                vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.income_date >= date && r.vacancy.income_date < nextDate);
            }
            if (request.education.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.education_id == (ulong?)request.education);
            if (!string.IsNullOrWhiteSpace(request.type)) vacanciesQuery = vacanciesQuery.Where(r => r.activity.type == request.type);
            if (request.direction?.Count > 0)
            {
                var vacancyIdDirection = vacanciesQuery.Where(r => request.direction.Contains(r.activity.direction)).Select(r => r.vacancy.id).ToHashSet();
                vacanciesQuery = vacanciesQuery.Where(r => vacancyIdDirection.Contains(r.vacancy.id));
            }

            if (!string.IsNullOrEmpty(request.SortItem))
            {
                vacanciesQuery = request.Order?.ToLower() == "desc"
                    ? request.SortItem.ToLower() switch
                    {
                        "experience" => vacanciesQuery.OrderByDescending(x => x.vacancy.experience),
                        "income_date" => vacanciesQuery.OrderByDescending(x => x.vacancy.income_date),
                        _ => vacanciesQuery.OrderByDescending(x => x.vacancy.id)
                    }
                    : request.SortItem.ToLower() switch
                    {
                        "experience" => vacanciesQuery.OrderBy(x => x.vacancy.experience),
                        "income_date" => vacanciesQuery.OrderBy(x => x.vacancy.income_date),
                        _ => vacanciesQuery.OrderBy(x => x.vacancy.id)
                    };
            }

            if (!string.IsNullOrEmpty(request.search))
            {
                vacanciesQuery = vacanciesQuery.Where(v => v.company.name.ToLower().Contains(request.search.ToLower()) || v.vacancy.description.ToLower().Contains(request.search.ToLower()) || v.vacancy.post.ToLower().Contains(request.search.ToLower()));
            }

            var grouped = vacanciesQuery.AsEnumerable().GroupBy(x => x.vacancy.id).Select(group => new VacancyDtos
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
                activities = group.Select(g => new ActivityDtos { id = g.activity.id, direction = g.activity.direction, type = g.activity.type }).Distinct().ToList(),
                company = new CompanyDto
                {
                    id = group.First().company.id,
                    name = group.First().company.name,
                    email = group.First().company.email,
                    phoneNumber = group.First().company.phoneNumber,
                    website = group.First().company.website,
                    latitude = group.First().company.office_coord?.Y.ToString(CultureInfo.InvariantCulture),
                    longitude = group.First().company.office_coord?.X.ToString(CultureInfo.InvariantCulture)
                }
            }).ToList();

            return grouped;
        }

        public async Task<VacancyDtos> GetVacancyByIdAsync(ulong id)
        {
            var vacanciesQuery = _dbContext.Vacancies
                .Join(_dbContext.Vacancy_filters, vacancy => vacancy.id, filter => filter.vacancy_id, (vacancy, filter) => new { vacancy, filter })
                .Join(_dbContext.typeOfActivities, arg => arg.filter.typeOfActivity_id, activity => activity.id, (arg, activity) => new { arg.vacancy, arg.filter, activity })
                .Join(_dbContext.companies, arg => arg.vacancy.company_id, company => company.id, (arg, company) => new { arg.vacancy, arg.filter, arg.activity, company })
                .Where(r => r.vacancy.id == id)
                .AsNoTracking();

            string? userId = vacanciesQuery.Select(r => r.company.id).FirstOrDefault();
            byte[]? image = await _dbContext.Users.Where(u => u.Id == userId).Select(u => u.image).FirstOrDefaultAsync();

            var grouped = vacanciesQuery.AsEnumerable().GroupBy(x => x.vacancy.id).Select(group => new VacancyDtos
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
                activities = group.Select(g => new ActivityDtos { id = g.activity.id, direction = g.activity.direction, type = g.activity.type }).Distinct().ToList(),
                company = new CompanyDto
                {
                    id = group.First().company.id,
                    name = group.First().company.name,
                    email = group.First().company.email,
                    phoneNumber = group.First().company.phoneNumber,
                    website = group.First().company.website,
                    latitude = group.First().company.office_coord?.Y.ToString(CultureInfo.InvariantCulture),
                    longitude = group.First().company.office_coord?.X.ToString(CultureInfo.InvariantCulture)
                }
            }).FirstOrDefault();

            return grouped;
        }

        public async Task<ulong> CreateVacancyAsync(CreateVacancy vacancy, string companyId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = "INSERT INTO Vacancies (company_id, post, min_salary, max_salary, experience, education_id, description, income_date) VALUES (@company_id, @post, @min_salary, @max_salary, @experience, @education_id, @description, @income_date); SELECT LAST_INSERT_ID();";
                return await db.ExecuteScalarAsync<ulong>(sql, new
                {
                    company_id = companyId,
                    vacancy.post,
                    vacancy.min_salary,
                    vacancy.max_salary,
                    vacancy.experience,
                    vacancy.education_id,
                    vacancy.description,
                    income_date = DateTime.UtcNow.Date
                });
            }
        }

        public async Task UpdateVacancyAsync(UpdateVacancy vacancy)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Vacancies SET post = @post, min_salary = @min_salary, max_salary = @max_salary, experience = @experience, education_id = @education_id, description = @description WHERE id = @id";
                await db.ExecuteAsync(sql, vacancy);
            }
        }

        public async Task DeleteVacancyAsync(ulong id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM Vacancies WHERE id = @id";
                await db.ExecuteAsync(sql, new { id });
            }
        }

        public async Task<IEnumerable<ulong>> AddVacancyFiltersAsync(AddFilter filter)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                List<ulong> ids = new List<ulong>();
                foreach (ulong activityId in filter.typeOfActivity_id)
                {
                    string sql = "INSERT INTO Vacancy_filter (vacancy_id, typeOfActivity_id) VALUES (@vacancy_id, @typeOfActivity_id); SELECT LAST_INSERT_ID();";
                    ulong curId = await db.ExecuteScalarAsync<ulong>(sql, new { vacancy_id = filter.id, typeOfActivity_id = activityId });
                    ids.Add(curId);
                }
                return ids;
            }
        }

        public async Task DeleteVacancyFilterAsync(ulong filterId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM Vacancy_filter WHERE filter_id = @filterId";
                await db.ExecuteAsync(sql, new { filterId });
            }
        }

        public async Task<IEnumerable<VacancyDtos>> GetMyVacanciesAsync(string companyId, ulong? vacancyId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT v.*, vf.*, a.id AS a_id, a.type AS a_type, a.direction AS a_direction, vf.filter_id AS filter_id
                    FROM Vacancies v
                    LEFT JOIN Vacancy_filter vf ON v.id = vf.vacancy_id
                    LEFT JOIN typeOfActivity a ON vf.typeOfActivity_id = a.id
                    WHERE v.company_id = @companyId";
                if (vacancyId.HasValue) sql += " AND v.id = @vacancyId";

                var result = await db.QueryAsync(sql, new { companyId });
                var vacancyDict = new Dictionary<ulong, VacancyDtos>();

                foreach (var row in result)
                {
                    ulong id = (ulong)row.id;
                    if (!vacancyDict.TryGetValue(id, out var vac))
                    {
                        vac = new VacancyDtos
                        {
                            id = id,
                            company_id = row.company_id,
                            post = row.post,
                            min_salary = row.min_salary,
                            max_salary = row.max_salary,
                            experience = row.experience,
                            education_id = row.education_id,
                            description = row.description,
                            income_date = row.income_date,
                            activities = new List<ActivityDtos>()
                        };
                        vacancyDict.Add(id, vac);
                    }

                    if (row.a_id != null)
                    {
                        vac.activities.Add(new ActivityDtos
                            { id = row.a_id, type = row.a_type, direction = row.a_direction });
                    }
                }

                return vacancyDict.Values;
            }
        }
}