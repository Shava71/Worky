using System.Data;
using System.Globalization;
using CompanyService.DAL.Contracts;
using CompanyService.DAL.Data;
using CompanyService.DAL.Data.DbConnection.Interface;
using CompanyService.DAL.DTO;
using CompanyService.DAL.Entities;
using CompanyService.DAL.Entities.TypeConfiguration;
using CompanyService.DAL.HttpClients.Clients;
using CompanyService.DAL.Models;
using CompanyService.DAL.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Worky.Repositories.Implementations;

public class VacancyRepository : IVacancyRepository
{
        private readonly CompanyDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IDbConnectionFactory _connectionFactory;


        public VacancyRepository(CompanyDbContext dbContext, IConfiguration configuration, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _connectionFactory = connectionFactory;
        }

        // public async Task<IEnumerable<VacancyDtos>> GetVacanciesAsync(GetVacanciesRequest request)
        // {
        //     var vacanciesQuery = _dbContext.Vacancies
        //         .Where(vacancy => !_dbContext.Feedbacks.Any(f => f.vacancy_id == vacancy.id && f.status == FeedbackStatus.Accepted))
        //         .Join(_dbContext.Vacancy_filters, vacancy => vacancy.id, filter => filter.vacancy_id, (vacancy, filter) => new { vacancy, filter })
        //         .Join(_dbContext.typeOfActivities, arg => arg.filter.typeOfActivity_id, activity => activity.id, (arg, activity) => new { arg.vacancy, arg.filter, activity })
        //         .Join(_dbContext.companies, arg => arg.vacancy.company_id, company => company.id, (arg, company) => new { arg.vacancy, arg.filter, arg.activity, company })
        //         .AsNoTracking()
        //         .AsQueryable();
        //
        //     if (request.id.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.id == request.id);
        //     if (request.min_experience.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.experience >= request.min_experience);
        //     if (request.max_experience.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.experience <= request.max_experience);
        //     if (request.min_wantedSalary.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.min_salary >= request.min_wantedSalary || r.vacancy.max_salary >= request.min_wantedSalary);
        //     if (request.max_wantedSalary.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.min_salary <= request.max_wantedSalary || r.vacancy.max_salary <= request.max_wantedSalary);
        //     if (request.income_date.HasValue)
        //     {
        //         var date = request.income_date.Value.Date;
        //         var nextDate = date.AddDays(1);
        //         vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.income_date >= date && r.vacancy.income_date < nextDate);
        //     }
        //     if (request.education.HasValue) vacanciesQuery = vacanciesQuery.Where(r => r.vacancy.education_id == (ulong?)request.education);
        //     if (!string.IsNullOrWhiteSpace(request.type)) vacanciesQuery = vacanciesQuery.Where(r => r.activity.type == request.type);
        //     if (request.direction?.Count > 0)
        //     {
        //         var vacancyIdDirection = vacanciesQuery.Where(r => request.direction.Contains(r.activity.direction)).Select(r => r.vacancy.id).ToHashSet();
        //         vacanciesQuery = vacanciesQuery.Where(r => vacancyIdDirection.Contains(r.vacancy.id));
        //     }
        //
        //     if (!string.IsNullOrEmpty(request.SortItem))
        //     {
        //         vacanciesQuery = request.Order?.ToLower() == "desc"
        //             ? request.SortItem.ToLower() switch
        //             {
        //                 "experience" => vacanciesQuery.OrderByDescending(x => x.vacancy.experience),
        //                 "income_date" => vacanciesQuery.OrderByDescending(x => x.vacancy.income_date),
        //                 _ => vacanciesQuery.OrderByDescending(x => x.vacancy.id)
        //             }
        //             : request.SortItem.ToLower() switch
        //             {
        //                 "experience" => vacanciesQuery.OrderBy(x => x.vacancy.experience),
        //                 "income_date" => vacanciesQuery.OrderBy(x => x.vacancy.income_date),
        //                 _ => vacanciesQuery.OrderBy(x => x.vacancy.id)
        //             };
        //     }
        //
        //     if (!string.IsNullOrEmpty(request.search))
        //     {
        //         vacanciesQuery = vacanciesQuery.Where(v => v.company.name.ToLower().Contains(request.search.ToLower()) || v.vacancy.description.ToLower().Contains(request.search.ToLower()) || v.vacancy.post.ToLower().Contains(request.search.ToLower()));
        //     }
        //
        //     var grouped = vacanciesQuery.AsEnumerable().GroupBy(x => x.vacancy.id).Select(group => new VacancyDtos
        //     {
        //         id = group.First().vacancy.id,
        //         company_id = group.First().vacancy.company_id,
        //         post = group.First().vacancy.post,
        //         min_salary = group.First().vacancy.min_salary,
        //         education_id = group.First().vacancy.education_id,
        //         experience = group.First().vacancy.experience,
        //         description = group.First().vacancy.description,
        //         income_date = group.First().vacancy.income_date,
        //         max_salary = group.First().vacancy.max_salary,
        //         activities = group.Select(g => new ActivityDtos { id = g.activity.id, direction = g.activity.direction, type = g.activity.type }).Distinct().ToList(),
        //         company = new CompanyDto
        //         {
        //             id = group.First().company.id,
        //             name = group.First().company.name,
        //             email = group.First().company.email,
        //             phoneNumber = group.First().company.phoneNumber,
        //             website = group.First().company.website,
        //             latitude = group.First().company.office_coord?.Y.ToString(CultureInfo.InvariantCulture),
        //             longitude = group.First().company.office_coord?.X.ToString(CultureInfo.InvariantCulture)
        //         }
        //     }).ToList();
        //
        //     return grouped;
        // }

