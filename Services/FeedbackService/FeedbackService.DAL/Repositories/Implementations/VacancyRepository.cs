using FeedbackService.DAL.Data;
using FeedbackService.DAL.Entities;
using FeedbackService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FeedbackService.DAL.Repositories.Implementations;

public class VacancyRepository : IVacancyRepository
{
    private readonly FeedbackDbContext _dbcontext;
    private readonly ILogger<VacancyRepository> _logger;

    public VacancyRepository(FeedbackDbContext dbcontext, ILogger<VacancyRepository> logger)
    {
        _dbcontext = dbcontext;
        _logger = logger;
    }
    
    public async Task<Guid> AddVacancyAsync(Vacancy vacancy)
    {
        try
        {
            await _dbcontext.vacancy.AddAsync(vacancy);
            await _dbcontext.SaveChangesAsync();
            return vacancy.vacancyId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw ex;
        }
    }

    public async Task DeleteVacancyAsync(Guid vacancyId)
    {
        try
        {
            Vacancy? vacancy = await GetVacacnyAsync(vacancyId);
            if (vacancy == null)
            {
                return;
            }
            _dbcontext.vacancy.Remove(vacancy);
            await _dbcontext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw ex;
        }
    }

    public async Task<Vacancy> GetVacacnyAsync(Guid vacancyId)
    {
        try
        {
            return await _dbcontext.vacancy.Where(r => r.vacancyId == vacancyId).AsNoTracking().FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw ex;
        }
    }
}