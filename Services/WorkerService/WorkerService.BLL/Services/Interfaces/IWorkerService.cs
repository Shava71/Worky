
using WorkerService.DAL.Contracts;
using WorkerService.DAL.DTO;

namespace WorkerService.BLL.Services.Interfaces;

public interface IWorkerService
{
    // Task<IEnumerable<VacancyDtos>> FilterVacanciesAsync(GetVacanciesRequest request);
    // Task<VacancyDtos> GetVacancyInfoAsync(ulong vacancyId);
    Task<IEnumerable<ResumeDtos>> FilterResumesAsync(GetResumesRequest request);
    
    Task<ResumeDtos> GetResumeInfoAsync(Guid resumeId);
    Task<IEnumerable<ResumeDtos>> GetMyResumesAsync(string workerId, Guid? resumeId);
    Task<Guid> CreateResumeAsync(CreateResume resume, string workerId);
    Task UpdateResumeAsync(UpdateResume resume, string workerId);
    Task DeleteResumeAsync(Guid id, string workerId);
    Task<IEnumerable<Guid>> AddResumeFilterAsync(AddFilter filter, string workerId);
    Task DeleteResumeFilterAsync(Guid filterId, string workerId);
    Task<WorkerProfileDto> GetProfileAsync(string workerId, string token, CancellationToken cancellationToken = default);
    //Task<IEnumerable<FeedbackDtos>> GetFeedbacksAsync(string workerId, ulong? vacancyId);
    //Task<ulong> MakeFeedbackAsync(MakeFeedbackRequest request, string workerId);
    //Task DeleteFeedbackAsync(ulong id, string workerId);
}