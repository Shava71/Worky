using Microsoft.EntityFrameworkCore;
using Worky.Context;
using Worky.Migrations;
using Worky.Repositories.Interfaces;

namespace Worky.Repositories.Implementations;

public class CompanyRepository : ICompanyRepository
{
    private readonly WorkyDbContext _dbContext;
    private readonly string _connectionString;

    public CompanyRepository(WorkyDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task CreateCompanyAsync(Company company)
    {
        await _dbContext.companies.AddAsync(company);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Company> GetCompanyByIdAsync(string id)
    {
        return await _dbContext.companies.FindAsync(id);
    }

    public async Task UpdateCompanyAsync(Company company)
    {
        _dbContext.companies.Update(company);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<ulong>> GetVacanciesByCompanyAsync(string companyId)
    {
        return await _dbContext.Vacancies.Where(v => v.company_id == companyId).Select(v => v.id).ToListAsync();
    }

    public async Task<IEnumerable<Feedback>> GetFeedbacksByVacanciesAsync(IEnumerable<ulong> vacancyIds, DateOnly start, DateOnly end)
    {
        return await _dbContext.Feedbacks.Where(f => vacancyIds.Contains(f.vacancy_id) && f.income_date >= start && f.income_date <= end).ToListAsync();
    }
}