        public async Task<VacancyDtos?> GetVacancyByIdAsync(Guid id)
        {
            var vacancy = await _dbContext.vacancy
                .AsNoTracking()
                .Include(v => v.company)
                .Include(v => v.Vacancy_filters)
                .Include(v => v.education)
                .Where(v => v.id == id)
                .Select(v => new VacancyDtos
                {
                    id = v.id,
                    company_id = v.company_id,
                    post = v.post,
                    experience = v.experience,
                    income_date = v.income_date,
                    education_id = v.education_id,
                    education_name = v.education.name,
                    description = v.description,
                    min_salary = v.min_salary,
                    max_salary = v.max_salary,
                    work_format = v.work_format,
                    work_hour = v.work_hour,

                    activities = v.Vacancy_filters.Select(f => new TypeOfActivityResponse()
                    {
                        id = f.typeOfActivity_id,
                        direction = "", // допилить в bll
                        type = ""
                    }).ToList(),
                    
                    company = new CompanyDto
                    {
                        id = v.company.UserId,
                        name = v.company.name,
                        email = v.company.email,
                        phoneNumber = v.company.phoneNumber,
                        website = v.company.website,
                        latitude = v.company.latitude,
                        longitude = v.company.longitude,
                    }
                })
                .FirstOrDefaultAsync();

            return vacancy;
        }

