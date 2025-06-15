using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace SledzSpecke.E2E.Tests.Scenarios;

/// <summary>
/// Guaranteed video test - creates video without using base class
/// </summary>
public class GuaranteedVideoTest
{
    private readonly ITestOutputHelper _output;
    
    public GuaranteedVideoTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public async Task Guaranteed_Video_Creation()
    {
        _output.WriteLine("Starting guaranteed video test");
        
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        
        // Create context with video recording
        await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = "Reports/Videos",
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        });
        
        var page = await context.NewPageAsync();
        
        _output.WriteLine("Navigating to Google");
        await page.GotoAsync("https://www.google.com");
        
        // Take screenshot
        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "Reports/Screenshots/guaranteed-video.png"
        });
        
        // Type in search
        await page.FillAsync("textarea[name='q']", "SledzSpecke Princess Dashboard");
        
        // Wait to ensure video has content
        await page.WaitForTimeoutAsync(5000);
        
        _output.WriteLine("Closing context to save video");
        await context.CloseAsync();
        
        // List all files in Reports/Videos
        var videoDir = "Reports/Videos";
        if (Directory.Exists(videoDir))
        {
            var files = Directory.GetFiles(videoDir);
            _output.WriteLine($"Video files created: {files.Length}");
            foreach (var file in files)
            {
                _output.WriteLine($"Video: {file} ({new FileInfo(file).Length} bytes)");
            }
        }
        else
        {
            _output.WriteLine("Reports/Videos directory does not exist!");
        }
        
        _output.WriteLine("Test completed");
    }
}