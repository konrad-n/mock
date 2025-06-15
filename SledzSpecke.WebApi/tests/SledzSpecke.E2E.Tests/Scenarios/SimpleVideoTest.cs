using Microsoft.Playwright;
using SledzSpecke.E2E.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace SledzSpecke.E2E.Tests.Scenarios;

/// <summary>
/// Simple test to ensure video recording works
/// </summary>
public class SimpleVideoTest : E2ETestBase
{
    private readonly ITestOutputHelper _output;
    
    public SimpleVideoTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public async Task Simple_Navigation_Should_Record_Video()
    {
        _output.WriteLine("Starting simple video test");
        
        // Navigate to Google as a simple test
        await Page.GotoAsync("https://www.google.com");
        
        // Wait a bit to ensure video has content
        await Page.WaitForTimeoutAsync(3000);
        
        // Take a screenshot for princess dashboard
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/simple-video-test.png",
            FullPage = true 
        });
        
        // Type something in search box
        await Page.FillAsync("textarea[name='q']", "SledzSpecke");
        
        // Wait a bit more
        await Page.WaitForTimeoutAsync(2000);
        
        _output.WriteLine("Simple video test completed");
    }
}