        public async Task<Guid> CreateVacancyAsync(CreateVacancy vacancy, string companyId)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                string sql = """
                    INSERT INTO "vacancy" (id,company_id, post, min_salary, max_salary, experience, education_id, description, income_date, work_format, work_hour) 
                    VALUES (@id, @company_id, @post, @min_salary, @max_salary, @experience, @education_id, @description, @income_date, @work_format, @work_hour)
                    returning id;
                    """;
                return await db.ExecuteScalarAsync<Guid>(sql, new
                {
                    id = Guid.NewGuid(),
                    company_id = Guid.Parse(companyId),
                    post = vacancy.post,
                    min_salary = vacancy.min_salary,
                    max_salary = vacancy.max_salary,
                    experience = vacancy.experience,
                    education_id = vacancy.education_id,
                    description = vacancy.description,
                    income_date = DateTime.UtcNow.Date,
                    work_format = vacancy.work_format,
                    work_hour = vacancy.work_hour
                });
            }
        }

        public async Task UpdateVacancyAsync(UpdateVacancy vacancy, Guid companyId)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                string sql = """
                    UPDATE "vacancy" SET post = @post, min_salary = @min_salary, max_salary = @max_salary, experience = @experience, education_id = @education_id, description = @description,
                                         work_format = @work_format, work_hour = @work_hour
                                     WHERE id = @id AND company_id = @company_id;
                    """;
                await db.ExecuteAsync(sql, new
                {
                    id = vacancy.Id,
                    post = vacancy.Post,
                    min_salary = vacancy.MinSalary,
                    max_salary = vacancy.MaxSalary,
                    experience = vacancy.Experience,
                    education_id = vacancy.EducationId,
                    description = vacancy.Description,
                    work_format = vacancy.WorkFormat,
                    work_hour = vacancy.WorkHour,
                    
                    company_id = companyId
                });
            }
        }

        public async Task DeleteVacancyAsync(Guid id, Guid companyId)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                string sql = """ DELETE FROM "vacancy" WHERE id = @id and company_id = @company_id """;
                await db.ExecuteAsync(sql, new { id = id, company_id = companyId });
            }
        }

        public async Task<IEnumerable<Guid>> AddVacancyFiltersAsync(AddFilter filter)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                List<Guid> ids = new List<Guid>();
                foreach (int activityId in filter.typeOfActivity_id)
                {
                    string sql = """
                        INSERT INTO "vacancy_filter" (filter_id, vacancy_id, "typeOfActivity_id") 
                        VALUES (@filter_id, @vacancy_id, @typeOfActivity_id) 
                        RETURNING filter_id; 
                        """;
                    Guid curId = await db.ExecuteScalarAsync<Guid>(sql, new {filter_id = Guid.NewGuid() ,vacancy_id = filter.id, typeOfActivity_id = activityId });
                    ids.Add(curId);
                }
                return ids;
            }
        }

        public async Task DeleteVacancyFilterAsync(Guid filterId)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                string sql = """DELETE FROM "vacancy_filter" WHERE filter_id = @filterId""";
                await db.ExecuteAsync(sql, new { filterId });
            }
        }

        public async Task<IEnumerable<VacancyDtos>> GetMyVacanciesAsync(string companyId, Guid? vacancyId)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                string sql = """
                                 SELECT v.*, vf."typeOfActivity_id"
                                 FROM "vacancy" v
                                 LEFT JOIN "vacancy_filter" vf ON v.id = vf.vacancy_id
                                 WHERE v.company_id = @companyId
                                   AND (@vacancyId IS NULL OR v.id = @vacancyId);
                             """;

                Guid parsedCompanyId = Guid.Parse(companyId);
                var rows = await db.QueryAsync(sql, new { companyId = parsedCompanyId, vacancyId });

                var grouped = rows.GroupBy(r => (Guid)r.id);

                var vacancies = grouped.Select(group => new VacancyDtos
                {
                    id = group.Key,
                    company_id = group.First().company_id,
                    post = group.First().post,
                    experience = group.First().experience,
                    income_date = group.First().income_date,
                    education_id = group.First().education_id,
                    description = group.First().description,
                    min_salary = group.First().min_salary,
                    max_salary = group.First().max_salary,
                    work_format = (WorkFormat?)group.First().work_format,
                    work_hour = (WorkHour?)group.First().work_hour,

                    activities = group
                        .Where(g => g.typeOfActivity_id != null)
                        .Select(g => new TypeOfActivityResponse
                        {
                            id = (int)g.typeOfActivity_id,
                            direction = null,
                            type = null
                        })
                        .DistinctBy(a => a.id)
                        .ToList()
                });

                return vacancies.ToList();
            }
        }
}