using FeedbackService.DAL.Entities;

namespace FeedbackService.DAL.Repositories.Interfaces;

public interface IFeedbackRepository
{
    Task<Guid> AddFeedbackAsync(Feedback? feedback);
    Task<Guid?> DeleteFeedbackAsync(Guid feedbackId, Guid userId);
    Task<Feedback?> GetFeedbackByIdAsync(Guid feedbackId, Guid userId);
    Task ChangeStatusAsync(Feedback feedback, FeedbackStatus status);
    Task<IEnumerable<Feedback>?> GetAllFeedbacksAsync(Guid userId, Guid? id);
}