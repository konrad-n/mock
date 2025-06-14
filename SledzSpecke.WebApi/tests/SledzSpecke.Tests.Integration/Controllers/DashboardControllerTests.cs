using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Tests.Integration.Common;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace SledzSpecke.Tests.Integration.Controllers;

public class DashboardControllerTests : IntegrationTestBase
{
    private readonly HttpClient _authenticatedClient;

    public DashboardControllerTests()
    {
        _authenticatedClient = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.AuthenticationScheme, options => { });
            });
        }).CreateClient();
        
        _authenticatedClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
    }

    [Fact]
    public async Task GetDashboardOverview_WithAuthentication_ShouldReturnOverview()
    {
        // Arrange
        await SeedCompleteTestData();

        // Act
        var response = await _authenticatedClient.GetAsync("/api/dashboard/overview");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var overview = await response.Content.ReadFromJsonAsync<DashboardOverviewDto>();
        overview.Should().NotBeNull();
        overview!.TotalCourses.Should().Be(2);
        overview.TotalProcedures.Should().Be(3);
        overview.TotalInternships.Should().Be(1);
        overview.TotalShiftHours.Should().BeGreaterThan(0);
        overview.CompletedCourses.Should().Be(1);
        overview.CurrentYear.Should().Be(1);
    }

    [Fact]
    public async Task GetDashboardOverview_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/dashboard/overview");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProgress_WithValidSpecialization_ShouldReturnProgress()
    {
        // Arrange
        await SeedCompleteTestData();

        // Act
        var response = await _authenticatedClient.GetAsync("/api/dashboard/progress/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var progress = await response.Content.ReadFromJsonAsync<ProgressDto>();
        progress.Should().NotBeNull();
        progress!.CourseProgress.Should().NotBeNull();
        progress.ProcedureProgress.Should().NotBeNull();
        progress.InternshipProgress.Should().NotBeNull();
        progress.ShiftProgress.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProgress_WithInvalidSpecialization_ShouldReturnNotFound()
    {
        // Arrange
        await SeedTestUser();

        // Act
        var response = await _authenticatedClient.GetAsync("/api/dashboard/progress/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProgress_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/dashboard/progress/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetStatistics_WithValidSpecialization_ShouldReturnStatistics()
    {
        // Arrange
        await SeedCompleteTestData();

        // Act
        var response = await _authenticatedClient.GetAsync("/api/dashboard/statistics/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var statistics = await response.Content.ReadFromJsonAsync<StatisticsDto>();
        statistics.Should().NotBeNull();
        statistics!.CoursesByType.Should().NotBeEmpty();
        statistics.ProceduresByCategory.Should().NotBeEmpty();
        statistics.ShiftsByMonth.Should().NotBeEmpty();
        statistics.AverageShiftDuration.Should().BeGreaterThan(0);
        statistics.TotalCreditHours.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetStatistics_WithInvalidSpecialization_ShouldReturnNotFound()
    {
        // Arrange
        await SeedTestUser();

        // Act
        var response = await _authenticatedClient.GetAsync("/api/dashboard/statistics/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetStatistics_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/dashboard/statistics/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task SeedTestUser()
    {
        var specialization = new Specialization
        {
            Id = new SpecializationId(1),
            Name = "Test Specialization",
            Code = "TST",
            Years = 5,
            IsActive = true
        };
        
        await DbContext.Specializations.AddAsync(specialization);
        
        var user = User.Create(
            new UserId(TestAuthHandler.TestUserId),
            new Email(TestAuthHandler.TestUserEmail),
            new Username("testuser"),
            new Password("TestPassword123!"),
            new FullName("Test User"),
            new SmkVersion("new"),
            new SpecializationId(1),
            DateTime.UtcNow);
            
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
    }

    private async Task SeedCompleteTestData()
    {
        await SeedTestUser();

        // Add modules
        var module1 = new Module
        {
            Id = new ModuleId(1),
            Name = "Module 1",
            SpecializationId = new SpecializationId(1),
            OrderIndex = 1,
            IsActive = true
        };
        
        var module2 = new Module
        {
            Id = new ModuleId(2),
            Name = "Module 2",
            SpecializationId = new SpecializationId(1),
            OrderIndex = 2,
            IsActive = true
        };
        
        await DbContext.Modules.AddAsync(module1);
        await DbContext.Modules.AddAsync(module2);

        // Add courses
        var course1 = Course.Create(
            new CourseId(1),
            new SpecializationId(1),
            CourseType.Mandatory,
            "Test Course 1",
            "Test Institution",
            DateTime.UtcNow.AddDays(-30));
        course1.AssignToModule(new ModuleId(1));
        
        var course2 = Course.Create(
            new CourseId(2),
            new SpecializationId(1),
            CourseType.Optional,
            "Test Course 2",
            "Test Institution",
            DateTime.UtcNow.AddDays(-20));
        course2.AssignToModule(new ModuleId(1));
        course2.SetCertificate("CERT123");
        
        await DbContext.Courses.AddAsync(course1);
        await DbContext.Courses.AddAsync(course2);

        // Add internship
        var internship = Internship.Create(
            new InternshipId(1),
            new SpecializationId(1),
            "Test Hospital",
            "Cardiology",
            DateTime.UtcNow.AddDays(-60),
            DateTime.UtcNow.AddDays(-30));
        internship.AssignToModule(new ModuleId(1));
        
        await DbContext.Internships.AddAsync(internship);

        // Add medical shifts
        var shift1 = MedicalShift.Create(
            new MedicalShiftId(1),
            new InternshipId(1),
            DateTime.UtcNow.AddDays(-50),
            8, 30,
            "Emergency Room",
            1);
            
        var shift2 = MedicalShift.Create(
            new MedicalShiftId(2),
            new InternshipId(1),
            DateTime.UtcNow.AddDays(-45),
            12, 0,
            "ICU",
            1);
            
        await DbContext.MedicalShifts.AddAsync(shift1);
        await DbContext.MedicalShifts.AddAsync(shift2);

        // Add procedures
        var procedure1 = Procedure.Create(
            new ProcedureId(1),
            new InternshipId(1),
            new UserId(TestAuthHandler.TestUserId),
            "ABC123",
            "Test Procedure 1",
            ProcedureType.Primary,
            DateTime.UtcNow.AddDays(-48),
            "Test Patient");
            
        var procedure2 = Procedure.Create(
            new ProcedureId(2),
            new InternshipId(1),
            new UserId(TestAuthHandler.TestUserId),
            "DEF456",
            "Test Procedure 2",
            ProcedureType.Assistant,
            DateTime.UtcNow.AddDays(-47),
            "Test Patient 2");
            
        var procedure3 = Procedure.Create(
            new ProcedureId(3),
            new InternshipId(1),
            new UserId(TestAuthHandler.TestUserId),
            "GHI789",
            "Test Procedure 3",
            ProcedureType.Primary,
            DateTime.UtcNow.AddDays(-46),
            "Test Patient 3");
            
        await DbContext.Procedures.AddAsync(procedure1);
        await DbContext.Procedures.AddAsync(procedure2);
        await DbContext.Procedures.AddAsync(procedure3);

        await DbContext.SaveChangesAsync();
    }
}