using SeachService.DAL.DTO;
using SearchService.Contract;
using SearchService.DAL.Entities;

namespace SearchService.BLL.Services.Interfaces;

public interface IVacancySearchService
{
    Task<IReadOnlyCollection<(VacancyDocument doc, double score)>> SearchAsync(GetVacanciesRequest request);
}