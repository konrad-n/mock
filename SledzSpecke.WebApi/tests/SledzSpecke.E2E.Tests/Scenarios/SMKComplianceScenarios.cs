using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using SledzSpecke.E2E.Tests.Fixtures;
using SledzSpecke.E2E.Tests.PageObjects;
using SledzSpecke.Tests.Common.Builders.Domain;
using Xunit;
using Serilog;

namespace SledzSpecke.E2E.Tests.Scenarios;

[Collection("E2E Tests")]
public class SMKComplianceScenarios : E2ETestBase
{
    private readonly ILogger _logger;
    
    public SMKComplianceScenarios(E2ETestFixture fixture) : base(fixture)
    {
        _logger = Log.ForContext<SMKComplianceScenarios>();
    }
    
    [Fact]
    public async Task CompleteCardiologyModule_MeetsAllSMKRequirements()
    {
        _logger.Information("Starting SMK compliance test for Cardiology module");
        
        // Arrange - Login as cardiology resident
        var loginPage = new LoginPage(Page, Configuration.BaseUrl, _logger);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("jan.kowalski@kardiologia.pl", "Test123!");
        
        // Act - Complete 2 years of basic module
        await CompleteBasicModuleRequirements();
        
        // Assert - Verify SMK compliance
        await VerifySMKCompliance();
    }
    
    private async Task CompleteBasicModuleRequirements()
    {
        _logger.Information("Completing basic module requirements");
        
        // 1. Add required medical shifts (average 10h 5min per week)
        await AddMedicalShiftsForBasicModule();
        
        // 2. Add required procedures (based on SMK PDF)
        await AddRequiredProcedures();
        
        // 3. Add required courses
        await AddRequiredCourses();
        
        // 4. Add self-education activities
        await AddSelfEducationActivities();
    }
    
    private async Task AddMedicalShiftsForBasicModule()
    {
        _logger.Information("Adding medical shifts for basic module");
        
        var shiftsPage = new MedicalShiftsPage(Page, Configuration.BaseUrl, _logger);
        await shiftsPage.NavigateAsync();
        
        // Add 2 years of shifts (104 weeks)
        // Each week needs average 10h 5min = 50.42 hours total per week
        var startDate = DateTime.Today.AddYears(-2);
        
        for (int week = 0; week < 104; week++)
        {
            var weekStart = startDate.AddDays(week * 7);
            
            // Monday-Thursday: 10 hours each
            for (int day = 0; day < 4; day++)
            {
                await shiftsPage.AddShiftAsync(new ShiftData
                {
                    Date = weekStart.AddDays(day),
                    Hours = 10,
                    Minutes = 0,
                    Type = "Oddział",
                    Location = "Oddział Chorób Wewnętrznych"
                });
            }
            
            // Friday: 10h 25min to get weekly average of 10h 5min
            await shiftsPage.AddShiftAsync(new ShiftData
            {
                Date = weekStart.AddDays(4),
                Hours = 10,
                Minutes = 25,
                Type = "Oddział",
                Location = "Oddział Chorób Wewnętrznych"
            });
            
            // Show progress every 10 weeks
            if (week % 10 == 0)
            {
                _logger.Information("Added shifts for week {Week} of 104", week);
            }
        }
    }
    
