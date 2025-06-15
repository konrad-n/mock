using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace SledzSpecke.E2E.Tests.Scenarios;

/// <summary>
/// API-only E2E tests that don't require frontend
/// These tests can run in CI/CD without browser automation
/// </summary>
public class ApiOnlyScenarios : IClassFixture<ApiTestFixture>
{
    private readonly ApiTestFixture _fixture;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public ApiOnlyScenarios(ApiTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _client = fixture.Client;
        _output = output;
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnOk()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/health");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _output.WriteLine($"Health check passed: {response.StatusCode}");
    }

    [Fact]
    public async Task SwaggerUI_ShouldBeAccessible()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/swagger/index.html");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("swagger-ui");
        _output.WriteLine("Swagger UI is accessible");
    }

    [Fact]
    public async Task UserRegistration_ViaAPI_ShouldSucceed()
    {
        // Arrange
        var registrationData = new
        {
            username = $"test_user_{Guid.NewGuid():N}",
            email = $"test_{Guid.NewGuid():N}@example.com",
            password = "Test123!",
            specialization = "Anestezjologia i intensywna terapia",
            smkVersion = "new",
            year = 3
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/register", registrationData);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        _output.WriteLine($"User registered successfully: {registrationData.username}");
    }

    [Fact]
    public async Task SpecializationTemplates_ShouldBeAccessible()
    {
        // Act
        var response = await _client.GetAsync("/api/specializations/templates");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var templates = await response.Content.ReadFromJsonAsync<List<dynamic>>();
        templates.Should().NotBeEmpty();
        _output.WriteLine($"Found {templates?.Count} specialization templates");
    }
}

/// <summary>
/// Test fixture for API-only tests
/// </summary>
public class ApiTestFixture : IDisposable
{
    public HttpClient Client { get; }
    private readonly IConfiguration _configuration;

    public ApiTestFixture()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var apiUrl = _configuration["E2ETests:ApiUrl"] ?? "http://localhost:5000";
        Client = new HttpClient
        {
            BaseAddress = new Uri(apiUrl)
        };
    }

    public void Dispose()
    {
        Client?.Dispose();
    }
}