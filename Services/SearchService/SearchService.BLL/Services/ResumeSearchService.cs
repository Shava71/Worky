using SearchService.BLL.Services.Interfaces;
using SearchService.Contract;
using SearchService.DAL.Dto;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.BLL.Services.Implementations;

public class ResumeSearchService : IResumeSearchService
{
    private readonly IResumeElasticRepository _repository;

    public ResumeSearchService(IResumeElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task<SearchResponse<ResumeSearchResultDto>> SearchAsync(GetResumesRequest request)
    {
        var result = await _repository.SearchAsync(request);
        return result;
    }
}