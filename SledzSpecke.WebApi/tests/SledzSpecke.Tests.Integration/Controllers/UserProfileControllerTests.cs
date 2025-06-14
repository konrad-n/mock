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

public class UserProfileControllerTests : IntegrationTestBase
{
    private readonly HttpClient _authenticatedClient;

    public UserProfileControllerTests()
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
    public async Task GetProfile_WithAuthentication_ShouldReturnUserProfile()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _authenticatedClient.GetAsync("/api/users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        profile.Should().NotBeNull();
        profile!.Email.Should().Be("test@example.com");
        profile.FullName.Should().Be("Test User");
        profile.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetProfile_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateProfile_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await SeedTestData();
        
        var command = new UpdateUserProfile(
            FullName: "Updated Name",
            Email: "updated@example.com",
            PhoneNumber: "+48123456789",
            DateOfBirth: new DateTime(1990, 1, 1),
            Bio: "Updated bio");

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/api/users/profile", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify in database
        var user = await DbContext.Users.FindAsync(new UserId(1));
        user.Should().NotBeNull();
        user!.FullName.Value.Should().Be("Updated Name");
        user.Email.Value.Should().Be("updated@example.com");
        user.PhoneNumber?.Value.Should().Be("+48123456789");
        user.DateOfBirth.Should().Be(new DateTime(1990, 1, 1));
        user.Bio.Value.Should().Be("Updated bio");
    }

    [Fact]
    public async Task UpdateProfile_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedTestData();
        
        var command = new UpdateUserProfile(
            FullName: "Updated Name",
            Email: "invalid-email",
            PhoneNumber: null,
            DateOfBirth: null,
            Bio: null);

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/api/users/profile", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProfile_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var command = new UpdateUserProfile(
            FullName: "Updated Name",
            Email: "updated@example.com",
            PhoneNumber: null,
            DateOfBirth: null,
            Bio: null);

        // Act
        var response = await Client.PutAsJsonAsync("/api/users/profile", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_WithValidCurrentPassword_ShouldReturnOk()
    {
        // Arrange
        await SeedTestData();
        
        var command = new ChangePassword(
            CurrentPassword: "TestPassword123!",
            NewPassword: "NewSecurePassword123!");

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/api/users/change-password", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangePassword_WithInvalidCurrentPassword_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedTestData();
        
        var command = new ChangePassword(
            CurrentPassword: "WrongPassword!",
            NewPassword: "NewSecurePassword123!");

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/api/users/change-password", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Current password is incorrect");
    }

    [Fact]
    public async Task ChangePassword_WithWeakNewPassword_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedTestData();
        
        var command = new ChangePassword(
            CurrentPassword: "TestPassword123!",
            NewPassword: "weak");

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/api/users/change-password", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePassword_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var command = new ChangePassword(
            CurrentPassword: "TestPassword123!",
            NewPassword: "NewSecurePassword123!");

        // Act
        var response = await Client.PutAsJsonAsync("/api/users/change-password", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdatePreferences_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await SeedTestData();
        
        var command = new UpdateUserPreferences(
            Language: "pl",
            Theme: "dark",
            NotificationsEnabled: false,
            EmailNotificationsEnabled: true);

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/api/users/preferences", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify in database
        var user = await DbContext.Users.FindAsync(new UserId(1));
        user.Should().NotBeNull();
        user!.PreferredLanguage.Value.Should().Be("pl");
        user.PreferredTheme.Value.Should().Be("dark");
        user.NotificationsEnabled.Should().BeFalse();
        user.EmailNotificationsEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task UpdatePreferences_WithInvalidTheme_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedTestData();
        
        var command = new UpdateUserPreferences(
            Language: "en",
            Theme: "invalid-theme",
            NotificationsEnabled: true,
            EmailNotificationsEnabled: true);

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/api/users/preferences", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePreferences_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var command = new UpdateUserPreferences(
            Language: "en",
            Theme: "dark",
            NotificationsEnabled: true,
            EmailNotificationsEnabled: true);

        // Act
        var response = await Client.PutAsJsonAsync("/api/users/preferences", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
        
        var user = User.Create(
            new UserId(1),
            new Email("test@example.com"),
            new Username("testuser"),
            new Password("TestPassword123!"), // In real app this would be hashed
            new FullName("Test User"),
            new SmkVersion("new"),
            new SpecializationId(1),
            DateTime.UtcNow);
            
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
    }
}