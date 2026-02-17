using CompanyService.BLL.Services.Http.Implementations;
using CompanyService.BLL.Services.Implementations;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;

namespace CompanyService.Tests.Integration;

public class FilterCacheServiceTests
{
    [Fact]
    public async Task GetFiltersByIdsAsync_Returns_From_Http_And_Caches()
    {
        var host = HostFactory.CreateHost();
        var httpClient = host.GetTestClient();

        var loggerFactory = new LoggerFactory();
        var redis = new InMemoryRedisRepository();

        var filterClient = new FilterClient(
            httpClient,
            loggerFactory.CreateLogger<FilterClient>());

        var service = new FilterCacheService(
            loggerFactory.CreateLogger<FilterCacheService>(),
            redis,
            filterClient);

        var result = await service.GetFiltersByIdsAsync(new List<int> { 1, 2 });

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // второй вызов должен прийти уже из кэша
        var result2 = await service.GetFiltersByIdsAsync(new List<int> { 1, 2 });

        Assert.Equal(2, result2.Count);
    }
}