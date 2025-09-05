using Worky.Migrations;

namespace Worky.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task CreateCompanyAsync(Company company);
    Task<Company> GetCompanyByIdAsync(string id);
    Task UpdateCompanyAsync(Company company);
    // Statistics methods if needed
    Task<IEnumerable<ulong>> GetVacanciesByCompanyAsync(string companyId);
    Task<IEnumerable<Feedback>> GetFeedbacksByVacanciesAsync(IEnumerable<ulong> vacancyIds, DateOnly start, DateOnly end);
}