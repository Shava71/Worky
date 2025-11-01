using FeedbackService.BLL.Services.Interfaces;
using FeedbackService.DAL.Entities;
using FeedbackService.DAL.Repositories.Interfaces;

namespace FeedbackService.BLL.Services.Implementations;

public class FeedbackService : IFeedbackService
{
    IFeedbackRepository _repository;

    public FeedbackService(IFeedbackRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<Feedback>?> GetAllFeedbacksAsync(Guid userId, Guid? Id)
    {
        return await _repository.GetAllFeedbacksAsync(userId, Id);
    }

    public async Task<Feedback?> GetFeedbackByIdAsync(Guid feedbackId, Guid userId)
    {
        return await _repository.GetFeedbackByIdAsync(feedbackId, userId);
    }

    public async Task<Guid> AddFeedbackAsync(Guid resumeId, Guid vacancyId)
    {
        Feedback feedback = new Feedback()
        {
            Id = Guid.NewGuid(),
            resumeId = resumeId,
            vacancyId = vacancyId,
        };
        Guid id = await _repository.AddFeedbackAsync(feedback);
        return id;
    }

    public async Task<Guid?> DeleteFeedbackAsync(Guid feedbackId, Guid userId)
    {
        
        Guid? id = await _repository.DeleteFeedbackAsync(feedbackId, userId);
        return id;
    }

    public async Task ChangeStatusAsync(Guid feedbackId, FeedbackStatus status, Guid userId)
    {
        Feedback feedback = await _repository.GetFeedbackByIdAsync(feedbackId, userId);
        if (feedback == null)
        {
            throw new KeyNotFoundException($"Feedback with id {feedbackId} not found");
        }
        await _repository.ChangeStatusAsync(feedback, status);
    }
    
}