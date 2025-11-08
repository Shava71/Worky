using CompanyService.DAL.Contracts;
using CompanyService.DAL.DTO;


namespace CompanyService.BLL.Services.Interfaces;

public interface ICompnayService
{
    Task<VacancyDtos> GetVacancyInfoAsync(Guid vacancyId);
    Task<IEnumerable<VacancyDtos>> GetMyVacanciesAsync(Guid companyId, Guid? vacancyId);
    Task<Guid> CreateVacancyAsync(CreateVacancy vacancy, string companyId);
    Task UpdateVacancyAsync(UpdateVacancy vacancy, string companyId);
    Task DeleteVacancyAsync(Guid id, string companyId);
    Task<IEnumerable<Guid>> AddVacancyFilterAsync(AddFilter filter, string companyId);
    Task DeleteVacancyFilterAsync(Guid filterId, string companyId);
 
    // Task<object> GetStatisticsJsonAsync(string companyId, int start_year, int start_month, int end_year, int end_month);
    // Task<byte[]> GetStatisticsPdfAsync(string companyId, int start_year, int start_month, int end_year, int end_month);
    Task<byte[]> GetFlyerAsync(Guid vacancyId, string url);
    Task<CompanyProfileDtos> GetProfileAsync(string companyId, string token, CancellationToken cancellationToken = default);
}