using Microsoft.Playwright;

namespace AuthService.Tests.UITests;

public class PlaywrightFixture
{
    public IPage Page { get; private set; }
    public string BaseUrl = "http://localhost:5173";

    public async Task InitializeAsync()
    {
        var playwright = await Playwright.CreateAsync();

        var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false
            });

        var context = await browser.NewContextAsync();

        Page = await context.NewPageAsync();
    }
}