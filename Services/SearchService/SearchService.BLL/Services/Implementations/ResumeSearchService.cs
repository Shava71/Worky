using SearchService.BLL.Services.Interfaces;
using SearchService.Contract;
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

    public async Task<IReadOnlyCollection<(ResumeDocument doc, double score)>> SearchAsync(GetResumesRequest request)
    {
        IReadOnlyCollection<(ResumeDocument doc, double score)> result = await _repository.SearchAsync(request);
        return result;
    }
}