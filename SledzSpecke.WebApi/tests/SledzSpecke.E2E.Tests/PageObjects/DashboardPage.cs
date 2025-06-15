using Microsoft.Playwright;
using Serilog;
using SledzSpecke.E2E.Tests.Core;

namespace SledzSpecke.E2E.Tests.PageObjects;

/// <summary>
/// Page Object for Dashboard - the main hub after login
/// </summary>
public interface IDashboardPage : IPageObject
{
    Task<string> GetWelcomeMessageAsync();
    Task<bool> IsUserMenuVisibleAsync();
    Task OpenUserMenuAsync();
    Task LogoutAsync();
    Task NavigateToMedicalShiftsAsync();
    Task NavigateToProceduresAsync();
    Task NavigateToInternshipsAsync();
    Task NavigateToCoursesAsync();
    Task<DashboardStatistics> GetStatisticsAsync();
}

public class DashboardPage : PageObjectBase, IDashboardPage
{
    // Selectors
    private const string WelcomeMessage = "h1, h2:has-text('Witaj')";
    private const string UserMenuButton = "button[aria-label*='user'], [data-testid='user-menu']";
    private const string UserMenu = "[role='menu']";
    private const string LogoutMenuItem = "[role='menuitem']:has-text('Wyloguj')";
    
    // Navigation items
    private const string MedicalShiftsLink = "a[href*='medical-shifts'], nav :has-text('Dyżury')";
    private const string ProceduresLink = "a[href*='procedures'], nav :has-text('Procedury')";
    private const string InternshipsLink = "a[href*='internships'], nav :has-text('Staże')";
    private const string CoursesLink = "a[href*='courses'], nav :has-text('Kursy')";
    
    // Statistics selectors
    private const string StatsContainer = "[data-testid='statistics'], .statistics-container";
    private const string StatShifts = "[data-testid='stat-shifts'], .stat-shifts";
    private const string StatProcedures = "[data-testid='stat-procedures'], .stat-procedures";
    private const string StatCourses = "[data-testid='stat-courses'], .stat-courses";
    
    protected override string PagePath => "/dashboard";
    protected override string PageIdentifier => WelcomeMessage;

    public DashboardPage(IPage page, string baseUrl, ILogger logger) 
        : base(page, baseUrl, logger)
    {
    }

    public async Task<string> GetWelcomeMessageAsync()
    {
        return await GetTextAsync(WelcomeMessage);
    }

    public async Task<bool> IsUserMenuVisibleAsync()
    {
        return await ElementExistsAsync(UserMenuButton);
    }

    public async Task OpenUserMenuAsync()
    {
        Logger.Information("Opening user menu");
        await ClickWithRetryAsync(UserMenuButton);
        await Page.WaitForSelectorAsync(UserMenu, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 3000
        });
    }

    public async Task LogoutAsync()
    {
        Logger.Information("Logging out from dashboard");
        await OpenUserMenuAsync();
        await ClickWithRetryAsync(LogoutMenuItem);
        
        // Wait for redirect to login page
        await Page.WaitForURLAsync(url => url.Contains("/login"), new PageWaitForURLOptions
        {
            Timeout = 5000
        });
    }

    public async Task NavigateToMedicalShiftsAsync()
    {
        Logger.Information("Navigating to Medical Shifts");
        await ClickWithRetryAsync(MedicalShiftsLink);
        await Page.WaitForURLAsync(url => url.Contains("medical-shifts"));
    }

    public async Task NavigateToProceduresAsync()
    {
        Logger.Information("Navigating to Procedures");
        await ClickWithRetryAsync(ProceduresLink);
        await Page.WaitForURLAsync(url => url.Contains("procedures"));
    }

    public async Task NavigateToInternshipsAsync()
    {
        Logger.Information("Navigating to Internships");
        await ClickWithRetryAsync(InternshipsLink);
        await Page.WaitForURLAsync(url => url.Contains("internships"));
    }

    public async Task NavigateToCoursesAsync()
    {
        Logger.Information("Navigating to Courses");
        await ClickWithRetryAsync(CoursesLink);
        await Page.WaitForURLAsync(url => url.Contains("courses"));
    }

    public async Task<DashboardStatistics> GetStatisticsAsync()
    {
        Logger.Information("Getting dashboard statistics");
        
        var stats = new DashboardStatistics();
        
        try
        {
            await Page.WaitForSelectorAsync(StatsContainer, new PageWaitForSelectorOptions
            {
                Timeout = 5000
            });
            
            // Extract statistics values
            var shiftsText = await GetTextAsync(StatShifts);
            var proceduresText = await GetTextAsync(StatProcedures);
            var coursesText = await GetTextAsync(StatCourses);
            
            // Parse numbers from text
            stats.TotalShifts = ExtractNumber(shiftsText);
            stats.TotalProcedures = ExtractNumber(proceduresText);
            stats.TotalCourses = ExtractNumber(coursesText);
            
            Logger.Information("Dashboard statistics retrieved: {@Statistics}", stats);
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Failed to retrieve dashboard statistics");
        }
        
        return stats;
    }

    private int ExtractNumber(string text)
    {
        var numbers = System.Text.RegularExpressions.Regex.Matches(text, @"\d+");
        if (numbers.Count > 0 && int.TryParse(numbers[0].Value, out var result))
        {
            return result;
        }
        return 0;
    }
}

public class DashboardStatistics
{
    public int TotalShifts { get; set; }
    public int TotalProcedures { get; set; }
    public int TotalCourses { get; set; }
    public int TotalInternships { get; set; }
}