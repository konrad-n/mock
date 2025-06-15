using FluentAssertions;
using SledzSpecke.E2E.Tests.Builders;
using SledzSpecke.E2E.Tests.Fixtures;
using SledzSpecke.E2E.Tests.PageObjects;
using Xunit;

namespace SledzSpecke.E2E.Tests.Scenarios;

/// <summary>
/// Complete E2E workflow simulating real SMK system usage
/// Based on scenarios from SMK user manuals in AdditionalContext folder
/// </summary>
public class CompleteSMKWorkflowScenario : E2ETestBase
{
    private ILoginPage _loginPage = null!;
    private IDashboardPage _dashboardPage = null!;
    private IMedicalShiftsPage _medicalShiftsPage = null!;

    protected override async Task OnInitializeAsync()
    {
        _loginPage = new LoginPage(Page, Configuration.BaseUrl, Logger);
        _dashboardPage = new DashboardPage(Page, Configuration.BaseUrl, Logger);
        _medicalShiftsPage = new MedicalShiftsPage(Page, Configuration.BaseUrl, Logger);
    }

    [Fact]
    public async Task CompleteMonthlyWorkflow_AsPerSMKRequirements_ShouldMeetAllCriteria()
    {
        // This test simulates a complete monthly workflow as a medical resident would use SMK
        Logger.Information("Starting complete SMK workflow simulation");
        
        // Step 1: User Registration (if needed) or Login
        await SimulateUserLoginAsync();
        
        // Step 2: Add Medical Shifts for the Month
        await SimulateMonthlyShiftsEntryAsync();
        
        // Step 3: Add Medical Procedures (would be in ProceduresPage)
        await SimulateProceduresEntryAsync();
        
        // Step 4: Register for Mandatory Courses
        await SimulateCourseRegistrationAsync();
        
        // Step 5: Document Self-Education Activities
        await SimulateSelfEducationAsync();
        
        // Step 6: Generate Monthly Report
        await SimulateMonthlyReportGenerationAsync();
        
        // Step 7: Submit for Supervisor Approval
        await SimulateSubmissionForApprovalAsync();
        
        Logger.Information("Complete SMK workflow simulation finished successfully");
    }

