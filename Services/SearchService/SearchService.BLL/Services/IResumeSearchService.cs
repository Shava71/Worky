using SearchService.Contract;
using SearchService.DAL.Dto;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;

namespace SearchService.BLL.Services.Interfaces;

public interface IResumeSearchService
{
    Task<SearchResponse<ResumeSearchResultDto>> SearchAsync(GetResumesRequest request);
}