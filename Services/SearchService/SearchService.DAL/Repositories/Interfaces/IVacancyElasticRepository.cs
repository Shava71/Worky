using SeachService.DAL.DTO;
using SearchService.Contract;
using SearchService.DAL.Dto;
using SearchService.DAL.Entities;

namespace SearchService.DAL.Repositories.Interfaces;

public interface IVacancyElasticRepository : IElasticRepository<VacancyDocument>
{
    Task<IReadOnlyCollection<VacancySearchResultDto>> SearchAsync(GetVacanciesRequest request);

}