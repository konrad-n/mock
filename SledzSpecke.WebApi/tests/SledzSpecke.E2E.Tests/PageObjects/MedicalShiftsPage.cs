using Microsoft.Playwright;
using Serilog;
using SledzSpecke.E2E.Tests.Core;

namespace SledzSpecke.E2E.Tests.PageObjects;

/// <summary>
/// Page Object for Medical Shifts management - crucial for SMK compliance
/// </summary>
public interface IMedicalShiftsPage : IPageObject
{
    Task ClickAddShiftButtonAsync();
    Task FillShiftFormAsync(MedicalShiftData shiftData);
    Task SubmitShiftFormAsync();
    Task<bool> IsShiftSavedSuccessfullyAsync();
    Task<List<MedicalShiftRow>> GetShiftListAsync();
    Task DeleteShiftAsync(int index);
    Task EditShiftAsync(int index);
    Task FilterByDateRangeAsync(DateTime from, DateTime to);
    Task ExportShiftsAsync(string format);
}

public class MedicalShiftsPage : PageObjectBase, IMedicalShiftsPage
{
    // Selectors
    private const string AddShiftButton = "button:has-text('Dodaj dyżur'), [data-testid='add-shift']";
    private const string ShiftForm = "form, [role='dialog']";
    private const string DateInput = "input[type='date'], input[name='date']";
    private const string StartTimeInput = "input[name='startTime'], input[placeholder*='początek']";
    private const string EndTimeInput = "input[name='endTime'], input[placeholder*='koniec']";
    private const string TypeSelect = "select[name='type'], [data-testid='shift-type']";
    private const string PlaceInput = "input[name='place'], input[placeholder*='miejsce']";
    private const string DescriptionInput = "textarea[name='description'], textarea[placeholder*='opis']";
    private const string SubmitButton = "button[type='submit']:has-text('Zapisz'), button:has-text('Dodaj')";
    private const string CancelButton = "button:has-text('Anuluj')";
    private const string SuccessMessage = "[role='alert'].success, .toast-success";
    private const string ShiftTable = "table, [data-testid='shifts-table']";
    private const string ShiftRow = "tbody tr";
    private const string DeleteButton = "button[aria-label*='Usuń'], button:has-text('Usuń')";
    private const string EditButton = "button[aria-label*='Edytuj'], button:has-text('Edytuj')";
    private const string ConfirmDeleteButton = "button:has-text('Potwierdź')";
    
    // Filters
    private const string DateFromFilter = "input[name='dateFrom']";
    private const string DateToFilter = "input[name='dateTo']";
    private const string ApplyFilterButton = "button:has-text('Filtruj')";
    
    // Export
    private const string ExportButton = "button:has-text('Eksportuj')";
    private const string ExportFormatSelect = "select[name='exportFormat']";
    
    protected override string PagePath => "/medical-shifts";
    protected override string PageIdentifier => "h1:has-text('Dyżury medyczne')";

    public MedicalShiftsPage(IPage page, string baseUrl, ILogger logger) 
        : base(page, baseUrl, logger)
    {
    }

