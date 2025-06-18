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

    public UserProfileControllerTests(SledzSpeckeApiFactory factory) : base(factory)
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
        profile.FirstName.Should().Be("Test");
        profile.LastName.Should().Be("User");
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
            FirstName: "Updated",
            LastName: "Name",
            Email: "updated@example.com",
            PhoneNumber: "+48123456789",
            CorrespondenceAddress: new AddressDto(
                Street: "Test Street",
                HouseNumber: "1",
                ApartmentNumber: null,
                PostalCode: "00-001",
                City: "Warsaw",
                Province: "Mazowieckie"
            ));

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/api/users/profile", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify in database
        var user = await DbContext.Users.FindAsync(new UserId(1));
        user.Should().NotBeNull();
        user!.FirstName.Value.Should().Be("Updated");
        user.LastName.Value.Should().Be("Name");
        user.Email.Value.Should().Be("updated@example.com");
        user.PhoneNumber?.Value.Should().Be("+48123456789");
        user.DateOfBirth.Should().Be(new DateTime(1990, 1, 1));
        // Bio is not part of User entity
    }

    [Fact]
    public async Task UpdateProfile_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedTestData();
        
        var command = new UpdateUserProfile(
            FirstName: "Updated",
            LastName: "Name",
            Email: "invalid-email",
            PhoneNumber: "+48123456789",
            CorrespondenceAddress: new AddressDto(
                Street: "Test Street",
                HouseNumber: "1",
                ApartmentNumber: null,
                PostalCode: "00-001",
                City: "Warsaw",
                Province: "Mazowieckie"
            ));

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
            FirstName: "Updated",
            LastName: "Name",
            Email: "updated@example.com",
            PhoneNumber: "+48123456789",
            CorrespondenceAddress: new AddressDto(
                Street: "Test Street",
                HouseNumber: "1",
                ApartmentNumber: null,
                PostalCode: "00-001",
                City: "Warsaw",
                Province: "Mazowieckie"
            ));

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
            NotificationsEnabled: false,
            EmailNotificationsEnabled: true);

        // Act
        var response = await _authenticatedClient.PutAsJsonAsync("/api/users/preferences", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify in database
        var user = await DbContext.Users.FindAsync(new UserId(1));
        user.Should().NotBeNull();
        user.NotificationsEnabled.Should().BeFalse();
        user.EmailNotificationsEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task UpdatePreferences_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var command = new UpdateUserPreferences(
            NotificationsEnabled: true,
            EmailNotificationsEnabled: true);

        // Act
        var response = await Client.PutAsJsonAsync("/api/users/preferences", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task SeedTestData()
    {
        var specialization = TestDataFactory.CreateSpecialization(userId: 1);
        
        await DbContext.Specializations.AddAsync(specialization);
        
        var user = User.CreateWithId(
            new UserId(1),
            new Email("test@example.com"),
            new HashedPassword("$2a$10$hashedpassword123"), // Hashed password
            new FirstName("Test"),
            null, // SecondName
            new LastName("User"),
            new Pesel("90010112345"),
            new PwzNumber("1234567"),
            new PhoneNumber("+48123456789"),
            new DateTime(1990, 1, 1),
            new Address(
                "Test Street",
                "1",
                null,
                "00-001",
                "Warsaw",
                "Mazowieckie"
            ),
            DateTime.UtcNow,
            true,
            true);
            
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
    }
}