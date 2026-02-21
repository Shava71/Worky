using SearchService.Contract;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;

namespace SearchService.BLL.Services.Interfaces;

public interface IResumeSearchService
{
    Task<IReadOnlyCollection<(ResumeDocument doc, double score)>> SearchAsync(GetResumesRequest request);
}