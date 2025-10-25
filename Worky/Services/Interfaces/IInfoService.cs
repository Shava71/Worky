using Worky.Migrations;

namespace Worky.Services;

public interface IInfoService
{
    Task<IEnumerable<Education>> GetEducationsAsync();
    Task<IEnumerable<TypeOfActivity>> GetFiltersAsync();
}