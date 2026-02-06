using CompanyService.DAL.Contracts;
using CompanyService.DAL.DTO;
using CompanyService.DAL.Entities;

namespace CompanyService.DAL.Repositories.Interfaces;

public interface IVacancyRepository
{
    // Task<IEnumerable<VacancyDtos>> GetVacanciesAsync(GetVacanciesRequest request);
    Task<VacancyDtos> GetVacancyByIdAsync(Guid id);
    Task<Guid> CreateVacancyAsync(CreateVacancy vacancy, string companyId);
    Task UpdateVacancyAsync(UpdateVacancy vacancy, Guid companyId);
    Task DeleteVacancyAsync(Guid id, Guid companyId);
    Task<IEnumerable<Guid>> AddVacancyFiltersAsync(AddFilter filter);
    Task DeleteVacancyFilterAsync(Guid filterId);
    Task<Vacancy_filter?> GetVacancyFilterByIdAsync(Guid id);
    Task<IEnumerable<VacancyDtos>> GetMyVacanciesAsync(string companyId, Guid? vacancyId);
    Task<int> GetMyVacanciesCountAsync(Guid companyId);
}