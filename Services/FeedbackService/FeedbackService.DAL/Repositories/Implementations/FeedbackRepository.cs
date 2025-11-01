using FeedbackService.DAL.Data;
using FeedbackService.DAL.Entities;
using FeedbackService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FeedbackService.DAL.Repositories.Implementations;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly FeedbackDbContext _dbContext;
    private readonly ILogger<FeedbackRepository> _logger;

    public FeedbackRepository(FeedbackDbContext dbContext, ILogger<FeedbackRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<Guid> AddFeedbackAsync(Feedback? feedback)
    {
        try
        {
            await _dbContext.feedback.AddAsync(feedback);
            await _dbContext.SaveChangesAsync();
            return feedback.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<Guid?> DeleteFeedbackAsync(Guid feedbackId, Guid userId)
    {
        try
        {
            Feedback? feedback = await GetFeedbackByIdAsync(feedbackId, userId);
            if(feedback == null)
                return null;
            
            _dbContext.feedback.Remove(feedback);
            await _dbContext.SaveChangesAsync();
            return feedback.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<Feedback?> GetFeedbackByIdAsync(Guid feedbackId, Guid userId)
    {
        return await _dbContext.feedback.Where(f => f.Id == feedbackId && (f.vacancy.companyId == userId ||
                                                   f.resume.workerId == userId)).FirstOrDefaultAsync();
    }

    
    public async Task<IEnumerable<Feedback>?> GetAllFeedbacksAsync(Guid userId, Guid? id)
    {
        var query =  _dbContext.feedback.Where(f =>
            f.vacancy.companyId == userId ||
            f.resume.workerId == userId).AsQueryable();

        if (id.HasValue)
        {
            query = query.Where(f => f.vacancyId == id || f.resumeId == id);
        }
        return await query.ToListAsync();
    }

    public async Task ChangeStatusAsync(Feedback feedback, FeedbackStatus status)
    {
        feedback.status = status;
        await _dbContext.SaveChangesAsync();
    }
}