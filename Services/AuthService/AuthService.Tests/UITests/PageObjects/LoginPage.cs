using Microsoft.Playwright;

namespace AuthService.Tests.UITests.PageObjects;

public class LoginPage
{
    private readonly IPage page;

    public LoginPage(IPage page)
    {
        this.page = page;
    }

    public async Task Open(string baseUrl)
    {
        await page.GotoAsync($"{baseUrl}/login");
    }

    // поле почты
    private ILocator Email =>
        page.Locator("[data-testid='email-input']");

    // поле пароля
    private ILocator Password =>
        page.Locator("[data-testid='password-input']");

    // поле кнопки входа
    private ILocator LoginButton =>
        page.Locator("[data-testid='login-button']");

    // поле ошибки
    private ILocator Error =>
        page.Locator("[data-testid='login-error']");

    public async Task Login(string email, string password)
    {
        await Email.FillAsync(email);
        await Password.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task<string> GetError()
    {
        return await Error.InnerTextAsync();
    }
}