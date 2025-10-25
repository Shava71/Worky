using Worky.Contracts;
using Worky.DTO;

namespace Worky.Services;

public interface ICompnayService
{
    Task<IEnumerable<ResumeDtos>> FilterResumesAsync(GetResumesRequest request);
    Task<ResumeDtos> GetResumeInfoAsync(ulong resumeId);
    Task<IEnumerable<VacancyDtos>> GetMyVacanciesAsync(string companyId, ulong? vacancyId);
    Task<ulong> CreateVacancyAsync(CreateVacancy vacancy, string companyId);
    Task UpdateVacancyAsync(UpdateVacancy vacancy, string companyId);
    Task DeleteVacancyAsync(ulong id, string companyId);
    Task<IEnumerable<ulong>> AddVacancyFilterAsync(AddFilter filter, string companyId);
    Task DeleteVacancyFilterAsync(ulong filterId, string companyId);
    Task<IEnumerable<FeedbackDtos>> GetFeedbacksAsync(string companyId, ulong? resumeId);
    Task<ulong> MakeFeedbackAsync(MakeFeedbackRequest request, string companyId);
    Task DeleteFeedbackAsync(ulong id, string companyId);
    Task<object> GetStatisticsJsonAsync(string companyId, int start_year, int start_month, int end_year, int end_month);
    Task<byte[]> GetStatisticsPdfAsync(string companyId, int start_year, int start_month, int end_year, int end_month);
    Task<byte[]> GetFlyerAsync(ulong vacancyId, string url);
    Task<CompanyProfileDtos> GetProfileAsync(string companyId);
}