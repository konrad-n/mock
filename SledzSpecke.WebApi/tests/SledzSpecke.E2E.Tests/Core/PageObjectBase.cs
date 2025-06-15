using Microsoft.Playwright;
using Serilog;

namespace SledzSpecke.E2E.Tests.Core;

/// <summary>
/// Base class for all page objects following Single Responsibility and DRY principles
/// </summary>
public abstract class PageObjectBase : IPageObject
{
    protected readonly IPage Page;
    protected readonly ILogger Logger;
    protected readonly string BaseUrl;
    
    protected PageObjectBase(IPage page, string baseUrl, ILogger logger)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
        BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected abstract string PagePath { get; }
    protected abstract string PageIdentifier { get; }

    public virtual async Task NavigateAsync()
    {
        var url = $"{BaseUrl}{PagePath}";
        Logger.Information("Navigating to {Url}", url);
        
        await Page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = 30000
        });
        
        await WaitForLoadAsync();
    }

    public virtual async Task WaitForLoadAsync()
    {
        Logger.Debug("Waiting for page to load: {PageIdentifier}", PageIdentifier);
        
        // Wait for the page identifier element to be visible
        await Page.WaitForSelectorAsync(PageIdentifier, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 10000
        });
        
        // Additional wait for any animations to complete
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public virtual async Task<bool> IsCurrentPageAsync()
    {
        try
        {
            var element = await Page.QuerySelectorAsync(PageIdentifier);
            return element != null;
        }
        catch
        {
            return false;
        }
    }

    public virtual async Task<string> GetTitleAsync()
    {
        return await Page.TitleAsync();
    }

    /// <summary>
    /// Helper method to click an element with retry logic
    /// </summary>
    protected async Task ClickWithRetryAsync(string selector, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await Page.ClickAsync(selector, new PageClickOptions
                {
                    Timeout = 5000
                });
                return;
            }
            catch (TimeoutException) when (i < maxRetries - 1)
            {
                Logger.Warning("Click failed for {Selector}, retrying... (attempt {Attempt}/{MaxRetries})", 
                    selector, i + 1, maxRetries);
                await Task.Delay(1000);
            }
        }
    }

    /// <summary>
    /// Helper method to fill a form field with clearing first
    /// </summary>
    protected async Task FillFieldAsync(string selector, string value)
    {
        await Page.ClickAsync(selector);
        await Page.Keyboard.PressAsync("Control+A");
        await Page.Keyboard.PressAsync("Delete");
        await Page.FillAsync(selector, value);
    }

    /// <summary>
    /// Helper method to select from dropdown
    /// </summary>
    protected async Task SelectOptionAsync(string selector, string value)
    {
        await Page.SelectOptionAsync(selector, value);
    }

    /// <summary>
    /// Helper method to wait for element and get its text
    /// </summary>
    protected async Task<string> GetTextAsync(string selector)
    {
        await Page.WaitForSelectorAsync(selector);
        return await Page.TextContentAsync(selector) ?? string.Empty;
    }

    /// <summary>
    /// Helper method to check if element exists
    /// </summary>
    protected async Task<bool> ElementExistsAsync(string selector)
    {
        var element = await Page.QuerySelectorAsync(selector);
        return element != null;
    }

    /// <summary>
    /// Helper method to take a screenshot for debugging
    /// </summary>
    protected async Task TakeScreenshotAsync(string name)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var path = $"Reports/Screenshots/{timestamp}_{name}.png";
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path });
        Logger.Information("Screenshot saved: {Path}", path);
    }
}