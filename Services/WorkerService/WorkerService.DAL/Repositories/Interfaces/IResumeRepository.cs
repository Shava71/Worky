

using WorkerService.DAL.Contracts;
using WorkerService.DAL.DTO;
using WorkerService.DAL.Entities;

namespace WorkerService.DAL.Repositories.Interfaces;

public interface IResumeRepository
{
    Task<IEnumerable<ResumeDtos>> GetResumesAsync(GetResumesRequest request);
    Task<ResumeDtos> GetResumeByIdAsync(Guid id);
    Task<Guid> CreateResumeAsync(CreateResume resume, string workerId);
    Task UpdateResumeAsync(UpdateResume resume);
    Task DeleteResumeAsync(Guid id);
    Task<IEnumerable<Guid>> AddResumeFiltersAsync(AddFilter filter);
    Task DeleteResumeFilterAsync(Guid filterId);
    Task<Resume_filter?> GetResumeFilterByIdAsync(Guid id);

    Task<IEnumerable<ResumeDtos>> GetMyResumesAsync(string workerId, Guid? resumeId);
}