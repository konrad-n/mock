using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Entities;
using SledzSpecke.Infrastructure.DAL;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace SledzSpecke.Integration.Tests.Controllers;

public class AdditionalSelfEducationDaysIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AdditionalSelfEducationDaysIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<SledzSpeckeDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Use in-memory database for testing
                services.AddDbContext<SledzSpeckeDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });
            });
        });
        
        _client = _factory.CreateClient();
        
        // Setup authentication for tests
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetTestToken());
    }

    [Fact]
    public async Task Post_AdditionalSelfEducationDays_ReturnsCreatedResult()
    {
        // Arrange
        await SeedTestDataAsync();
        
        var dto = new CreateAdditionalSelfEducationDaysDto
        {
            SpecializationId = 1,
            Year = 2024,
            DaysUsed = 3,
            Comment = "Conference attendance"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/additional-self-education-days", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<int>();
        Assert.True(result > 0);
    }

    [Fact]
    public async Task Get_AdditionalSelfEducationDaysByModule_ReturnsOkResult()
    {
        // Arrange
        await SeedTestDataAsync();
        var moduleId = 1;

        // Act
        var response = await _client.GetAsync($"/api/additional-self-education-days/module/{moduleId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<AdditionalSelfEducationDaysDto>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Post_AdditionalSelfEducationDays_ExceedingLimit_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestDataAsync();
        
        // First add 5 days
        var firstDto = new CreateAdditionalSelfEducationDaysDto
        {
            SpecializationId = 1,
            Year = 2024,
            DaysUsed = 5,
            Comment = "Initial days"
        };
        var firstResponse = await _client.PostAsJsonAsync("/api/additional-self-education-days", firstDto);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        // Try to add 3 more days (exceeding the 6-day limit)
        var secondDto = new CreateAdditionalSelfEducationDaysDto
        {
            SpecializationId = 1,
            Year = 2024,
            DaysUsed = 3,
            Comment = "Exceeding limit"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/additional-self-education-days", secondDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Contains("exceed", problemDetails.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Get_AdditionalSelfEducationDaysById_ReturnsCorrectData()
    {
        // Arrange
        await SeedTestDataAsync();
        
        // First create a record
        var dto = new CreateAdditionalSelfEducationDaysDto
        {
            SpecializationId = 1,
            Year = 2024,
            DaysUsed = 2,
            Comment = "Test days"
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/additional-self-education-days", dto);
        var createdId = await createResponse.Content.ReadFromJsonAsync<int>();

        // Act
        var response = await _client.GetAsync($"/api/additional-self-education-days/{createdId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AdditionalSelfEducationDaysDto>();
        Assert.NotNull(result);
        Assert.Equal(createdId, result.Id);
        Assert.Equal(2, result.NumberOfDays);
    }

    [Fact]
    public async Task Get_AdditionalSelfEducationDaysById_NotFound_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/additional-self-education-days/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_AdditionalSelfEducationDays_UpdatesSuccessfully()
    {
        // Arrange
        await SeedTestDataAsync();
        
        // First create a record
        var createDto = new CreateAdditionalSelfEducationDaysDto
        {
            SpecializationId = 1,
            Year = 2024,
            DaysUsed = 2,
            Comment = "Initial comment"
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/additional-self-education-days", createDto);
        var createdId = await createResponse.Content.ReadFromJsonAsync<int>();

        var updateDto = new UpdateAdditionalSelfEducationDaysDto
        {
            DaysUsed = 3,
            Comment = "Updated comment"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/additional-self-education-days/{createdId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AdditionalSelfEducationDays_DeletesSuccessfully()
    {
        // Arrange
        await SeedTestDataAsync();
        
        // First create a record
        var dto = new CreateAdditionalSelfEducationDaysDto
        {
            SpecializationId = 1,
            Year = 2024,
            DaysUsed = 1,
            Comment = "To be deleted"
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/additional-self-education-days", dto);
        var createdId = await createResponse.Content.ReadFromJsonAsync<int>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/additional-self-education-days/{createdId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify it's deleted
        var getResponse = await _client.GetAsync($"/api/additional-self-education-days/{createdId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task SeedTestDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SledzSpeckeDbContext>();
        
        // Ensure database is created
        await dbContext.Database.EnsureCreatedAsync();

        // Add test specialization
        if (!await dbContext.Specializations.AnyAsync())
        {
            var specialization = new Specialization
            {
                Id = 1,
                Name = "Cardiology",
                SmkCode = "CARD",
                SmkVersion = "new",
                TotalMonths = 60,
                IsActive = true
            };
            dbContext.Specializations.Add(specialization);
        }

        // Add test module
        if (!await dbContext.Modules.AnyAsync())
        {
            var module = new Module
            {
                Id = 1,
                SpecializationId = 1,
                Name = "Basic Cardiology",
                ModuleType = "basic",
                StartDate = DateTime.UtcNow.AddMonths(-6),
                EndDate = DateTime.UtcNow.AddMonths(6),
                IsCompleted = false
            };
            dbContext.Modules.Add(module);
        }

        // Add test internship
        if (!await dbContext.Internships.AnyAsync())
        {
            var internship = new Internship
            {
                Id = 1,
                ModuleId = 1,
                Name = "Test Internship",
                InstitutionName = "Test Hospital",
                DepartmentName = "Cardiology",
                StartDate = DateTime.UtcNow.AddMonths(-3),
                EndDate = DateTime.UtcNow.AddMonths(3),
                Status = Core.Enums.InternshipStatus.InProgress
            };
            dbContext.Internships.Add(internship);
        }

        await dbContext.SaveChangesAsync();
    }

    private string GetTestToken()
    {
        // In a real scenario, generate a valid JWT token for testing
        // For now, return a dummy token
        return "test-jwt-token";
    }
}

// DTOs for testing
public class ProblemDetails
{
    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public string Detail { get; set; }
    public string Instance { get; set; }
}