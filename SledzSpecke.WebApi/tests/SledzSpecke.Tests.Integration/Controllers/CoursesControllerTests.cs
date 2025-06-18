using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Tests.Integration.Common;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace SledzSpecke.Tests.Integration.Controllers;

public class CoursesControllerTests : IntegrationTestBase
{
    private readonly HttpClient _authenticatedClient;

    public CoursesControllerTests(SledzSpeckeApiFactory factory) : base(factory)
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
    public async Task GetCourses_ShouldReturnAllCourses()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _authenticatedClient.GetAsync("/api/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var courses = await response.Content.ReadFromJsonAsync<List<CourseDto>>();
        courses.Should().NotBeNull();
        courses!.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCourse_WithValidId_ShouldReturnCourse()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _authenticatedClient.GetAsync("/api/courses/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var course = await response.Content.ReadFromJsonAsync<CourseDto>();
        course.Should().NotBeNull();
        course!.CourseName.Should().Be("Test Course 1");
    }

    [Fact]
    public async Task GetCourse_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _authenticatedClient.GetAsync("/api/courses/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateCourse_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        await SeedTestData();
        
        var command = new CreateCourse(
            SpecializationId: 1,
            CourseType: "Mandatory",
            CourseName: "New Course",
            InstitutionName: "Test Institution",
            CompletionDate: DateTime.UtcNow.AddDays(-10),
            CourseNumber: "COURSE001",
            CertificateNumber: "CERT001",
            ModuleId: 1);

        // Act
        var response = await _authenticatedClient.PostAsJsonAsync("/api/courses", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var courseId = await response.Content.ReadFromJsonAsync<int>();
        courseId.Should().BeGreaterThan(0);
        
        // Verify in database
        var course = await DbContext.Courses.FindAsync(new CourseId(courseId));
        course.Should().NotBeNull();
        course!.CourseName.Should().Be("New Course");
        course.HasCertificate.Should().BeTrue();
        course.CertificateNumber.Should().Be("CERT001");
    }

    [Fact]
    public async Task CreateCourse_WithFutureDate_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedTestData();
        
        var command = new CreateCourse(
            SpecializationId: 1,
            ModuleId: 1,
            CourseType: "Mandatory",
            CourseName: "New Course",
            CourseNumber: null,
            InstitutionName: "Test Institution",
            CompletionDate: DateTime.UtcNow.AddDays(10), // Future date
            HasCertificate: false,
            CertificateNumber: null);

        // Act
        var response = await _authenticatedClient.PostAsJsonAsync("/api/courses", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Completion date cannot be in the future");
    }

    [Fact]
    public async Task CreateCourse_WithInvalidSpecialization_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedTestData();
        
        var command = new CreateCourse(
            SpecializationId: 999, // Non-existent
            ModuleId: 1,
            CourseType: "Mandatory",
            CourseName: "New Course",
            CourseNumber: null,
            InstitutionName: "Test Institution",
            CompletionDate: DateTime.UtcNow.AddDays(-10),
            HasCertificate: false,
            CertificateNumber: null);

        // Act
        var response = await _authenticatedClient.PostAsJsonAsync("/api/courses", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Specialization with ID 999 not found");
    }

    [Fact]
    public async Task UpdateCourse_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await SeedTestData();
        
        var command = new UpdateCourse(
            CourseId: 1,
            CourseName: "Updated Course Name",
            CourseNumber: "UPDATED001",
            InstitutionName: "Updated Institution",
            CompletionDate: DateTime.UtcNow.AddDays(-5),
            CertificateNumber: "UPDATED-CERT");

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/api/courses/1", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify in database
        var course = await DbContext.Courses.FindAsync(new CourseId(1));
        course.Should().NotBeNull();
        course!.CourseName.Should().Be("Updated Course Name");
        course.CourseNumber.Should().Be("UPDATED001");
        course.InstitutionName.Should().Be("Updated Institution");
        course.HasCertificate.Should().BeTrue();
        course.CertificateNumber.Should().Be("UPDATED-CERT");
    }

    [Fact]
    public async Task UpdateCourse_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await SeedTestData();
        
        var command = new UpdateCourse(
            CourseId: 999,
            CourseName: "Updated Course Name",
            CourseNumber: null,
            InstitutionName: "Updated Institution",
            CompletionDate: DateTime.UtcNow.AddDays(-5),
            HasCertificate: false,
            CertificateNumber: null);

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/api/courses/999", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCourse_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _authenticatedClient.DeleteAsync("/api/courses/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify in database
        var course = await DbContext.Courses.FindAsync(new CourseId(2));
        course.Should().BeNull();
    }

    [Fact]
    public async Task DeleteCourse_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _authenticatedClient.DeleteAsync("/api/courses/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CompleteCourse_WithValidId_ShouldReturnOk()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _authenticatedClient.PostAsync("/api/courses/1/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Note: The current Course entity doesn't have a completion status,
        // so this endpoint might need to be implemented differently
    }

    [Fact]
    public async Task CompleteCourse_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _authenticatedClient.PostAsync("/api/courses/999/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task SeedTestData()
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

        var module = new Module
        {
            Id = new ModuleId(1),
            Name = "Test Module",
            SpecializationId = new SpecializationId(1),
            OrderIndex = 1,
            IsActive = true
        };
        
        await DbContext.Modules.AddAsync(module);

        var course1 = Course.Create(
            new CourseId(1),
            new SpecializationId(1),
            CourseType.Specialization,
            "Test Course 1",
            "Test Institution",
            DateTime.UtcNow.AddDays(-30));
        course1.AssignToModule(new ModuleId(1));
        
        var course2 = Course.Create(
            new CourseId(2),
            new SpecializationId(1),
            CourseType.Improvement,
            "Test Course 2",
            "Test Institution 2",
            DateTime.UtcNow.AddDays(-20));
        
        await DbContext.Courses.AddAsync(course1);
        await DbContext.Courses.AddAsync(course2);
        
        await DbContext.SaveChangesAsync();
    }
}