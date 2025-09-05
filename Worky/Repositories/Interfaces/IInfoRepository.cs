using Worky.Migrations;

namespace Worky.Repositories.Interfaces;

public interface IInfoRepository
{
    Task<IEnumerable<Education>> GetEducationsAsync();
    Task<IEnumerable<TypeOfActivity>> GetFiltersAsync();
}