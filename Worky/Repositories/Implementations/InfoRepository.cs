using Microsoft.EntityFrameworkCore;
using Worky.Context;
using Worky.Migrations;
using Worky.Repositories.Interfaces;

namespace Worky.Repositories.Implementations;

public class InfoRepository : IInfoRepository
{
    private readonly WorkyDbContext _dbContext;

    public InfoRepository(WorkyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Education>> GetEducationsAsync()
    {
        return await _dbContext.Educations.ToListAsync();
    }

    public async Task<IEnumerable<TypeOfActivity>> GetFiltersAsync()
    {
        return await _dbContext.typeOfActivities.OrderBy(f => f.type).ToListAsync();
    }
}