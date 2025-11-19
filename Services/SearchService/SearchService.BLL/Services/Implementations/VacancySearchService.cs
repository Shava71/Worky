using Elastic.Clients.Elasticsearch.Ingest;
using SeachService.DAL.DTO;
using SearchService.BLL.Services.Interfaces;
using SearchService.Contract;
using SearchService.DAL.Entities;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.BLL.Services.Implementations;

public class VacancySearchService : IVacancySearchService
{
    private readonly IVacancyElasticRepository _repository;

    public VacancySearchService(IVacancyElasticRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<(VacancyDocument doc, double score)>> SearchAsync(GetVacanciesRequest request)
    {
        IReadOnlyCollection<(VacancyDocument doc, double score)> result = await _repository.SearchAsync(request);
        return result;
    }
}