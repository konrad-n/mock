using Microsoft.Playwright;
using Serilog;
using SledzSpecke.E2E.Tests.Core;

namespace SledzSpecke.E2E.Tests.PageObjects;

/// <summary>
/// Page Object for Login page following Single Responsibility Principle
/// </summary>
public interface ILoginPage : IPageObject
{
    Task LoginAsync(string username, string password);
    Task<bool> IsLoginErrorDisplayedAsync();
    Task<string> GetErrorMessageAsync();
    Task ClickRegisterLinkAsync();
}

public class LoginPage : PageObjectBase, ILoginPage
{
    // Selectors - centralized for maintainability
    private const string UsernameInput = "input[name='username']";
    private const string PasswordInput = "input[name='password']";
    private const string LoginButton = "button[type='submit']";
    private const string ErrorMessage = "[role='alert']";
    private const string RegisterLink = "a[href*='register']";
    
    protected override string PagePath => "/login";
    protected override string PageIdentifier => "form"; // Login form

    public LoginPage(IPage page, string baseUrl, ILogger logger) 
        : base(page, baseUrl, logger)
    {
    }

    public async Task LoginAsync(string username, string password)
    {
        Logger.Information("Attempting login with username: {Username}", username);
        
        // Fill username
        await FillFieldAsync(UsernameInput, username);
        await Page.WaitForTimeoutAsync(200); // Small delay for realistic interaction
        
        // Fill password
        await FillFieldAsync(PasswordInput, password);
        await Page.WaitForTimeoutAsync(200);
        
        // Take screenshot before login if configured
        if (Logger.IsEnabled(Serilog.Events.LogEventLevel.Debug))
        {
            await TakeScreenshotAsync("before_login");
        }
        
        // Click login button
        await ClickWithRetryAsync(LoginButton);
        
        // Wait for navigation or error
        try
        {
            await Page.WaitForURLAsync(url => !url.Contains("/login"), new PageWaitForURLOptions
            {
                Timeout = 5000
            });
            Logger.Information("Login successful, navigated away from login page");
        }
        catch (TimeoutException)
        {
            // Check if error message appeared instead
            if (await IsLoginErrorDisplayedAsync())
            {
                var error = await GetErrorMessageAsync();
                Logger.Warning("Login failed with error: {Error}", error);
            }
            else
            {
                Logger.Warning("Login timeout - neither navigation nor error occurred");
            }
        }
    }

    public async Task<bool> IsLoginErrorDisplayedAsync()
    {
        return await ElementExistsAsync(ErrorMessage);
    }

    public async Task<string> GetErrorMessageAsync()
    {
        if (await IsLoginErrorDisplayedAsync())
        {
            return await GetTextAsync(ErrorMessage);
        }
        return string.Empty;
    }

    public async Task ClickRegisterLinkAsync()
    {
        Logger.Information("Clicking register link");
        await ClickWithRetryAsync(RegisterLink);
    }
}