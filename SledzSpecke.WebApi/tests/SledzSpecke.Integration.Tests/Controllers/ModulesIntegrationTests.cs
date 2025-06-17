using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SledzSpecke.Infrastructure.DAL;
using SledzSpecke.Core.Entities;
using Xunit;

namespace SledzSpecke.Integration.Tests.Controllers;

public class ModulesIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ModulesIntegrationTests(WebApplicationFactory<Program> factory)
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
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetTestToken());
    }

    [Fact]
    public async Task CreateInternship_ValidRequest_ReturnsOk()
    {
        // Arrange
        await SeedTestDataAsync();
        
        var request = new CreateModuleInternshipRequest
        {
            SpecializationId = 1,
            Name = "Cardiology Internship",
            InstitutionName = "City Hospital",
            DepartmentName = "Cardiology",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(3),
            PlannedWeeks = 12,
            PlannedDays = 60
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/modules/1/internships", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateInternshipResponse>();
        Assert.NotNull(result);
        Assert.True(result.InternshipId > 0);
        Assert.Equal("Internship created successfully", result.Message);
    }

    [Fact]
    public async Task AddProcedure_ValidRequest_ReturnsOk()
    {
        // Arrange
        await SeedTestDataAsync();
        
        var request = new AddModuleProcedureRequest
        {
            InternshipId = 1,
            Date = DateTime.UtcNow,
            Code = "PROC001",
            Name = "ECG",
            Location = "Cardiology Department",
            ExecutionType = "CodeA",
            SupervisorName = "Dr. Smith"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/modules/1/procedures", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AddProcedureResponse>();
        Assert.NotNull(result);
        Assert.True(result.ProcedureId > 0);
        Assert.Contains("successfully", result.Message);
    }

    [Fact]
    public async Task AddMedicalShift_ValidRequest_ReturnsOk()
    {
        // Arrange
        await SeedTestDataAsync();
        
        var request = new AddModuleMedicalShiftRequest
        {
            InternshipId = 1,
            Date = DateTime.UtcNow,
            Hours = 8,
            Minutes = 30,
            Location = "Emergency Department"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/modules/1/medical-shifts", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AddMedicalShiftResponse>();
        Assert.NotNull(result);
        Assert.Contains("successfully", result.Message);
    }

    [Fact]
    public async Task AddMedicalShift_MissingInternshipId_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestDataAsync();
        
        var request = new AddModuleMedicalShiftRequest
        {
            InternshipId = null, // Missing required field
            Date = DateTime.UtcNow,
            Hours = 8,
            Minutes = 30
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/modules/1/medical-shifts", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("InternshipId is required", content);
    }

    [Fact]
    public async Task CreateCourse_ValidRequest_ReturnsOk()
    {
        // Arrange
        await SeedTestDataAsync();
        
        var request = new CreateModuleCourseRequest
        {
            CourseName = "Advanced Cardiology",
            CourseNumber = "CARD-ADV-2024",
            InstitutionName = "Medical University",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            DurationDays = 5,
            DurationHours = 40,
            CmkpCertificateNumber = "CMKP/2024/12345"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/modules/1/courses", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateCourseResponse>();
        Assert.NotNull(result);
        Assert.True(result.CourseId > 0);
        Assert.Contains("successfully", result.Message);
    }

    [Fact]
    public async Task AddSelfEducation_ValidRequest_ReturnsOk()
    {
        // Arrange
        await SeedTestDataAsync();
        
        var request = new AddModuleSelfEducationRequest
        {
            Type = "Conference",
            Description = "International Cardiology Conference 2024",
            Date = DateTime.UtcNow,
            Hours = 16
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/modules/1/self-education", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AddSelfEducationResponse>();
        Assert.NotNull(result);
        Assert.Contains("successfully", result.Message);
    }

    [Fact]
    public async Task AddAdditionalDays_ValidRequest_ReturnsOk()
    {
        // Arrange
        await SeedTestDataAsync();
        
        var request = new AddModuleAdditionalDaysRequest
        {
            InternshipId = 1,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2),
            NumberOfDays = 3,
            Purpose = "Medical Conference",
            EventName = "European Cardiology Summit"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/modules/1/additional-days", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AddAdditionalDaysResponse>();
        Assert.NotNull(result);
        Assert.True(result.AdditionalDaysId > 0);
        Assert.Contains("successfully", result.Message);
    }

    [Fact]
    public async Task GetModuleProgress_ValidId_ReturnsProgress()
    {
        // Arrange
        await SeedTestDataAsync();
        var moduleId = 1;

        // Act
        var response = await _client.GetAsync($"/api/modules/{moduleId}/progress");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);
        // The actual DTO structure would depend on SpecializationStatisticsDto
    }

    [Fact]
    public async Task GetModule_ValidId_ReturnsModule()
    {
        // Arrange
        await SeedTestDataAsync();
        var moduleId = 1;

        // Act
        var response = await _client.GetAsync($"/api/modules/{moduleId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task SwitchModule_ValidRequest_ReturnsOk()
    {
        // Arrange
        await SeedTestDataAsync();
        
        var request = new SwitchModuleRequest
        {
            SpecializationId = 1,
            ModuleId = 2
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/modules/switch", request);

        // Assert
        // The actual response depends on the handler implementation
        Assert.True(response.StatusCode == HttpStatusCode.OK || 
                   response.StatusCode == HttpStatusCode.NoContent);
    }

    private async Task SeedTestDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SledzSpeckeDbContext>();
        
        // Ensure database is created
        await dbContext.Database.EnsureCreatedAsync();

        // Add test user
        if (!await dbContext.Users.AnyAsync())
        {
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hashed_password",
                IsActive = true
            };
            dbContext.Users.Add(user);
        }

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

        // Add test modules
        if (!await dbContext.Modules.AnyAsync())
        {
            var module1 = new Module
            {
                Id = 1,
                SpecializationId = 1,
                Name = "Basic Cardiology",
                ModuleType = "basic",
                StartDate = DateTime.UtcNow.AddMonths(-6),
                EndDate = DateTime.UtcNow.AddMonths(6),
                IsCompleted = false
            };
            
            var module2 = new Module
            {
                Id = 2,
                SpecializationId = 1,
                Name = "Advanced Cardiology",
                ModuleType = "specialized",
                StartDate = DateTime.UtcNow.AddMonths(6),
                EndDate = DateTime.UtcNow.AddMonths(18),
                IsCompleted = false
            };
            
            dbContext.Modules.Add(module1);
            dbContext.Modules.Add(module2);
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
        return "test-jwt-token";
    }
}

// Request/Response DTOs
public class CreateModuleInternshipRequest
{
    public int SpecializationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PlannedWeeks { get; set; }
    public int PlannedDays { get; set; }
}

public class AddModuleMedicalShiftRequest
{
    public int? InternshipId { get; set; }
    public DateTime Date { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public string? Location { get; set; }
}

public class AddModuleProcedureRequest  
{
    public int InternshipId { get; set; }
    public DateTime Date { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ExecutionType { get; set; } = string.Empty;
    public string SupervisorName { get; set; } = string.Empty;
}

public class CreateModuleCourseRequest
{
    public string CourseName { get; set; } = string.Empty;
    public string CourseNumber { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public int DurationHours { get; set; }
    public string? CmkpCertificateNumber { get; set; }
}

public class AddModuleSelfEducationRequest
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Hours { get; set; }
}

public class AddModuleAdditionalDaysRequest
{
    public int InternshipId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfDays { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? EventName { get; set; }
}

public class SwitchModuleRequest
{
    public int SpecializationId { get; set; }
    public int ModuleId { get; set; }
}

// Response DTOs
public class CreateInternshipResponse
{
    public int InternshipId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AddMedicalShiftResponse
{
    public int ShiftId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AddProcedureResponse
{
    public int ProcedureId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class CreateCourseResponse
{
    public int CourseId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AddSelfEducationResponse
{
    public int SelfEducationId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AddAdditionalDaysResponse
{
    public int AdditionalDaysId { get; set; }
    public string Message { get; set; } = string.Empty;
}