
using CompanyService.DAL.Entities;

namespace CompanyService.DAL.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task CreateCompanyAsync(Company company);
    Task<Company> GetCompanyByIdAsync(Guid id);
    Task UpdateCompanyAsync(Company company);
   
}