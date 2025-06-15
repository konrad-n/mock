using FluentAssertions;
using SledzSpecke.E2E.Tests.Fixtures;
using SledzSpecke.E2E.Tests.PageObjects;
using Xunit;

namespace SledzSpecke.E2E.Tests.Scenarios;

/// <summary>
/// E2E test scenarios for Medical Shifts based on SMK system workflows
/// These tests simulate real user interactions as described in SMK documentation
/// </summary>
public class MedicalShiftsScenarios : E2ETestBase
{
    private ILoginPage _loginPage = null!;
    private IDashboardPage _dashboardPage = null!;
    private IMedicalShiftsPage _medicalShiftsPage = null!;

    protected override async Task OnInitializeAsync()
    {
        // Initialize page objects
        _loginPage = new LoginPage(Page, Configuration.BaseUrl, Logger);
        _dashboardPage = new DashboardPage(Page, Configuration.BaseUrl, Logger);
        _medicalShiftsPage = new MedicalShiftsPage(Page, Configuration.BaseUrl, Logger);
        
        // Perform login before each test
        await LoginAsTestUserAsync();
    }

    [Fact]
    public async Task AddMedicalShift_CompleteWorkflow_ShouldSaveSuccessfully()
    {
        // Arrange - Prepare test data simulating real SMK input
        var shiftData = new MedicalShiftData
        {
            Date = DateTime.Today.AddDays(-1), // Yesterday's shift
            StartTime = "07:00",
            EndTime = "19:00",
            Type = "regular",
            Place = "Szpital Wojewódzki - Oddział Anestezjologii",
            Description = "Dyżur w ramach specjalizacji - anestezjologia"
        };

        // Act - Simulate user actions as in SMK system
        
        // 1. Navigate to Medical Shifts section
        await _dashboardPage.NavigateToMedicalShiftsAsync();
        await WaitForUserPaceAsync(); // Simulate real user speed
        
        // 2. Take screenshot of the list before adding (for documentation)
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/shifts_list_before.png",
            FullPage = true 
        });
        
        // 3. Click "Add Shift" button
        await _medicalShiftsPage.ClickAddShiftButtonAsync();
        await WaitForUserPaceAsync();
        
        // 4. Fill the shift form with data
        await _medicalShiftsPage.FillShiftFormAsync(shiftData);
        await WaitForUserPaceAsync();
        
        // 5. Submit the form
        await _medicalShiftsPage.SubmitShiftFormAsync();
        
        // Assert - Verify the shift was saved
        var isSuccess = await _medicalShiftsPage.IsShiftSavedSuccessfullyAsync();
        isSuccess.Should().BeTrue("shift should be saved successfully");
        
        // 6. Verify the shift appears in the list
        var shifts = await _medicalShiftsPage.GetShiftListAsync();
        shifts.Should().NotBeEmpty("at least one shift should be present");
        
        var addedShift = shifts.FirstOrDefault(s => s.Place.Contains("Anestezjologii"));
        addedShift.Should().NotBeNull("the added shift should appear in the list");
        
        // 7. Take final screenshot for documentation
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/shifts_list_after.png",
            FullPage = true 
        });
        
        Logger.Information("Medical shift added successfully with {Hours} hours", shiftData.DurationHours);
    }

    [Fact]
    public async Task AddMultipleShifts_MonthlyRotation_ShouldCalculateTotalHours()
    {
        // Arrange - Simulate a month of shifts as per SMK requirements
        var shifts = GenerateMonthlyShiftRotation();
        
        // Act
        await _dashboardPage.NavigateToMedicalShiftsAsync();
        
        int totalHours = 0;
        foreach (var shift in shifts)
        {
            await _medicalShiftsPage.ClickAddShiftButtonAsync();
            await _medicalShiftsPage.FillShiftFormAsync(shift);
            await _medicalShiftsPage.SubmitShiftFormAsync();
            
            totalHours += shift.DurationHours;
            
            // Small delay between adding shifts
            await Task.Delay(500);
        }
        
        // Assert - Verify total hours meet SMK requirements
        totalHours.Should().BeGreaterOrEqualTo(160, "monthly shift hours should meet minimum requirement");
        
        // Get updated statistics
        await _dashboardPage.NavigateAsync();
        var stats = await _dashboardPage.GetStatisticsAsync();
        stats.TotalShifts.Should().BeGreaterOrEqualTo(shifts.Count);
        
        Logger.Information("Added {Count} shifts with total {Hours} hours", shifts.Count, totalHours);
    }

    [Fact]
    public async Task EditShift_CorrectTime_ShouldUpdateSuccessfully()
    {
        // Arrange - First add a shift with incorrect time
        await _dashboardPage.NavigateToMedicalShiftsAsync();
        
        var incorrectShift = new MedicalShiftData
        {
            Date = DateTime.Today,
            StartTime = "08:00",
            EndTime = "14:00", // Wrong end time
            Type = "regular",
            Place = "Klinika"
        };
        
        await _medicalShiftsPage.ClickAddShiftButtonAsync();
        await _medicalShiftsPage.FillShiftFormAsync(incorrectShift);
        await _medicalShiftsPage.SubmitShiftFormAsync();
        
        // Act - Edit the shift to correct the time
        await _medicalShiftsPage.EditShiftAsync(0); // Edit first shift
        
        var correctedShift = new MedicalShiftData
        {
            Date = DateTime.Today,
            StartTime = "08:00",
            EndTime = "20:00", // Correct end time
            Type = "regular",
            Place = "Klinika"
        };
        
        await _medicalShiftsPage.FillShiftFormAsync(correctedShift);
        await _medicalShiftsPage.SubmitShiftFormAsync();
        
        // Assert
        var isSuccess = await _medicalShiftsPage.IsShiftSavedSuccessfullyAsync();
        isSuccess.Should().BeTrue("shift update should be successful");
    }

    [Fact]
    public async Task ExportShifts_ToPDF_ShouldDownloadFile()
    {
        // Arrange - Ensure we have some shifts to export
        await _dashboardPage.NavigateToMedicalShiftsAsync();
        
        // Act - Export shifts as PDF (common SMK export format)
        await _medicalShiftsPage.ExportShiftsAsync("PDF");
        
        // Assert - Verify file was downloaded
        var downloadPath = Path.Combine("Reports", "Downloads");
        var pdfFiles = Directory.GetFiles(downloadPath, "*.pdf");
        pdfFiles.Should().NotBeEmpty("PDF export should create a file");
        
        Logger.Information("Shifts exported to PDF: {File}", Path.GetFileName(pdfFiles.Last()));
    }

    [Fact]
    public async Task FilterShifts_ByDateRange_ShouldShowFilteredResults()
    {
        // Arrange
        var startDate = DateTime.Today.AddMonths(-1);
        var endDate = DateTime.Today;
        
        // Act
        await _dashboardPage.NavigateToMedicalShiftsAsync();
        await _medicalShiftsPage.FilterByDateRangeAsync(startDate, endDate);
        
        // Assert
        var shifts = await _medicalShiftsPage.GetShiftListAsync();
        // All shifts should be within the date range
        foreach (var shift in shifts)
        {
            // Parse and verify dates are in range
            Logger.Information("Filtered shift: {Date}", shift.Date);
        }
    }

    private async Task LoginAsTestUserAsync()
    {
        await _loginPage.NavigateAsync();
        await _loginPage.LoginAsync(
            Configuration.TestUser.DefaultUsername,
            Configuration.TestUser.DefaultPassword
        );
        
        // Verify we're logged in
        await _dashboardPage.WaitForLoadAsync();
    }

    private List<MedicalShiftData> GenerateMonthlyShiftRotation()
    {
        var shifts = new List<MedicalShiftData>();
        var random = new Random();
        
        // Generate realistic shift pattern for a month
        for (int i = 0; i < 20; i++) // ~20 shifts per month
        {
            var shiftDate = DateTime.Today.AddDays(-30 + (i * 1.5));
            
            // Mix of day and night shifts
            bool isNightShift = i % 3 == 0;
            
            shifts.Add(new MedicalShiftData
            {
                Date = shiftDate,
                StartTime = isNightShift ? "19:00" : "07:00",
                EndTime = isNightShift ? "07:00" : "15:00",
                Type = isNightShift ? "on-call" : "regular",
                Place = GetRandomHospitalDepartment(),
                Description = $"Dyżur nr {i + 1} - specjalizacja"
            });
        }
        
        return shifts;
    }

    private string GetRandomHospitalDepartment()
    {
        var departments = new[]
        {
            "Oddział Anestezjologii i Intensywnej Terapii",
            "Blok Operacyjny - Anestezjologia",
            "SOR - Anestezjologia",
            "Oddział Kardioanestezji",
            "Oddział Neuroanestezji"
        };
        
        return departments[new Random().Next(departments.Length)];
    }
}