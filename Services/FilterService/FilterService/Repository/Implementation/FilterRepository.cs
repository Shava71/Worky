using FilterService.Data;
using FilterService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FilterService.Repository;

public class FilterRepository : IFilterRepository
{
    private readonly ApplicationDbContext _context;


    public FilterRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ICollection<TypeOfActivity>> GetFiltersAsync(List<int> filterIds)
    {
        IQueryable<TypeOfActivity> filters = _context.TypeOfActivities.AsQueryable().AsNoTracking();

        if (filterIds != null && filterIds.Any())
        {
            filters = filters.Where(a => filterIds.Contains(a.id));
        }
        return await filters.ToListAsync();
    }

    public async Task<int> AddFilterAsync(TypeOfActivity filter)
    {
        EntityEntry<TypeOfActivity> entry = await _context.TypeOfActivities.AddAsync(filter);
        await _context.SaveChangesAsync();
        return entry.Entity.id;
    }

    
}