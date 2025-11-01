using FeedbackService.DAL.Data;
using FeedbackService.DAL.Entities;

namespace FeedbackService.DAL.Repositories.Interfaces;

public interface IResumeRepository
{
    Task<Guid> AddResumeAsync(Resume resume);
    Task DeleteResumeAsync(Guid resumeId);
    Task<Resume> GetResumeAsync(Guid resumeId);
}