    public async Task ClickAddShiftButtonAsync()
    {
        Logger.Information("Clicking add shift button");
        await ClickWithRetryAsync(AddShiftButton);
        
        // Wait for form to appear
        await Page.WaitForSelectorAsync(ShiftForm, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 5000
        });
    }

    public async Task FillShiftFormAsync(MedicalShiftData shiftData)
    {
        Logger.Information("Filling shift form with data: {@ShiftData}", shiftData);
        
        // Fill date
        await Page.FillAsync(DateInput, shiftData.Date.ToString("yyyy-MM-dd"));
        await Page.WaitForTimeoutAsync(200);
        
        // Fill start time
        await FillFieldAsync(StartTimeInput, shiftData.StartTime);
        await Page.WaitForTimeoutAsync(200);
        
        // Fill end time
        await FillFieldAsync(EndTimeInput, shiftData.EndTime);
        await Page.WaitForTimeoutAsync(200);
        
        // Select shift type
        if (!string.IsNullOrEmpty(shiftData.Type))
        {
            await SelectOptionAsync(TypeSelect, shiftData.Type);
            await Page.WaitForTimeoutAsync(200);
        }
        
        // Fill place
        if (!string.IsNullOrEmpty(shiftData.Place))
        {
            await FillFieldAsync(PlaceInput, shiftData.Place);
            await Page.WaitForTimeoutAsync(200);
        }
        
        // Fill description
        if (!string.IsNullOrEmpty(shiftData.Description))
        {
            await FillFieldAsync(DescriptionInput, shiftData.Description);
        }
        
        // Take screenshot of filled form
        await TakeScreenshotAsync("shift_form_filled");
    }

    public async Task SubmitShiftFormAsync()
    {
        Logger.Information("Submitting shift form");
        await ClickWithRetryAsync(SubmitButton);
        
        // Wait for form to disappear or success message
        await Page.WaitForSelectorAsync(SuccessMessage, new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 5000
        });
    }

    public async Task<bool> IsShiftSavedSuccessfullyAsync()
    {
        var successVisible = await ElementExistsAsync(SuccessMessage);
        if (successVisible)
        {
            var message = await GetTextAsync(SuccessMessage);
            Logger.Information("Shift save result: {Message}", message);
        }
        return successVisible;
    }

    public async Task<List<MedicalShiftRow>> GetShiftListAsync()
    {
        Logger.Information("Getting shift list");
        var shifts = new List<MedicalShiftRow>();
        
        try
        {
            await Page.WaitForSelectorAsync(ShiftTable, new PageWaitForSelectorOptions
            {
                Timeout = 5000
            });
            
            var rows = await Page.QuerySelectorAllAsync(ShiftRow);
            
            foreach (var row in rows)
            {
                var cells = await row.QuerySelectorAllAsync("td");
                if (cells.Count >= 4)
                {
                    var shift = new MedicalShiftRow
                    {
                        Date = await cells[0].TextContentAsync() ?? "",
                        Time = await cells[1].TextContentAsync() ?? "",
                        Type = await cells[2].TextContentAsync() ?? "",
                        Place = await cells[3].TextContentAsync() ?? ""
                    };
                    shifts.Add(shift);
                }
            }
            
            Logger.Information("Found {Count} shifts in the list", shifts.Count);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to get shift list");
        }
        
        return shifts;
    }

    public async Task DeleteShiftAsync(int index)
    {
        Logger.Information("Deleting shift at index {Index}", index);
        
        var deleteButtons = await Page.QuerySelectorAllAsync($"{ShiftRow} {DeleteButton}");
        if (index < deleteButtons.Count)
        {
            await deleteButtons[index].ClickAsync();
            
            // Wait for confirmation dialog
            await Page.WaitForSelectorAsync(ConfirmDeleteButton);
            await ClickWithRetryAsync(ConfirmDeleteButton);
            
            // Wait for success message
            await Page.WaitForSelectorAsync(SuccessMessage);
        }
    }

    public async Task EditShiftAsync(int index)
    {
        Logger.Information("Editing shift at index {Index}", index);
        
        var editButtons = await Page.QuerySelectorAllAsync($"{ShiftRow} {EditButton}");
        if (index < editButtons.Count)
        {
            await editButtons[index].ClickAsync();
            
            // Wait for form to appear
            await Page.WaitForSelectorAsync(ShiftForm);
        }
    }

    public async Task FilterByDateRangeAsync(DateTime from, DateTime to)
    {
        Logger.Information("Filtering shifts from {From} to {To}", from, to);
        
        await Page.FillAsync(DateFromFilter, from.ToString("yyyy-MM-dd"));
        await Page.FillAsync(DateToFilter, to.ToString("yyyy-MM-dd"));
        await ClickWithRetryAsync(ApplyFilterButton);
        
        // Wait for table to reload
        await Page.WaitForTimeoutAsync(1000);
    }

    public async Task ExportShiftsAsync(string format)
    {
        Logger.Information("Exporting shifts in format: {Format}", format);
        
        await ClickWithRetryAsync(ExportButton);
        await SelectOptionAsync(ExportFormatSelect, format);
        
        // Handle download
        var downloadTask = Page.WaitForDownloadAsync();
        await ClickWithRetryAsync("button:has-text('Pobierz')");
        
        var download = await downloadTask;
        var fileName = $"shifts_export_{DateTime.Now:yyyyMMdd_HHmmss}.{format.ToLower()}";
        await download.SaveAsAsync(Path.Combine("Reports", "Downloads", fileName));
        
        Logger.Information("Shifts exported to: {FileName}", fileName);
    }
}

public class MedicalShiftData
{
    public DateTime Date { get; set; } = DateTime.Today;
    public string StartTime { get; set; } = "08:00";
    public string EndTime { get; set; } = "16:00";
    public string Type { get; set; } = "regular"; // regular, on-call, emergency
    public string Place { get; set; } = "";
    public string Description { get; set; } = "";
    public int DurationHours => CalculateDuration();
    
    private int CalculateDuration()
    {
        if (TimeSpan.TryParse(StartTime, out var start) && TimeSpan.TryParse(EndTime, out var end))
        {
            var duration = end - start;
            if (duration.TotalHours < 0) // Overnight shift
            {
                duration = duration.Add(TimeSpan.FromHours(24));
            }
            return (int)duration.TotalHours;
        }
        return 0;
    }
}

public class MedicalShiftRow
{
    public string Date { get; set; } = "";
    public string Time { get; set; } = "";
    public string Type { get; set; } = "";
    public string Place { get; set; } = "";
}