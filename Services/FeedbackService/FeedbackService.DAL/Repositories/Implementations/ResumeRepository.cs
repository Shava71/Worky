using FeedbackService.DAL.Data;
using FeedbackService.DAL.Entities;
using FeedbackService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FeedbackService.DAL.Repositories.Implementations;

public class ResumeRepository : IResumeRepository
{
    private readonly FeedbackDbContext _dbcontext;
    private ILogger<ResumeRepository> _logger;

    public ResumeRepository(FeedbackDbContext dbcontext, ILogger<ResumeRepository> logger)
    {
        _dbcontext = dbcontext;
        _logger = logger;
    }
    
    public async Task<Guid> AddResumeAsync(Resume resume)
    {
        try
        {
            await _dbcontext.resume.AddAsync(resume);
            await _dbcontext.SaveChangesAsync();
            return resume.resumeId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw ex;
        }
     
    }

    public async Task DeleteResumeAsync(Guid resumeId)
    {
        try
        {
            Resume? resume = await GetResumeAsync(resumeId);
            _dbcontext.resume.Remove(resume);
            await _dbcontext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw ex;
        }
    }

    public async Task<Resume?> GetResumeAsync(Guid resumeId)
    {
        try
        {
            return await _dbcontext.resume.Where(r => r.resumeId == resumeId).AsNoTracking().FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw ex;
        }
    }
}