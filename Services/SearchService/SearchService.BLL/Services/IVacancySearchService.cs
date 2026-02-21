using SeachService.DAL.DTO;
using SearchService.Contract;
using SearchService.DAL.Dto;
using SearchService.DAL.Entities;

namespace SearchService.BLL.Services.Interfaces;

public interface IVacancySearchService
{
    Task<SearchResponse<VacancySearchResultDto>> SearchAsync(GetVacanciesRequest request);
}