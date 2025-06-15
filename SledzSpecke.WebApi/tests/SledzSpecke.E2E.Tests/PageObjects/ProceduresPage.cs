using Microsoft.Playwright;
using Serilog;
using SledzSpecke.E2E.Tests.Core;

namespace SledzSpecke.E2E.Tests.PageObjects;

/// <summary>
/// Page object for Procedures page following Page Object Model pattern
/// </summary>
public interface IProceduresPage : IPageObject
{
    Task<bool> IsProcedureFormVisibleAsync();
    Task FillProcedureFormAsync(ProcedureData data);
    Task SubmitProcedureFormAsync();
    Task<bool> IsProcedureSavedSuccessfullyAsync();
    Task<int> GetProcedureCountAsync();
    Task<List<ProcedureListItem>> GetProcedureListAsync();
    Task ClickAddProcedureButtonAsync();
    Task FilterByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task FilterByCategoryAsync(string category);
    Task ExportProceduresAsync(string format);
    Task<ProcedureStatistics> GetStatisticsAsync();
}

public class ProceduresPage : PageObjectBase, IProceduresPage
{
    public ProceduresPage(IPage page, string baseUrl, ILogger logger) 
        : base(page, baseUrl, logger)
    {
    }

    protected override string PagePath => "/procedures";
    protected override string PageIdentifier => "[data-testid='procedures-page']";

    // Selectors
    private const string AddProcedureButton = "button:has-text('Dodaj procedurę')";
    private const string ProcedureForm = "[data-testid='procedure-form']";
    private const string ProcedureList = "[data-testid='procedures-list']";
    private const string ProcedureItem = "[data-testid='procedure-item']";
    private const string EmptyState = "[data-testid='empty-procedures']";
    private const string SuccessMessage = "[data-testid='success-message']";
    
    // Form fields
    private const string DateInput = "input[name='date']";
    private const string NameInput = "input[name='name']";
    private const string CategorySelect = "select[name='category']";
    private const string IcdCodeInput = "input[name='icdCode']";
    private const string DescriptionTextarea = "textarea[name='description']";
    private const string SupervisedCheckbox = "input[name='supervised']";
    private const string PatientAgeInput = "input[name='patientAge']";
    private const string SubmitButton = "button[type='submit']";
    
    // Filter selectors
    private const string DateRangeFilter = "[data-testid='date-range-filter']";
    private const string CategoryFilter = "[data-testid='category-filter']";
    private const string ExportButton = "[data-testid='export-button']";
    
    // Statistics selectors
    private const string TotalProceduresCount = "[data-testid='total-procedures']";
    private const string SupervisedCount = "[data-testid='supervised-count']";
    private const string UnsupervisedCount = "[data-testid='unsupervised-count']";

