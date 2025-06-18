using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Tests.Integration.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace SledzSpecke.Tests.Integration.Controllers;

public class MedicalShiftsControllerTests : IntegrationTestBase
{
    private readonly HttpClient _authenticatedClient;
    private readonly ISpecializationRepository _specializationRepository;

    public MedicalShiftsControllerTests(SledzSpeckeApiFactory factory) : base(factory)
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
            
        _specializationRepository = Scope.ServiceProvider.GetRequiredService<ISpecializationRepository>();
    }

    [Fact]
    public async Task CreateMedicalShift_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization();
        await _specializationRepository.AddAsync(specialization);
        await DbContext.SaveChangesAsync();

        // Create an internship first
        var internship = TestDataFactory.CreateInternship(specialization.Id.Value);
        await DbContext.Internships.AddAsync(internship);
        await DbContext.SaveChangesAsync();
        
        var command = new AddMedicalShift(
            InternshipId: internship.Id.Value,
            Date: DateTime.Today.AddDays(1),
            Hours: 8);

        // Act
        var response = await _authenticatedClient.PostAsJsonAsync("/medical-shifts", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateMedicalShift_WithPastDate_ShouldReturnBadRequest()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization();
        await _specializationRepository.AddAsync(specialization);
        await DbContext.SaveChangesAsync();

        // Create an internship first
        var internship = TestDataFactory.CreateInternship(specialization.Id.Value);
        await DbContext.Internships.AddAsync(internship);
        await DbContext.SaveChangesAsync();
        
        var command = new AddMedicalShift(
            InternshipId: internship.Id.Value,
            Date: DateTime.Today.AddDays(-1),
            Hours: 8,
            Minutes: 0,
            Location: "Test Hospital",
            Year: 3);

        // Act
        var response = await _authenticatedClient.PostAsJsonAsync("/medical-shifts", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Date");
    }

    [Fact]
    public async Task CreateMedicalShift_WithInvalidTimeRange_ShouldReturnBadRequest()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization();
        await _specializationRepository.AddAsync(specialization);
        await DbContext.SaveChangesAsync();

        // Create an internship first
        var internship = TestDataFactory.CreateInternship(specialization.Id.Value);
        await DbContext.Internships.AddAsync(internship);
        await DbContext.SaveChangesAsync();
        
        var command = new AddMedicalShift(
            InternshipId: internship.Id.Value,
            Date: DateTime.Today.AddDays(1),
            Hours: -8, // This should be invalid (negative hours)
            Minutes: 0,
            Location: "Test Hospital",
            Year: 3);

        // Act
        var response = await _authenticatedClient.PostAsJsonAsync("/medical-shifts", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Hours");
    }

    [Fact]
    public async Task CreateMedicalShift_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var command = new AddMedicalShift(
            InternshipId: 1,
            Date: DateTime.Today.AddDays(1),
            Hours: 8,
            Minutes: 0,
            Location: "Test Hospital",
            Year: 3);

        // Act
        var response = await Client.PostAsJsonAsync("/medical-shifts", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateMedicalShift_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization();
        await _specializationRepository.AddAsync(specialization);
        
        var shift = TestDataFactory.CreateMedicalShift(
            specialization.Id.Value,
            DateTime.Today.AddDays(5));
        
        var repository = Scope.ServiceProvider.GetRequiredService<IMedicalShiftRepository>();
        await repository.AddAsync(shift);
        await DbContext.SaveChangesAsync();

        var command = new UpdateMedicalShift(
            ShiftId: shift.Id.Value,
            Date: DateTime.Today.AddDays(7),
            Hours: 8,
            Minutes: 0,
            Location: "New Hospital");

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync($"/medical-shifts/{shift.Id.Value}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateMedicalShift_WithNonExistentId_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new UpdateMedicalShift(
            ShiftId: 999999,
            Date: DateTime.Today.AddDays(1),
            Hours: 8,
            Minutes: 0,
            Location: "Hospital");

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/medical-shifts/999999", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("not found");
    }

    [Fact]
    public async Task GetMedicalShift_WithExistingId_ShouldReturnShift()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization();
        await _specializationRepository.AddAsync(specialization);
        
        var shift = TestDataFactory.CreateMedicalShift(
            specialization.Id.Value,
            DateTime.Today.AddDays(5));
        
        var repository = Scope.ServiceProvider.GetRequiredService<IMedicalShiftRepository>();
        await repository.AddAsync(shift);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await _authenticatedClient.GetAsync($"/medical-shifts/{shift.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(shift.Location.Value);
        // Description is not exposed in the API response
    }

    [Fact]
    public async Task GetMedicalShift_WithNonExistentId_ShouldReturnNotFound()
    {
        // Act
        var response = await _authenticatedClient.GetAsync("/medical-shifts/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteMedicalShift_WithExistingId_ShouldReturnOk()
    {
        // Arrange
        var specialization = TestDataFactory.CreateSpecialization();
        await _specializationRepository.AddAsync(specialization);
        
        var shift = TestDataFactory.CreateMedicalShift(
            specialization.Id.Value,
            DateTime.Today.AddDays(5));
        
        var repository = Scope.ServiceProvider.GetRequiredService<IMedicalShiftRepository>();
        await repository.AddAsync(shift);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await _authenticatedClient.DeleteAsync($"/medical-shifts/{shift.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify deletion
        var getResponse = await _authenticatedClient.GetAsync($"/medical-shifts/{shift.Id.Value}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}