    private async Task AddRequiredProcedures()
    {
        _logger.Information("Adding required procedures based on SMK requirements");
        
        var proceduresPage = new ProceduresPage(Page, Configuration.BaseUrl, _logger);
        await proceduresPage.NavigateAsync();
        
        // Based on SMK PDF requirements for basic module
        var requiredProcedures = new[]
        {
            ("EKG interpretation", 500, 0, "88.72"),
            ("Echocardiography", 100, 50, "88.72"),
            ("Stress test", 50, 20, "89.41"),
            ("Holter monitoring", 30, 20, "89.50"),
            ("Blood pressure monitoring", 20, 10, "89.52")
        };
        
        foreach (var (name, codeA, codeB, icdCode) in requiredProcedures)
        {
            _logger.Information("Adding procedures for {Name}: {CodeA} performed, {CodeB} assisted", 
                name, codeA, codeB);
            
            // Add Code A procedures (performed independently)
            for (int i = 0; i < codeA; i++)
            {
                await proceduresPage.ClickAddProcedureButtonAsync();
                await proceduresPage.FillProcedureFormAsync(new ProcedureData
                {
                    Name = name,
                    Category = "Kardiologia",
                    Date = DateTime.Today.AddDays(-i),
                    Supervised = false,
                    IcdCode = icdCode,
                    Description = $"Procedure performed independently #{i+1}",
                    PatientAge = new Random().Next(30, 80)
                });
                await proceduresPage.SubmitProcedureFormAsync();
                await proceduresPage.IsProcedureSavedSuccessfullyAsync();
            }
            
            // Add Code B procedures (assisted)
            for (int i = 0; i < codeB; i++)
            {
                await proceduresPage.ClickAddProcedureButtonAsync();
                await proceduresPage.FillProcedureFormAsync(new ProcedureData
                {
                    Name = name,
                    Category = "Kardiologia", 
                    Date = DateTime.Today.AddDays(-i-codeA),
                    Supervised = true,
                    IcdCode = icdCode,
                    Description = $"Procedure assisted #{i+1}",
                    PatientAge = new Random().Next(30, 80)
                });
                await proceduresPage.SubmitProcedureFormAsync();
                await proceduresPage.IsProcedureSavedSuccessfullyAsync();
            }
        }
    }
    
    private async Task AddRequiredCourses()
    {
        _logger.Information("Adding mandatory courses from SMK");
        
        await Page.GotoAsync($"{Configuration.BaseUrl}/courses");
        
        // Add mandatory courses from SMK
        var mandatoryCourses = new[]
        {
            ("Kurs wprowadzający", "CMKP/2023/001", DateTime.Today.AddMonths(-20)),
            ("Ratownictwo medyczne", "CMKP/2023/002", DateTime.Today.AddMonths(-18)),
            ("Przetaczanie krwi", "CMKP/2023/003", DateTime.Today.AddMonths(-16)),
            ("Orzecznictwo lekarskie", "CMKP/2024/001", DateTime.Today.AddMonths(-12)),
            ("Zdrowie publiczne", "CMKP/2024/002", DateTime.Today.AddMonths(-6))
        };
        
        foreach (var (name, certificate, date) in mandatoryCourses)
        {
            _logger.Information("Adding course: {Name}", name);
            
            await Page.ClickAsync("button:has-text('Dodaj kurs')");
            await Page.FillAsync("input[name='name']", name);
            await Page.FillAsync("input[name='certificate']", certificate);
            await Page.FillAsync("input[name='date']", date.ToString("yyyy-MM-dd"));
            await Page.ClickAsync("button[type='submit']");
            await Page.WaitForSelectorAsync($"text={name}");
        }
    }
    
    private async Task AddSelfEducationActivities()
    {
        _logger.Information("Adding self-education activities");
        
        await Page.GotoAsync($"{Configuration.BaseUrl}/self-education");
        
        // Add conferences
        await Page.ClickAsync("button:has-text('Dodaj konferencję')");
        await Page.FillAsync("input[name='name']", "Europejski Kongres Kardiologiczny");
        await Page.FillAsync("input[name='date']", DateTime.Today.AddMonths(-3).ToString("yyyy-MM-dd"));
        await Page.FillAsync("input[name='hours']", "16");
        await Page.ClickAsync("button[type='submit']");
        
        // Add publication
        await Page.ClickAsync("button:has-text('Dodaj publikację')");
        await Page.FillAsync("input[name='title']", "Nowe metody leczenia niewydolności serca");
        await Page.FillAsync("input[name='journal']", "Kardiologia Polska");
        await Page.FillAsync("input[name='doi']", "10.1234/kp.2024.001");
        await Page.SelectOptionAsync("select[name='type']", "first-author");
        await Page.ClickAsync("button[type='submit']");
    }
    
