using Worky.Contracts;
using Worky.DTO;

namespace Worky.Services;

public interface IWorkerService
{
    Task<IEnumerable<VacancyDtos>> FilterVacanciesAsync(GetVacanciesRequest request);
    Task<VacancyDtos> GetVacancyInfoAsync(ulong vacancyId);
    Task<IEnumerable<ResumeDtos>> GetMyResumesAsync(string workerId, ulong? resumeId);
    Task<ulong> CreateResumeAsync(CreateResume resume, string workerId);
    Task UpdateResumeAsync(UpdateResume resume, string workerId);
    Task DeleteResumeAsync(ulong id, string workerId);
    Task<IEnumerable<ulong>> AddResumeFilterAsync(AddFilter filter, string workerId);
    Task DeleteResumeFilterAsync(ulong filterId, string workerId);
    Task<WorkerProfileDto> GetProfileAsync(string workerId);
    Task<IEnumerable<FeedbackDtos>> GetFeedbacksAsync(string workerId, ulong? vacancyId);
    Task<ulong> MakeFeedbackAsync(MakeFeedbackRequest request, string workerId);
    Task DeleteFeedbackAsync(ulong id, string workerId);
}