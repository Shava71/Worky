using Allure.Xunit.Attributes;
using CompanyService.BLL.Services.Http.Implementations;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;

namespace CompanyService.Tests.Integration;

public class FilterClientTests
{
    [Trait("Category", "Integration")]
    [Fact]
    [AllureSuite("CompanyService")] //allurt
    [AllureSubSuite("CreateVacancy")]
    public async Task GetFiltersByIdAsync_Returns_Data_From_TestServer()
    {
        var host = HostFactory.CreateHost();
        var client = host.GetTestClient();

        var logger = new LoggerFactory()
            .CreateLogger<FilterClient>();

        var filterClient = new FilterClient(client, logger);

        var result = await filterClient.GetFiltersByIdAsync(new List<int> { 1, 2 });

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r!.direction == "IT");
    }
}