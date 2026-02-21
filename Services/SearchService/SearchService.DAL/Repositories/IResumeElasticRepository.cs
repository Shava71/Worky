using SearchService.Contract;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;

namespace SearchService.DAL.Repositories.Interfaces;

public interface IResumeElasticRepository : IElasticRepository<ResumeDocument>
{
    Task<IReadOnlyCollection<(ResumeDocument doc, double score)>> SearchAsync(GetResumesRequest request);
}