using FluentAssertions;
using Microsoft.Playwright;
using SledzSpecke.E2E.Tests.Builders;
using SledzSpecke.E2E.Tests.Fixtures;
using SledzSpecke.E2E.Tests.PageObjects;
using Xunit;

namespace SledzSpecke.E2E.Tests.Scenarios;

/// <summary>
/// Complete user journey scenarios with proper test isolation
/// Each test runs in its own database instance
/// </summary>
public class UserJourneyScenarios : IsolatedE2ETestBase, IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture;
    
    public UserJourneyScenarios(E2ETestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task CompleteUserJourney_FromRegistrationToProcedures_ShouldSucceed()
    {
        Logger.Information("Starting complete user journey test");
        
        // STEP 1: User Registration
        var registrationData = await RegisterNewUserAsync();
        
        // STEP 2: Login with registered user
        await LoginWithRegisteredUserAsync(registrationData);
        
        // STEP 3: Check Dashboard
        await VerifyDashboardAccessAsync();
        
        // STEP 4: Add Medical Shifts
        await AddMedicalShiftsAsync();
        
        // STEP 5: Add Procedures
        await AddProceduresAsync();
        
        Logger.Information("Complete user journey test completed successfully");
    }
    
    [Fact]
    public async Task UserRegistration_WithValidPolishData_ShouldCreateAccount()
    {
        Logger.Information("=== SCENARIO 1: User Registration ===");
        
        // Navigate to registration page
        await Page.GotoAsync($"{Configuration.BaseUrl}/register");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Take screenshot of registration form
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/01_Registration_Form.png" 
        });
        
        // Fill registration form with Polish medical resident data
        var testData = TestDataBuilderExtensions.BuildPolishMedicalResident();
        
        await TypeLikeUserAsync("input[name='firstName']", testData.FirstName);
        await TypeLikeUserAsync("input[name='lastName']", testData.LastName);
        await TypeLikeUserAsync("input[name='email']", testData.Email);
        await TypeLikeUserAsync("input[name='password']", "SecurePass123!");
        await TypeLikeUserAsync("input[name='confirmPassword']", "SecurePass123!");
        
        // Select SMK version
        await Page.SelectOptionAsync("select[name='smkVersion']", "new");
        
        // Fill additional fields
        await TypeLikeUserAsync("input[name='location']", testData.Location);
        await TypeLikeUserAsync("input[name='university']", testData.University);
        await Page.SelectOptionAsync("select[name='year']", testData.Year.ToString());
        await TypeLikeUserAsync("input[name='phone']", testData.Phone);
        
        // Accept terms
        await Page.CheckAsync("input[name='acceptTerms']");
        
        // Submit registration
        var responsePromise = WaitForApiResponseAsync("/api/auth/register", async () =>
        {
            await Page.ClickAsync("button[type='submit']");
        });
        
        var response = await responsePromise;
        response.Status.Should().Be(200);
        
        // Verify success message
        await Page.WaitForSelectorAsync("text=Rejestracja zakończona sukcesem");
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/01_Registration_Success.png" 
        });
        
        Logger.Information("User registration completed successfully");
    }
    
    [Fact]
    public async Task LoginAfterRegistration_WithValidCredentials_ShouldAccessDashboard()
    {
        Logger.Information("=== SCENARIO 2: Login After Registration ===");
        
        // First create a user
        var userData = await CreateTestUserDirectlyAsync();
        
        // Navigate to login page
        await Page.GotoAsync($"{Configuration.BaseUrl}/login");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/02_Login_Page.png" 
        });
        
        // Fill login form
        await TypeLikeUserAsync("input[name='email']", userData.Email);
        await TypeLikeUserAsync("input[name='password']", userData.Password);
        
        // Submit login
        var responsePromise = WaitForApiResponseAsync("/api/auth/login", async () =>
        {
            await Page.ClickAsync("button[type='submit']");
        });
        
        var response = await responsePromise;
        response.Status.Should().Be(200);
        
        // Verify redirect to dashboard
        await Page.WaitForURLAsync($"{Configuration.BaseUrl}/dashboard");
        
        // Verify user is logged in
        var welcomeText = await Page.TextContentAsync("h1");
        welcomeText.Should().Contain($"Witaj, {userData.FirstName}");
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/02_Login_Success_Dashboard.png" 
        });
        
        Logger.Information("Login after registration completed successfully");
    }
    
    [Fact]
    public async Task Dashboard_ForLoggedInUser_ShouldDisplayAllSections()
    {
        Logger.Information("=== SCENARIO 3: Dashboard Check ===");
        
        // Setup: Create and login user
        var userData = await CreateTestUserDirectlyAsync();
        await LoginUserAsync(userData.Email, userData.Password);
        
        // Navigate to dashboard
        await Page.GotoAsync($"{Configuration.BaseUrl}/dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Verify dashboard sections
        var sections = new[]
        {
            ("Dyżury medyczne", "medical-shifts-card"),
            ("Procedury", "procedures-card"),
            ("Kursy", "courses-card"),
            ("Statystyki", "statistics-card")
        };
        
        foreach (var (sectionName, sectionId) in sections)
        {
            var sectionElement = await Page.QuerySelectorAsync($"[data-testid='{sectionId}']");
            sectionElement.Should().NotBeNull($"Section '{sectionName}' should be visible");
            
            var sectionText = await sectionElement!.TextContentAsync();
            sectionText.Should().Contain(sectionName);
        }
        
        // Check quick stats
        var totalHoursElement = await Page.QuerySelectorAsync("[data-testid='total-hours']");
        var totalHours = await totalHoursElement!.TextContentAsync();
        totalHours.Should().Contain("0"); // New user has 0 hours
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/03_Dashboard_Overview.png",
            FullPage = true
        });
        
        Logger.Information("Dashboard check completed successfully");
    }
    
    [Fact]
    public async Task MedicalShifts_ViewAndAdd_ShouldWorkCorrectly()
    {
        Logger.Information("=== SCENARIO 4: Medical Shifts (View and Add) ===");
        
        // Setup: Create and login user
        var userData = await CreateTestUserDirectlyAsync();
        await LoginUserAsync(userData.Email, userData.Password);
        
        // Navigate to medical shifts
        await Page.ClickAsync("[data-testid='medical-shifts-card']");
        await Page.WaitForURLAsync($"{Configuration.BaseUrl}/medical-shifts");
        
        // Verify empty state
        var emptyStateText = await Page.TextContentAsync("[data-testid='empty-shifts']");
        emptyStateText.Should().Contain("Nie masz jeszcze żadnych dyżurów");
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/04_Medical_Shifts_Empty.png" 
        });
        
        // Add new shift
        await Page.ClickAsync("button:has-text('Dodaj dyżur')");
        await Page.WaitForSelectorAsync("[data-testid='shift-form']");
        
        // Fill shift form
        var shiftData = new MedicalShiftBuilder()
            .OnDate(DateTime.Today)
            .WithTimes("08:00", "16:00")
            .OfType("regular")
            .Build();
        
        await Page.FillAsync("input[name='date']", shiftData.Date.ToString("yyyy-MM-dd"));
        await Page.FillAsync("input[name='startTime']", shiftData.StartTime);
        await Page.FillAsync("input[name='endTime']", shiftData.EndTime);
        await Page.SelectOptionAsync("select[name='type']", shiftData.Type);
        await Page.FillAsync("input[name='place']", shiftData.Place);
        await Page.FillAsync("textarea[name='description']", shiftData.Description);
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/04_Medical_Shift_Form_Filled.png" 
        });
        
        // Submit shift
        var responsePromise = WaitForApiResponseAsync("/api/medical-shifts", async () =>
        {
            await Page.ClickAsync("button[type='submit']");
        });
        
        var response = await responsePromise;
        response.Status.Should().Be(201);
        
        // Verify shift appears in list
        await Page.WaitForSelectorAsync("[data-testid='shift-item']");
        var shiftItem = await Page.TextContentAsync("[data-testid='shift-item']");
        shiftItem.Should().Contain(shiftData.Place);
        shiftItem.Should().Contain("8 godzin");
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/04_Medical_Shift_Added.png" 
        });
        
        Logger.Information("Medical shifts scenario completed successfully");
    }
    
    [Fact]
    public async Task Procedures_ViewAndAdd_ShouldWorkCorrectly()
    {
        Logger.Information("=== SCENARIO 5: Procedures (View and Add) ===");
        
        // Setup: Create and login user
        var userData = await CreateTestUserDirectlyAsync();
        await LoginUserAsync(userData.Email, userData.Password);
        
        // Navigate to procedures
        await Page.GotoAsync($"{Configuration.BaseUrl}/dashboard");
        await Page.ClickAsync("[data-testid='procedures-card']");
        await Page.WaitForURLAsync($"{Configuration.BaseUrl}/procedures");
        
        // Verify empty state
        var emptyStateText = await Page.TextContentAsync("[data-testid='empty-procedures']");
        emptyStateText.Should().Contain("Nie masz jeszcze żadnych procedur");
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/05_Procedures_Empty.png" 
        });
        
        // Add new procedure
        await Page.ClickAsync("button:has-text('Dodaj procedurę')");
        await Page.WaitForSelectorAsync("[data-testid='procedure-form']");
        
        // Fill procedure form
        var procedureData = TestDataBuilderExtensions.BuildMedicalProcedure();
        
        await Page.FillAsync("input[name='date']", procedureData.Date.ToString("yyyy-MM-dd"));
        await Page.FillAsync("input[name='name']", procedureData.Name);
        await Page.SelectOptionAsync("select[name='category']", procedureData.Category);
        await Page.FillAsync("input[name='icdCode']", procedureData.IcdCode);
        await Page.FillAsync("textarea[name='description']", procedureData.Description);
        await Page.CheckAsync("input[name='supervised']");
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/05_Procedure_Form_Filled.png" 
        });
        
        // Submit procedure
        var responsePromise = WaitForApiResponseAsync("/api/procedures", async () =>
        {
            await Page.ClickAsync("button[type='submit']");
        });
        
        var response = await responsePromise;
        response.Status.Should().Be(201);
        
        // Verify procedure appears in list
        await Page.WaitForSelectorAsync("[data-testid='procedure-item']");
        var procedureItem = await Page.TextContentAsync("[data-testid='procedure-item']");
        procedureItem.Should().Contain(procedureData.Name);
        procedureItem.Should().Contain(procedureData.Category);
        
        await Page.ScreenshotAsync(new() 
        { 
            Path = "Reports/Screenshots/05_Procedure_Added.png" 
        });
        
        Logger.Information("Procedures scenario completed successfully");
    }
    
    // Helper methods
    private async Task<(string Email, string Password, string FirstName)> CreateTestUserDirectlyAsync()
    {
        var testData = TestDataBuilderExtensions.BuildPolishMedicalResident();
        var password = "TestPass123!";
        
        // Insert user directly into test database
        await ExecuteSqlAsync($@"
            INSERT INTO ""Users"" 
            (""Email"", ""Password"", ""FirstName"", ""LastName"", ""SmkVersion"", 
             ""Location"", ""University"", ""Year"", ""Phone"", ""RegistrationDate"", ""IsActive"")
            VALUES 
            ('{testData.Email}', '$2a$10$8JqVb7SERQ2HCLKSMbfAkOSr1r2Ot.piVcAVZQQYjQxZX2x1B0XMO', 
             '{testData.FirstName}', '{testData.LastName}', 'new', 
             '{testData.Location}', '{testData.University}', {testData.Year}, 
             '{testData.Phone}', NOW(), true)
        ");
        
        return (testData.Email, password, testData.FirstName);
    }
    
    private async Task LoginUserAsync(string email, string password)
    {
        await Page.GotoAsync($"{Configuration.BaseUrl}/login");
        await Page.FillAsync("input[name='email']", email);
        await Page.FillAsync("input[name='password']", password);
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForURLAsync($"{Configuration.BaseUrl}/dashboard");
    }
    
    private async Task<RegistrationData> RegisterNewUserAsync()
    {
        var testData = TestDataBuilderExtensions.BuildPolishMedicalResident();
        var registrationData = new RegistrationData
        {
            Email = testData.Email,
            Password = "SecurePass123!",
            FirstName = testData.FirstName,
            LastName = testData.LastName
        };
        
        await Page.GotoAsync($"{Configuration.BaseUrl}/register");
        
        // Fill and submit registration form
        await TypeLikeUserAsync("input[name='firstName']", registrationData.FirstName);
        await TypeLikeUserAsync("input[name='lastName']", registrationData.LastName);
        await TypeLikeUserAsync("input[name='email']", registrationData.Email);
        await TypeLikeUserAsync("input[name='password']", registrationData.Password);
        await TypeLikeUserAsync("input[name='confirmPassword']", registrationData.Password);
        await Page.SelectOptionAsync("select[name='smkVersion']", "new");
        await TypeLikeUserAsync("input[name='location']", testData.Location);
        await TypeLikeUserAsync("input[name='university']", testData.University);
        await Page.SelectOptionAsync("select[name='year']", testData.Year.ToString());
        await TypeLikeUserAsync("input[name='phone']", testData.Phone);
        await Page.CheckAsync("input[name='acceptTerms']");
        
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForSelectorAsync("text=Rejestracja zakończona sukcesem");
        
        return registrationData;
    }
    
    private async Task LoginWithRegisteredUserAsync(RegistrationData registrationData)
    {
        await Page.GotoAsync($"{Configuration.BaseUrl}/login");
        await TypeLikeUserAsync("input[name='email']", registrationData.Email);
        await TypeLikeUserAsync("input[name='password']", registrationData.Password);
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForURLAsync($"{Configuration.BaseUrl}/dashboard");
    }
    
    private async Task VerifyDashboardAccessAsync()
    {
        var welcomeText = await Page.TextContentAsync("h1");
        welcomeText.Should().Contain("Witaj");
        
        // Verify main sections are visible
        await Page.WaitForSelectorAsync("[data-testid='medical-shifts-card']");
        await Page.WaitForSelectorAsync("[data-testid='procedures-card']");
    }
    
    private async Task AddMedicalShiftsAsync()
    {
        await Page.ClickAsync("[data-testid='medical-shifts-card']");
        await Page.WaitForURLAsync($"{Configuration.BaseUrl}/medical-shifts");
        
        // Add a shift
        await Page.ClickAsync("button:has-text('Dodaj dyżur')");
        
        var shiftData = new MedicalShiftBuilder()
            .OnDate(DateTime.Today)
            .WithTimes("07:00", "19:00")
            .Build();
        
        await Page.FillAsync("input[name='date']", shiftData.Date.ToString("yyyy-MM-dd"));
        await Page.FillAsync("input[name='startTime']", shiftData.StartTime);
        await Page.FillAsync("input[name='endTime']", shiftData.EndTime);
        await Page.SelectOptionAsync("select[name='type']", shiftData.Type);
        await Page.FillAsync("input[name='place']", shiftData.Place);
        
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForSelectorAsync("[data-testid='shift-item']");
    }
    
    private async Task AddProceduresAsync()
    {
        await Page.GotoAsync($"{Configuration.BaseUrl}/dashboard");
        await Page.ClickAsync("[data-testid='procedures-card']");
        await Page.WaitForURLAsync($"{Configuration.BaseUrl}/procedures");
        
        // Add a procedure
        await Page.ClickAsync("button:has-text('Dodaj procedurę')");
        
        var procedureData = TestDataBuilderExtensions.BuildMedicalProcedure();
        
        await Page.FillAsync("input[name='date']", procedureData.Date.ToString("yyyy-MM-dd"));
        await Page.FillAsync("input[name='name']", procedureData.Name);
        await Page.SelectOptionAsync("select[name='category']", procedureData.Category);
        await Page.FillAsync("input[name='icdCode']", procedureData.IcdCode);
        
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForSelectorAsync("[data-testid='procedure-item']");
    }
}

public class RegistrationData
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
}

/// <summary>
/// Shared test fixture for E2E tests
/// </summary>
public class E2ETestFixture : IDisposable
{
    public E2ETestFixture()
    {
        // Shared setup if needed
    }
    
    public void Dispose()
    {
        // Shared cleanup if needed
    }
}