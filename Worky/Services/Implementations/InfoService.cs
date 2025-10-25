using Worky.Migrations;
using Worky.Repositories.Interfaces;

namespace Worky.Services;

public class InfoService : IInfoService
{
    private readonly IInfoRepository _infoRepository;

    public InfoService(IInfoRepository infoRepository)
    {
        _infoRepository = infoRepository;
    }

    public async Task<IEnumerable<Education>> GetEducationsAsync()
    {
        return await _infoRepository.GetEducationsAsync();
    }

    public async Task<IEnumerable<TypeOfActivity>> GetFiltersAsync()
    {
        return await _infoRepository.GetFiltersAsync();
    }
}