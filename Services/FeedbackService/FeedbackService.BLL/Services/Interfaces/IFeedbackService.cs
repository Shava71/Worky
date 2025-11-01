using FeedbackService.DAL.Entities;

namespace FeedbackService.BLL.Services.Interfaces;

public interface IFeedbackService
{
    Task<Feedback?> GetFeedbackByIdAsync(Guid feedbackId, Guid userId);
    Task<Guid> AddFeedbackAsync(Guid resumeId, Guid vacancyId);
    Task<Guid?> DeleteFeedbackAsync(Guid feedbackId, Guid userId);
    Task ChangeStatusAsync(Guid feedbackId, FeedbackStatus status, Guid userId);
    Task<IEnumerable<Feedback>?> GetAllFeedbacksAsync(Guid userId, Guid? Id);
}