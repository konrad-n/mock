using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;
using SledzSpecke.E2E.Tests.Fixtures;
using SledzSpecke.E2E.Tests.PageObjects;
using SledzSpecke.E2E.Tests.Infrastructure;
using Serilog;

namespace SledzSpecke.E2E.Tests.Scenarios;

[Collection("E2E Tests")]
public class PerformanceScenarios : E2ETestBase
{
    private readonly ITestOutputHelper _output;
    private readonly ILogger _logger;
    
    public PerformanceScenarios(ITestOutputHelper output)
    {
        _output = output;
        _logger = Log.ForContext<PerformanceScenarios>();
    }
    
    [Fact]
    public async Task LoadTest_Add100MedicalShifts_CompletesUnder30Seconds()
    {
        // Arrange
        await LoginAsTestUser();
        var shiftsPage = new MedicalShiftsPage(Page, Configuration.BaseUrl, _logger);
        await shiftsPage.NavigateAsync();
        
        var stopwatch = Stopwatch.StartNew();
        
        // Act - Add 100 shifts using parallel API calls
        var tasks = new List<Task>();
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(AddShiftViaApiAsync(i));
        }
        
        await Task.WhenAll(tasks);
        
        // Assert
        stopwatch.Stop();
        _output.WriteLine($"Added 100 shifts in {stopwatch.ElapsedMilliseconds}ms");
        _logger.Information("Performance test: Added 100 shifts in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        
        Assert.True(stopwatch.ElapsedMilliseconds < 30000, 
            $"Operation took {stopwatch.ElapsedMilliseconds}ms, expected under 30000ms");
        
        // Verify shifts were added
        await Page.ReloadAsync();
        var shiftCount = await shiftsPage.GetShiftCountAsync();
        Assert.True(shiftCount >= 100, $"Expected at least 100 shifts, but found {shiftCount}");
    }
    
