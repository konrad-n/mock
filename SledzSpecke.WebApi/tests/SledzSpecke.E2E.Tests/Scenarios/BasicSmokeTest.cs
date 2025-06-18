using Microsoft.Playwright;
using Xunit;
using FluentAssertions;

namespace SledzSpecke.E2E.Tests.Scenarios;

public class BasicSmokeTest : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IPage _page = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        var context = await _browser.NewContextAsync();
        _page = await context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    [Fact]
    public async Task HomePage_ShouldLoad_Successfully()
    {
        // Act
        var response = await _page.GotoAsync("https://sledzspecke.pl");
        
        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(200);
    }
    
    [Fact]
    public async Task ApiHealth_ShouldReturn_OK()
    {
        // Act
        var response = await _page.APIRequest.GetAsync("https://api.sledzspecke.pl/api/health");
        
        // Assert
        response.Ok.Should().BeTrue();
        var json = await response.JsonAsync();
        json.Should().NotBeNull();
    }
}
