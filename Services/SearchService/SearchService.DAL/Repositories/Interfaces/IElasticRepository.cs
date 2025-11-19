namespace SearchService.DAL.Repositories.Interfaces;

public interface IElasticRepository<T> where T : class
{
    Task IndexAsync(string id, T document);
    Task UpdateAsync(string id, T document);
    Task DeleteAsync(string id);
    Task<T?> GetByIdAsync(string id);
}