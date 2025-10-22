using FilterService.Models;

namespace FilterService.Repository;

public interface IFilterRepository
{
    Task<ICollection<TypeOfActivity>> GetFiltersAsync(List<int> filterIds);
    Task<int> AddFilterAsync(TypeOfActivity filter);
}