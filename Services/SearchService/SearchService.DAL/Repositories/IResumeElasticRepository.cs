using SearchService.Contract;
using SearchService.DAL.Dto;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;

namespace SearchService.DAL.Repositories.Interfaces;

public interface IResumeElasticRepository : IElasticRepository<ResumeDocument>
{
    Task<SearchResponse<ResumeSearchResultDto>> SearchAsync(GetResumesRequest request);
}