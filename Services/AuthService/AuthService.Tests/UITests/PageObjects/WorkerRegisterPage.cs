using Microsoft.Playwright;

namespace AuthService.Tests.UITests.PageObjects;

public class WorkerRegisterPage
{
    private readonly IPage page;

    public WorkerRegisterPage(IPage page)
    {
        this.page = page;
    }

    public async Task Open(string baseUrl)
    {
        await page.GotoAsync($"{baseUrl}/WorkerRegister");
    }

    // поле имени пользователя
    private ILocator UserName =>
        page.Locator("[data-testid='username-input']");

    // поле почты
    private ILocator Email =>
        page.Locator("[data-testid='email-input']");

    // поле телефона
    private ILocator Phone =>
        page.Locator("[data-testid='phone-input']");

    // поле пароля
    private ILocator Password =>
        page.Locator("[data-testid='password-input']");

    // поле имени
    private ILocator FirstName =>
        page.Locator("[data-testid='firstname-input']");

    // поле фамилии
    private ILocator LastName =>
        page.Locator("[data-testid='lastname-input']");

    // поле отчества
    private ILocator Surname =>
        page.Locator("[data-testid='surname-input']");

    // поле кнопки регистрации
    private ILocator RegisterButton =>
        page.Locator("[data-testid='register-button']");
    
    // поле даты рождения
    private ILocator Birthday =>
        page.Locator("[data-testid='birthday-input']");

    public async Task Register(
        string username,
        string email,
        string phone,
        string password,
        string firstName,
        string surname,
        string lastName,
        string date)
    {
        await UserName.FillAsync(username);
        await Email.FillAsync(email);
        await Phone.FillAsync(phone);
        await Password.FillAsync(password);
        await FirstName.FillAsync(firstName);
        await Surname.FillAsync(surname);
        await LastName.FillAsync(lastName);
        await Birthday.FillAsync(date);

        await RegisterButton.ClickAsync();
    }
}