    private async Task VerifySMKCompliance()
    {
        _logger.Information("Verifying SMK compliance");
        
        // Navigate to SMK validation
        await Page.GotoAsync($"{Configuration.BaseUrl}/smk/validation");
        
        // Wait for validation to complete
        await Page.WaitForSelectorAsync(".validation-success", new() { Timeout = 30000 });
        
        // Check all requirements are met
        var validationItems = await Page.QuerySelectorAllAsync(".validation-item");
        foreach (var item in validationItems)
        {
            var status = await item.GetAttributeAsync("data-status");
            Assert.Equal("passed", status);
            
            var requirement = await item.GetAttributeAsync("data-requirement");
            _logger.Information("Requirement {Requirement}: {Status}", requirement, status);
        }
        
        // Export SMK file
        _logger.Information("Exporting SMK file");
        await Page.ClickAsync("button:has-text('Eksportuj do SMK')");
        
        var download = await Page.RunAndWaitForDownloadAsync(async () =>
        {
            await Page.ClickAsync("button:has-text('Potwierdź eksport')");
        });
        
        var fileName = $"SMK_Kardiologia_{DateTime.Now:yyyyMMdd}.xlsx";
        await download.SaveAsAsync($"Reports/Downloads/{fileName}");
        
        // Verify file was created
        Assert.True(System.IO.File.Exists($"Reports/Downloads/{fileName}"));
        _logger.Information("SMK file exported successfully: {FileName}", fileName);
    }
    
    [Fact]
    public async Task AnesthesiologySpecialization_OldSMK_MeetsRequirements()
    {
        _logger.Information("Starting SMK compliance test for Anesthesiology (old SMK)");
        
        // Login as anesthesiology resident
        var loginPage = new LoginPage(Page, Configuration.BaseUrl, _logger);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("anna.nowak@anestezjologia.pl", "Test123!");
        
        // Complete anesthesiology-specific requirements
        await CompleteAnesthesiologyRequirements();
        
        // Verify compliance
        await VerifySMKCompliance();
    }
    
    private async Task CompleteAnesthesiologyRequirements()
    {
        _logger.Information("Completing anesthesiology-specific requirements");
        
        var proceduresPage = new ProceduresPage(Page, Configuration.BaseUrl, _logger);
        await proceduresPage.NavigateAsync();
        
        // Anesthesiology-specific procedures from SMK
        var anesthesiologyProcedures = new[]
        {
            ("Intubacja dotchawicza", 200, 50, "96.04"),
            ("Kaniulacja żyły centralnej", 100, 30, "38.93"),
            ("Znieczulenie podpajęczynówkowe", 80, 20, "03.91"),
            ("Blokada splotu ramiennego", 50, 20, "04.81"),
            ("Znieczulenie ogólne", 300, 0, "00.01")
        };
        
        foreach (var (name, performed, assisted, icdCode) in anesthesiologyProcedures)
        {
            _logger.Information("Adding {Name} procedures", name);
            
            // Add performed procedures
            for (int i = 0; i < performed; i++)
            {
                await proceduresPage.ClickAddProcedureButtonAsync();
                await proceduresPage.FillProcedureFormAsync(new ProcedureData
                {
                    Name = name,
                    Category = "Anestezjologia",
                    Date = DateTime.Today.AddDays(-i * 2),
                    Supervised = false,
                    IcdCode = icdCode,
                    Description = $"Zabieg wykonany samodzielnie #{i+1}"
                });
                await proceduresPage.SubmitProcedureFormAsync();
                
                // Add small delay to avoid overwhelming the system
                if (i % 10 == 0)
                {
                    await Task.Delay(100);
                }
            }
        }
    }
    
    [Fact]
    public async Task VerifyWeeklyHourLimits_NotExceeded()
    {
        _logger.Information("Verifying weekly hour limits compliance");
        
        // Login
        var loginPage = new LoginPage(Page, Configuration.BaseUrl, _logger);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("test.resident@kardiologia.pl", "Test123!");
        
        // Navigate to shifts page
        var shiftsPage = new MedicalShiftsPage(Page, Configuration.BaseUrl, _logger);
        await shiftsPage.NavigateAsync();
        
        // Try to add shifts exceeding 48 hours per week
        var monday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
        
        // Add 5 days of 10 hours each (50 hours total)
        for (int i = 0; i < 5; i++)
        {
            await shiftsPage.AddShiftAsync(new ShiftData
            {
                Date = monday.AddDays(i),
                Hours = 10,
                Minutes = 0,
                Type = "Oddział"
            });
        }
        
        // Verify warning is displayed
        var warningElement = await Page.QuerySelectorAsync("[data-testid='weekly-hours-warning']");
        Assert.NotNull(warningElement);
        
        var warningText = await warningElement.TextContentAsync();
        Assert.Contains("48 godzin", warningText);
        
        _logger.Information("Weekly hour limit warning displayed correctly");
    }
}