    public async Task<bool> IsProcedureFormVisibleAsync()
    {
        try
        {
            await Page.WaitForSelectorAsync(ProcedureForm, new() { Timeout = 5000 });
            return await Page.IsVisibleAsync(ProcedureForm);
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public async Task FillProcedureFormAsync(ProcedureData data)
    {
        Logger.Information("Filling procedure form with data: {@Data}", data);
        
        // Date
        await Page.FillAsync(DateInput, data.Date.ToString("yyyy-MM-dd"));
        
        // Procedure name
        await Page.FillAsync(NameInput, data.Name);
        
        // Category
        if (!string.IsNullOrEmpty(data.Category))
        {
            await Page.SelectOptionAsync(CategorySelect, data.Category);
        }
        
        // ICD Code
        if (!string.IsNullOrEmpty(data.IcdCode))
        {
            await Page.FillAsync(IcdCodeInput, data.IcdCode);
        }
        
        // Description
        if (!string.IsNullOrEmpty(data.Description))
        {
            await Page.FillAsync(DescriptionTextarea, data.Description);
        }
        
        // Supervised checkbox
        if (data.Supervised)
        {
            var isChecked = await Page.IsCheckedAsync(SupervisedCheckbox);
            if (!isChecked)
            {
                await Page.CheckAsync(SupervisedCheckbox);
            }
        }
        else
        {
            var isChecked = await Page.IsCheckedAsync(SupervisedCheckbox);
            if (isChecked)
            {
                await Page.UncheckAsync(SupervisedCheckbox);
            }
        }
        
        // Patient age
        if (data.PatientAge.HasValue)
        {
            await Page.FillAsync(PatientAgeInput, data.PatientAge.Value.ToString());
        }
        
        Logger.Information("Procedure form filled successfully");
    }

    public async Task SubmitProcedureFormAsync()
    {
        Logger.Information("Submitting procedure form");
        
        await Page.ClickAsync(SubmitButton);
        
        // Wait for either success message or error
        await Page.WaitForSelectorAsync($"{SuccessMessage}, [data-testid='error-message']", new() { Timeout = 10000 });
    }

    public async Task<bool> IsProcedureSavedSuccessfullyAsync()
    {
        try
        {
            await Page.WaitForSelectorAsync(SuccessMessage, new() { Timeout = 5000 });
            var isVisible = await Page.IsVisibleAsync(SuccessMessage);
            
            if (isVisible)
            {
                var message = await Page.TextContentAsync(SuccessMessage);
                Logger.Information("Success message displayed: {Message}", message);
            }
            
            return isVisible;
        }
        catch (TimeoutException)
        {
            Logger.Warning("Success message not found within timeout");
            return false;
        }
    }

    public async Task<int> GetProcedureCountAsync()
    {
        try
        {
            // Check if empty state is visible
            if (await Page.IsVisibleAsync(EmptyState))
            {
                return 0;
            }
            
            // Count procedure items
            var items = await Page.QuerySelectorAllAsync(ProcedureItem);
            return items.Count;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error getting procedure count");
            return 0;
        }
    }

    public async Task<List<ProcedureListItem>> GetProcedureListAsync()
    {
        var procedures = new List<ProcedureListItem>();
        
        try
        {
            var items = await Page.QuerySelectorAllAsync(ProcedureItem);
            
            foreach (var item in items)
            {
                var procedure = new ProcedureListItem
                {
                    Name = await item.GetAttributeAsync("data-name") ?? "",
                    Category = await item.GetAttributeAsync("data-category") ?? "",
                    Date = DateTime.Parse(await item.GetAttributeAsync("data-date") ?? DateTime.Today.ToString()),
                    Supervised = bool.Parse(await item.GetAttributeAsync("data-supervised") ?? "false")
                };
                
                procedures.Add(procedure);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error getting procedure list");
        }
        
        return procedures;
    }

    public async Task ClickAddProcedureButtonAsync()
    {
        Logger.Information("Clicking add procedure button");
        
        await Page.ClickAsync(AddProcedureButton);
        await Page.WaitForSelectorAsync(ProcedureForm);
    }

    public async Task FilterByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        Logger.Information("Filtering procedures by date range: {Start} to {End}", startDate, endDate);
        
        // Open date range filter
        await Page.ClickAsync(DateRangeFilter);
        
        // Fill start date
        await Page.FillAsync("input[name='startDate']", startDate.ToString("yyyy-MM-dd"));
        
        // Fill end date
        await Page.FillAsync("input[name='endDate']", endDate.ToString("yyyy-MM-dd"));
        
        // Apply filter
        await Page.ClickAsync("button:has-text('Zastosuj')");
        
        // Wait for list to update
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task FilterByCategoryAsync(string category)
    {
        Logger.Information("Filtering procedures by category: {Category}", category);
        
        await Page.SelectOptionAsync(CategoryFilter, category);
        
        // Wait for list to update
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ExportProceduresAsync(string format)
    {
        Logger.Information("Exporting procedures as {Format}", format);
        
        // Click export button
        await Page.ClickAsync(ExportButton);
        
        // Select format
        await Page.ClickAsync($"button:has-text('{format}')");
        
        // Wait for download
        var download = await Page.RunAndWaitForDownloadAsync(async () =>
        {
            await Page.ClickAsync("button:has-text('Eksportuj')");
        });
        
        // Save the download
        var fileName = $"procedures_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";
        await download.SaveAsAsync($"Reports/Downloads/{fileName}");
        
        Logger.Information("Procedures exported to {FileName}", fileName);
    }

    public async Task<ProcedureStatistics> GetStatisticsAsync()
    {
        var stats = new ProcedureStatistics();
        
        try
        {
            var totalText = await Page.TextContentAsync(TotalProceduresCount);
            stats.TotalProcedures = int.Parse(totalText?.Replace("Łącznie: ", "") ?? "0");
            
            var supervisedText = await Page.TextContentAsync(SupervisedCount);
            stats.SupervisedProcedures = int.Parse(supervisedText?.Replace("Pod nadzorem: ", "") ?? "0");
            
            var unsupervisedText = await Page.TextContentAsync(UnsupervisedCount);
            stats.UnsupervisedProcedures = int.Parse(unsupervisedText?.Replace("Samodzielnie: ", "") ?? "0");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error getting procedure statistics");
        }
        
        return stats;
    }
}

// Data models
public class ProcedureData
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string IcdCode { get; set; } = "";
    public string Description { get; set; } = "";
    public bool Supervised { get; set; } = true;
    public int? PatientAge { get; set; }
}

public class ProcedureListItem
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public DateTime Date { get; set; }
    public bool Supervised { get; set; }
}

public class ProcedureStatistics
{
    public int TotalProcedures { get; set; }
    public int SupervisedProcedures { get; set; }
    public int UnsupervisedProcedures { get; set; }
}