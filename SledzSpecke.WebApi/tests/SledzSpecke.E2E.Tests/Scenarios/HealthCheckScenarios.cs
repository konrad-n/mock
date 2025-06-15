using System.Net;
using FluentAssertions;
using Microsoft.Playwright;
using SledzSpecke.E2E.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace SledzSpecke.E2E.Tests.Scenarios;

/// <summary>
/// Simple health check scenarios to verify E2E setup is working
/// </summary>
public class HealthCheckScenarios : E2ETestBase, IClassFixture<E2ETestFixture>
{
    private readonly ITestOutputHelper _output;
    
    public HealthCheckScenarios(E2ETestFixture fixture, ITestOutputHelper output) : base(fixture)
    {
        _output = output;
    }
    
    [Fact]
    public async Task Frontend_HomePage_ShouldLoad()
    {
        // Navigate to home page
        await Page.GotoAsync(Configuration["E2ETests:BaseUrl"]);
        
        // Wait for page to load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Take screenshot for princess dashboard
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/frontend-homepage.png",
            FullPage = true 
        });
        
        // Check that page loaded
        var title = await Page.TitleAsync();
        title.Should().NotBeNullOrEmpty();
        
        _output.WriteLine($"Frontend loaded successfully with title: {title}");
    }
    
    [Fact]
    public async Task API_HealthEndpoint_ShouldReturnOK()
    {
        var apiUrl = Configuration["E2ETests:ApiUrl"];
        var response = await Page.APIRequest.GetAsync($"{apiUrl}/health");
        
        response.Status.Should().Be((int)HttpStatusCode.OK);
        
        var json = await response.JsonAsync();
        json?.Value.GetProperty("status").GetString().Should().Be("Healthy");
        
        _output.WriteLine("API health check passed");
    }
    
    [Fact] 
    public async Task Frontend_LoginPage_ShouldBeAccessible()
    {
        // Navigate to login page
        await Page.GotoAsync($"{Configuration["E2ETests:BaseUrl"]}/login");
        
        // Wait for page to load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Take screenshot for princess dashboard
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/login-page.png",
            FullPage = true 
        });
        
        // Check for login form elements
        var hasForm = await Page.Locator("form").CountAsync() > 0;
        hasForm.Should().BeTrue("Login page should have a form");
        
        _output.WriteLine("Login page is accessible");
    }
}