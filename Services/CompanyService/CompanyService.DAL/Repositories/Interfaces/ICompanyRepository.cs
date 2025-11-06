
using CompanyService.DAL.Entities;

namespace CompanyService.DAL.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task CreateCompanyAsync(Company company);
    Task<Company> GetCompanyByIdAsync(string id);
    Task UpdateCompanyAsync(Company company);
   
}