    [Fact]
    public async Task PageLoad_DashboardWithFullData_LoadsUnder3Seconds()
    {
        // Arrange - Create user with lots of data
        await SeedLargeDataset();
        await LoginAsTestUser();
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        await Page.GotoAsync($"{Configuration.BaseUrl}/dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        stopwatch.Stop();
        
        // Assert
        _output.WriteLine($"Dashboard loaded in {stopwatch.ElapsedMilliseconds}ms");
        _logger.Information("Dashboard load time: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        
        Assert.True(stopwatch.ElapsedMilliseconds < 3000,
            $"Dashboard load took {stopwatch.ElapsedMilliseconds}ms, expected under 3000ms");
        
        // Verify dashboard elements are visible
        await Page.WaitForSelectorAsync("[data-testid='dashboard-overview']");
        await Page.WaitForSelectorAsync("[data-testid='progress-charts']");
    }
    
    [Fact]
    public async Task Export_LargeDataset_GeneratesUnder10Seconds()
    {
        // Arrange
        await SeedLargeDataset();
        await LoginAsTestUser();
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        await Page.GotoAsync($"{Configuration.BaseUrl}/export");
        
        var downloadTask = Page.WaitForDownloadAsync();
        await Page.ClickAsync("button:has-text('Export to Excel')");
        var download = await downloadTask;
        
        stopwatch.Stop();
        
        // Assert
        _output.WriteLine($"Export completed in {stopwatch.ElapsedMilliseconds}ms");
        _logger.Information("Export time: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        
        Assert.True(stopwatch.ElapsedMilliseconds < 10000,
            $"Export took {stopwatch.ElapsedMilliseconds}ms, expected under 10000ms");
        
        // Verify file size
        var path = await download.PathAsync();
        if (path != null)
        {
            var fileInfo = new System.IO.FileInfo(path);
            _output.WriteLine($"Export file size: {fileInfo.Length / 1024}KB");
            _logger.Information("Export file size: {SizeKB}KB", fileInfo.Length / 1024);
        }
    }
    
    [Fact]
    public async Task ConcurrentUsers_5SimultaneousLogins_AllSucceed()
    {
        // Arrange
        var userTasks = new List<Task<long>>();
        
        // Act - Simulate 5 concurrent users
        for (int i = 0; i < 5; i++)
        {
            var userId = i;
            userTasks.Add(Task.Run(async () =>
            {
                var context = await BrowserFactory.CreateBrowserContextAsync();
                var page = await context.NewPageAsync();
                
                var stopwatch = Stopwatch.StartNew();
                
                var loginPage = new LoginPage(page, Configuration.BaseUrl, _logger);
                await loginPage.NavigateAsync();
                await loginPage.LoginAsync($"user{userId}@test.pl", "Test123!");
                
                // Verify dashboard loads
                await page.WaitForSelectorAsync("[data-testid='dashboard-overview']");
                
                stopwatch.Stop();
                
                await context.CloseAsync();
                
                return stopwatch.ElapsedMilliseconds;
            }));
        }
        
        var results = await Task.WhenAll(userTasks);
        
        // Assert
        var avgTime = results.Average();
        var maxTime = results.Max();
        
        _output.WriteLine($"Average login time: {avgTime}ms");
        _output.WriteLine($"Max login time: {maxTime}ms");
        _logger.Information("Concurrent login test - Avg: {AvgMs}ms, Max: {MaxMs}ms", avgTime, maxTime);
        
        Assert.True(maxTime < 5000, $"Slowest login took {maxTime}ms, expected under 5000ms");
    }
    
    [Fact]
    public async Task SearchPerformance_1000Procedures_ReturnsUnder1Second()
    {
        // Arrange
        await SeedLargeProcedureDataset();
        await LoginAsTestUser();
        
        var proceduresPage = new ProceduresPage(Page, Configuration.BaseUrl, _logger);
        await proceduresPage.NavigateAsync();
        
        // Act - Search for specific procedure
        var stopwatch = Stopwatch.StartNew();
        
        await Page.FillAsync("input[placeholder='Szukaj procedury...']", "Echokardiografia");
        await Page.PressAsync("input[placeholder='Szukaj procedury...']", "Enter");
        
        // Wait for results
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        stopwatch.Stop();
        
        // Assert
        _output.WriteLine($"Search completed in {stopwatch.ElapsedMilliseconds}ms");
        _logger.Information("Search performance: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        
        Assert.True(stopwatch.ElapsedMilliseconds < 1000,
            $"Search took {stopwatch.ElapsedMilliseconds}ms, expected under 1000ms");
    }
    
    [Fact]
    public async Task MemoryUsage_NavigateAllPages_NoMemoryLeaks()
    {
        // Arrange
        await LoginAsTestUser();
        
        // Get initial memory usage
        var initialMemory = await GetMemoryUsageAsync();
        _output.WriteLine($"Initial memory usage: {initialMemory / 1024 / 1024}MB");
        
        // Act - Navigate through all main pages multiple times
        for (int i = 0; i < 3; i++)
        {
            await NavigateAllPages();
        }
        
        // Force garbage collection in browser
        await Page.EvaluateAsync("() => { if (window.gc) window.gc(); }");
        await Task.Delay(1000); // Wait for GC
        
        // Get final memory usage
        var finalMemory = await GetMemoryUsageAsync();
        _output.WriteLine($"Final memory usage: {finalMemory / 1024 / 1024}MB");
        
        // Assert - Memory should not increase by more than 50MB
        var memoryIncrease = (finalMemory - initialMemory) / 1024 / 1024;
        _output.WriteLine($"Memory increase: {memoryIncrease}MB");
        
        Assert.True(memoryIncrease < 50, 
            $"Memory increased by {memoryIncrease}MB, expected less than 50MB");
    }
    
    private async Task AddShiftViaApiAsync(int index)
    {
        var shift = new ShiftData
        {
            Date = DateTime.Today.AddDays(-index),
            Hours = 8,
            Minutes = 0,
            Location = "Test Location",
            ShiftType = "regular"
        };
            
        await Page.EvaluateAsync(@"
            async (shift) => {
                const response = await fetch('/api/medical-shifts', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + localStorage.getItem('token')
                    },
                    body: JSON.stringify(shift)
                });
                return response.ok;
            }
        ", new 
        {
            internshipId = 1,
            date = shift.Date.ToString("yyyy-MM-dd"),
            hours = shift.Hours,
            minutes = shift.Minutes,
            type = shift.ShiftType
        });
    }
    
    private async Task SeedLargeDataset()
    {
        _logger.Information("Seeding large dataset for performance testing");
        
        // This would typically be done via API or direct database access
        // For E2E tests, we'll add a reasonable amount of test data
        
        // Add 500 medical shifts
        for (int i = 0; i < 500; i++)
        {
            await AddShiftViaApiAsync(i);
        }
        
        // Add 200 procedures via API
        for (int i = 0; i < 200; i++)
        {
            await AddProcedureViaApiAsync(i);
        }
    }
    
    private async Task SeedLargeProcedureDataset()
    {
        _logger.Information("Seeding large procedure dataset");
        
        // Add 1000 procedures with various names
        var procedureNames = new[]
        {
            "Echokardiografia", "Koronarografia", "Test wysiłkowy",
            "Holter EKG", "Gastroskopia", "Kolonoskopia"
        };
        
        for (int i = 0; i < 1000; i++)
        {
            await AddProcedureViaApiAsync(i, procedureNames[i % procedureNames.Length]);
        }
    }
    
    private async Task AddProcedureViaApiAsync(int index, string name = "Test Procedure")
    {
        var procedure = new ProcedureTestData
        {
            Date = DateTime.Today.AddDays(-index),
            Name = name,
            Category = "Test Category",
            IcdCode = "00.00",
            ExecutionType = "Performed",
            Supervisor = ""
        };
            
        await Page.EvaluateAsync(@"
            async (procedure) => {
                const response = await fetch('/api/procedures', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + localStorage.getItem('token')
                    },
                    body: JSON.stringify(procedure)
                });
                return response.ok;
            }
        ", new 
        {
            internshipId = 1,
            date = procedure.Date.ToString("yyyy-MM-dd"),
            name = procedure.Name,
            category = procedure.Category,
            icdCode = procedure.IcdCode,
            supervised = procedure.Supervisor != ""
        });
    }
    
    private async Task NavigateAllPages()
    {
        var pages = new[]
        {
            "/dashboard",
            "/medical-shifts",
            "/procedures",
            "/internships",
            "/courses",
            "/self-education",
            "/export"
        };
        
        foreach (var page in pages)
        {
            await Page.GotoAsync($"{Configuration.BaseUrl}{page}");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Task.Delay(500); // Simulate user reading the page
        }
    }
    
    private async Task<long> GetMemoryUsageAsync()
    {
        var metrics = await Page.EvaluateAsync<Dictionary<string, object>>(@"
            () => {
                if (performance.memory) {
                    return {
                        usedJSHeapSize: performance.memory.usedJSHeapSize,
                        totalJSHeapSize: performance.memory.totalJSHeapSize
                    };
                }
                return { usedJSHeapSize: 0, totalJSHeapSize: 0 };
            }
        ");
        
        return Convert.ToInt64(metrics["usedJSHeapSize"]);
    }
    
    private async Task LoginAsTestUser()
    {
        var loginPage = new LoginPage(Page, Configuration.BaseUrl, _logger);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("test@example.com", "Test123!");
    }
}