using FilterService.Models;
using FilterService.Repository;

namespace FilterService.Service;

public class FilterService : IFilterService
{
    private readonly IFilterRepository _repository;

    public FilterService(IFilterRepository repository)
    {
        _repository = repository;
    }

    public async Task<ICollection<TypeOfActivity>> GetFiltersAsync(List<int> filterIds)
    {
        return await _repository.GetFiltersAsync(filterIds);
    }

    public async Task<int> AddFilterAsync(TypeOfActivity filter)
    {
        return await _repository.AddFilterAsync(filter);
    }
}