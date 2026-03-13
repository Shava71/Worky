using AuthService.Tests.UITests.PageObjects;

namespace AuthService.Tests.UITests.Tests;

public class RegisterTests
{
    [Fact]
    public async Task Register_NewUser_ShouldRedirectToLogin()
    {
        var fixture = new PlaywrightFixture();
        await fixture.InitializeAsync();

        var registerPage = new WorkerRegisterPage(fixture.Page);

        await registerPage.Open(fixture.BaseUrl);

        string email = $"test{Guid.NewGuid()}@mail.com";

        await registerPage.Register(
            "testuser",
            email,
            "89582753821",
            "Password123",
            "Ivan",
                    "Ivanovich",
            "Ivanov",
            "10.10.2000"
        );

        Assert.Contains("", fixture.Page.Url);
    }
}