

using WorkerService.DAL.Contracts;
using WorkerService.DAL.DTO;

namespace WorkerService.DAL.Repositories.Interfaces;

public interface IResumeRepository
{
    Task<IEnumerable<ResumeDtos>> GetResumesAsync(GetResumesRequest request);
    Task<ResumeDtos> GetResumeByIdAsync(ulong id);
    Task<ulong> CreateResumeAsync(CreateResume resume, string workerId);
    Task UpdateResumeAsync(UpdateResume resume);
    Task DeleteResumeAsync(ulong id);
    Task<IEnumerable<ulong>> AddResumeFiltersAsync(AddFilter filter);
    Task DeleteResumeFilterAsync(ulong filterId);
    Task<IEnumerable<ResumeDtos>> GetMyResumesAsync(string workerId, Guid? resumeId);
}