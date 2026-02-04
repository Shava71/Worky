using CompanyService.DAL.Data;
using CompanyService.DAL.Entities;
using CompanyService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Worky.Repositories.Implementations;

public class CompanyRepository : ICompanyRepository
{
    private readonly CompanyDbContext _dbContext;
    private readonly string _connectionString;

    public CompanyRepository(CompanyDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task CreateCompanyAsync(Company company)
    {
        await _dbContext.company.AddAsync(company);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Company> GetCompanyByIdAsync(Guid id)
    {
        return await _dbContext.company
            .Include(c => c.Deals.OrderByDescending(d => d.date_start))
            .ThenInclude(d => d.tariff)
            .FirstOrDefaultAsync(c => c.UserId == id);
            
    }

    public async Task UpdateCompanyAsync(Company company)
    {
        _dbContext.company.Update(company);
        await _dbContext.SaveChangesAsync();
    }
    
}