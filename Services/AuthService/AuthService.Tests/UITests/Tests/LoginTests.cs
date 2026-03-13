using AuthService.Tests.UITests.PageObjects;

namespace AuthService.Tests.UITests.Tests;

public class LoginTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ShouldRedirect()
    {
        var fixture = new PlaywrightFixture();
        await fixture.InitializeAsync();

        var loginPage = new LoginPage(fixture.Page);

        await loginPage.Open(fixture.BaseUrl);

        await loginPage.Login("test@mail.com", "Password123");

        Assert.Contains("", fixture.Page.Url);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldShowError()
    {
        var fixture = new PlaywrightFixture();
        await fixture.InitializeAsync();

        var loginPage = new LoginPage(fixture.Page);

        await loginPage.Open(fixture.BaseUrl);

        await loginPage.Login("test@mail.com", "wrongpass");

        var error = await loginPage.GetError();

        Assert.Equal("Неверный email или пароль", error);
    }
}