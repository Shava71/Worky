using Worky.Contracts;
using Worky.DTO;

namespace Worky.Repositories.Interfaces;

public interface IVacancyRepository
{
    Task<IEnumerable<VacancyDtos>> GetVacanciesAsync(GetVacanciesRequest request);
    Task<VacancyDtos> GetVacancyByIdAsync(ulong id);
    Task<ulong> CreateVacancyAsync(CreateVacancy vacancy, string companyId);
    Task UpdateVacancyAsync(UpdateVacancy vacancy);
    Task DeleteVacancyAsync(ulong id);
    Task<IEnumerable<ulong>> AddVacancyFiltersAsync(AddFilter filter);
    Task DeleteVacancyFilterAsync(ulong filterId);
    Task<IEnumerable<VacancyDtos>> GetMyVacanciesAsync(string companyId, ulong? vacancyId);
}