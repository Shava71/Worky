using CompanyService.DAL.Contracts;
using CompanyService.DAL.DTO;


namespace CompanyService.BLL.Services.Interfaces;

public interface ICompnayService
{
    Task<VacancyDtos> GetVacancyInfoAsync(Guid vacancyId);
    Task<IEnumerable<VacancyDtos>> GetMyVacanciesAsync(Guid companyId, Guid? vacancyId);
    Task<ulong> CreateVacancyAsync(CreateVacancy vacancy, string companyId);
    Task UpdateVacancyAsync(UpdateVacancy vacancy, string companyId);
    Task DeleteVacancyAsync(ulong id, string companyId);
    Task<IEnumerable<ulong>> AddVacancyFilterAsync(AddFilter filter, string companyId);
    Task DeleteVacancyFilterAsync(ulong filterId, string companyId);
 
    Task<object> GetStatisticsJsonAsync(string companyId, int start_year, int start_month, int end_year, int end_month);
    Task<byte[]> GetStatisticsPdfAsync(string companyId, int start_year, int start_month, int end_year, int end_month);
    Task<byte[]> GetFlyerAsync(ulong vacancyId, string url);
    Task<CompanyProfileDtos> GetProfileAsync(string companyId);
}