using FilterService.Models;

namespace FilterService.Service;

public interface IFilterService
{
    Task<ICollection<TypeOfActivity>> GetFiltersAsync(List<int> filterIds);
    Task<int> AddFilterAsync(TypeOfActivity filter);
}