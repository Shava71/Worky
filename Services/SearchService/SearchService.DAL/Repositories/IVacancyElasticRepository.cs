using SeachService.DAL.DTO;
using SearchService.Contract;
using SearchService.DAL.Dto;
using SearchService.DAL.Entities;

namespace SearchService.DAL.Repositories.Interfaces;

public interface IVacancyElasticRepository : IElasticRepository<VacancyDocument>
{
    Task<Contract.SearchResponse<VacancySearchResultDto>> SearchAsync(GetVacanciesRequest request);

    /*Task<IReadOnlyCollection<VacancySearchResultDto>> SearchLexicalAsync(string query);
    Task<IReadOnlyCollection<VacancySearchResultDto>> SearchSemanticAsync(string query);

    Task<IReadOnlyCollection<VacancySearchResultDto>> SearchHybridAsync(string query);
    Task<IReadOnlyCollection<VacancySearchResultDto>> SearchTwoStageAsync(string query);*/
}