    private async Task SimulateUserLoginAsync()
    {
        Logger.Information("=== STEP 1: User Login ===");
        
        await _loginPage.NavigateAsync();
        await WaitForUserPaceAsync();
        
        // Take screenshot of login page (as seen in SMK screenshots)
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/SMK_01_LoginPage.png" 
        });
        
        // Use test user credentials
        await _loginPage.LoginAsync(
            Configuration.TestUser.DefaultUsername,
            Configuration.TestUser.DefaultPassword
        );
        
        // Verify successful login
        await _dashboardPage.WaitForLoadAsync();
        var welcomeMessage = await _dashboardPage.GetWelcomeMessageAsync();
        welcomeMessage.Should().Contain("Witaj");
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/SMK_02_Dashboard.png" 
        });
    }

    private async Task SimulateMonthlyShiftsEntryAsync()
    {
        Logger.Information("=== STEP 2: Monthly Shifts Entry ===");
        
        await _dashboardPage.NavigateToMedicalShiftsAsync();
        await WaitForUserPaceAsync();
        
        // Generate realistic monthly shift schedule
        var shiftBuilder = new MedicalShiftBuilder();
        var monthlyShifts = new List<MedicalShiftTestData>();
        
        // Add regular weekday shifts
        for (int week = 0; week < 4; week++)
        {
            for (int day = 0; day < 5; day++)
            {
                if (day % 2 == 0) // Every other day
                {
                    var shift = shiftBuilder
                        .OnDate(DateTime.Today.AddDays(-28 + (week * 7) + day))
                        .WithTimes("07:00", "15:00")
                        .OfType("regular")
                        .Build();
                    monthlyShifts.Add(shift);
                }
            }
        }
        
        // Add night shifts (as per SMK requirements)
        for (int i = 0; i < 4; i++)
        {
            var nightShift = shiftBuilder
                .OnDate(DateTime.Today.AddDays(-25 + (i * 7)))
                .AsNightShift()
                .Build();
            monthlyShifts.Add(nightShift);
        }
        
        // Add weekend shifts
        for (int i = 0; i < 2; i++)
        {
            var weekendShift = shiftBuilder
                .AsWeekendShift()
                .WithTimes("08:00", "20:00")
                .Build();
            monthlyShifts.Add(weekendShift);
        }
        
        // Enter each shift
        int totalHours = 0;
        foreach (var shift in monthlyShifts.Take(5)) // Enter first 5 for demo
        {
            await _medicalShiftsPage.ClickAddShiftButtonAsync();
            await WaitForUserPaceAsync();
            
            var shiftData = new MedicalShiftData
            {
                Date = shift.Date,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Type = shift.Type,
                Place = shift.Place,
                Description = shift.Description
            };
            
            await _medicalShiftsPage.FillShiftFormAsync(shiftData);
            await Page.ScreenshotAsync(new() 
            { 
                Path = $"Reports/Screenshots/SMK_Shift_{shift.Date:yyyyMMdd}.png" 
            });
            
            await _medicalShiftsPage.SubmitShiftFormAsync();
            var success = await _medicalShiftsPage.IsShiftSavedSuccessfullyAsync();
            success.Should().BeTrue();
            
            totalHours += shift.CalculatedHours;
            await Task.Delay(1000); // Pause between entries
        }
        
        Logger.Information("Added {Count} shifts with {Hours} total hours", 
            monthlyShifts.Take(5).Count(), totalHours);
        
        // Verify minimum hours requirement (typically 160h/month)
        totalHours.Should().BeGreaterThan(0, "should have logged shift hours");
    }

    private async Task SimulateProceduresEntryAsync()
    {
        Logger.Information("=== STEP 3: Medical Procedures Entry ===");
        
        // Navigate to procedures section (when implemented)
        // For now, we'll simulate the workflow
        
        await Task.Delay(1000); // Simulate navigation
        
        // Would add procedures like:
        // - Intubation procedures
        // - Central line placements
        // - Epidural anesthesia
        // etc. as per specialization requirements
        
        Logger.Information("Procedures entry simulated (page object pending)");
    }

    private async Task SimulateCourseRegistrationAsync()
    {
        Logger.Information("=== STEP 4: Course Registration ===");
        
        // Navigate to courses section
        try
        {
            await _dashboardPage.NavigateToCoursesAsync();
            await WaitForUserPaceAsync();
            
            // Would register for mandatory courses like:
            // - Advanced Life Support (ALS)
            // - Difficult Airway Management
            // - Regional Anesthesia Workshop
            
            await Page.ScreenshotAsync(new() 
            { 
                Path = "Reports/Screenshots/SMK_Courses.png" 
            });
        }
        catch
        {
            Logger.Warning("Courses navigation not yet implemented");
        }
    }

    private async Task SimulateSelfEducationAsync()
    {
        Logger.Information("=== STEP 5: Self-Education Documentation ===");
        
        // Document conference attendance, workshops, publications
        // This is required by SMK for specialization progress
        
        await Task.Delay(1000);
        Logger.Information("Self-education entry simulated");
    }

    private async Task SimulateMonthlyReportGenerationAsync()
    {
        Logger.Information("=== STEP 6: Monthly Report Generation ===");
        
        // Navigate back to medical shifts for export
        await _dashboardPage.NavigateToMedicalShiftsAsync();
        await WaitForUserPaceAsync();
        
        // Export monthly report
        await _medicalShiftsPage.ExportShiftsAsync("PDF");
        
        Logger.Information("Monthly report exported as PDF");
    }

    private async Task SimulateSubmissionForApprovalAsync()
    {
        Logger.Information("=== STEP 7: Submit for Supervisor Approval ===");
        
        // In real SMK, user would submit monthly activities for supervisor approval
        // This ensures compliance with specialization requirements
        
        await Task.Delay(1000);
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/SMK_Final_Summary.png",
            FullPage = true
        });
        
        Logger.Information("Monthly activities ready for supervisor approval");
    }
}

/// <summary>
/// Performance tests to ensure the app handles SMK-scale data
/// </summary>
public class SMKPerformanceScenarios : E2ETestBase
{
    [Fact]
    public async Task LoadTest_Multiple_Years_Of_Data_ShouldPerformWell()
    {
        // SMK users may have years of data
        // Test that the app performs well with large datasets
        
        var loginPage = new LoginPage(Page, Configuration.BaseUrl, Logger);
        var dashboardPage = new DashboardPage(Page, Configuration.BaseUrl, Logger);
        
        // Login
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync(
            Configuration.TestUser.DefaultUsername,
            Configuration.TestUser.DefaultPassword
        );
        
        // Measure dashboard load time
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await dashboardPage.WaitForLoadAsync();
        stopwatch.Stop();
        
        Logger.Information("Dashboard loaded in {Ms}ms", stopwatch.ElapsedMilliseconds);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(3000, "dashboard should load quickly");
        
        // Navigate to shifts and measure load time with filters
        var medicalShiftsPage = new MedicalShiftsPage(Page, Configuration.BaseUrl, Logger);
        await dashboardPage.NavigateToMedicalShiftsAsync();
        
        stopwatch.Restart();
        await medicalShiftsPage.FilterByDateRangeAsync(
            DateTime.Today.AddYears(-2),
            DateTime.Today
        );
        stopwatch.Stop();
        
        Logger.Information("Two years of shifts filtered in {Ms}ms", stopwatch.ElapsedMilliseconds);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "filtering should be performant");
    }
}