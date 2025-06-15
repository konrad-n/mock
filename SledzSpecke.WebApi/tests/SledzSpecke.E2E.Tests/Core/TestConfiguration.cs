namespace SledzSpecke.E2E.Tests.Core;

/// <summary>
/// Test configuration following Open/Closed Principle - open for extension, closed for modification
/// </summary>
public class TestConfiguration
{
    public string BaseUrl { get; set; } = "https://sledzspecke.pl";
    public string ApiUrl { get; set; } = "https://api.sledzspecke.pl";
    public BrowserType Browser { get; set; } = BrowserType.Chromium;
    public bool Headless { get; set; } = false;
    public int DefaultTimeout { get; set; } = 30000;
    public bool RecordVideo { get; set; } = true;
    public bool TraceEnabled { get; set; } = true;
    public string VideoPath { get; set; } = "Reports/Videos";
    public string TracePath { get; set; } = "Reports/Traces";
    public string ScreenshotPath { get; set; } = "Reports/Screenshots";
    
    // Test user credentials
    public TestUserConfiguration TestUser { get; set; } = new();
    
    // SMK simulation settings
    public SmkSimulationConfiguration SmkSimulation { get; set; } = new();
}

public class TestUserConfiguration
{
    public string DefaultUsername { get; set; } = "e2e_test_user";
    public string DefaultPassword { get; set; } = "Test123!";
    public string DefaultEmail { get; set; } = "e2e@test.sledzspecke.pl";
    public string DefaultSpecialization { get; set; } = "Anestezjologia i intensywna terapia";
}

public class SmkSimulationConfiguration
{
    public int DelayBetweenActions { get; set; } = 500; // ms
    public bool SimulateRealUserSpeed { get; set; } = true;
    public bool TakeScreenshotsOnKeyActions { get; set; } = true;
    public int MaxRetryAttempts { get; set; } = 3;
}

public enum BrowserType
{
    Chromium,
    Firefox,
    Safari
}