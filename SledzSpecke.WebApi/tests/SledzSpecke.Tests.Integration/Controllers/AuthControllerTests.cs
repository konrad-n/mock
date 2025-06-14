using FluentAssertions;
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

public class AuthControllerTests : IntegrationTestBase
{
    [Fact]
    public async Task SignUp_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await SeedSpecialization();
        
        var command = new SignUp(
            Email: "newuser@example.com",
            Username: "newuser",
            Password: "SecurePassword123!",
            FullName: "John Doe",
            SmkVersion: "new",
            SpecializationId: 1);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-up", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify user was created in database
        var user = await DbContext.Users.FindAsync(2); // ID 2 because seed data creates user with ID 1
        user.Should().NotBeNull();
        user!.Email.Value.Should().Be("newuser@example.com");
        user.Username.Value.Should().Be("newuser");
        user.FullName.Value.Should().Be("John Doe");
    }

    [Fact]
    public async Task SignUp_WithExistingEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedSpecialization();
        await SeedUser();
        
        var command = new SignUp(
            Email: "test@example.com", // Same as seeded user
            Username: "differentuser",
            Password: "SecurePassword123!",
            FullName: "Jane Doe",
            SmkVersion: "new",
            SpecializationId: 1);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-up", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("already exists");
    }

    [Fact]
    public async Task SignUp_WithExistingUsername_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedSpecialization();
        await SeedUser();
        
        var command = new SignUp(
            Email: "different@example.com",
            Username: "testuser", // Same as seeded user
            Password: "SecurePassword123!",
            FullName: "Jane Doe",
            SmkVersion: "new",
            SpecializationId: 1);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-up", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("already exists");
    }

    [Fact]
    public async Task SignUp_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedSpecialization();
        
        var command = new SignUp(
            Email: "invalid-email",
            Username: "newuser",
            Password: "SecurePassword123!",
            FullName: "John Doe",
            SmkVersion: "new",
            SpecializationId: 1);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-up", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SignUp_WithInvalidSmkVersion_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedSpecialization();
        
        var command = new SignUp(
            Email: "newuser@example.com",
            Username: "newuser",
            Password: "SecurePassword123!",
            FullName: "John Doe",
            SmkVersion: "invalid",
            SpecializationId: 1);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-up", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SignUp_WithNonExistentSpecialization_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new SignUp(
            Email: "newuser@example.com",
            Username: "newuser",
            Password: "SecurePassword123!",
            FullName: "John Doe",
            SmkVersion: "new",
            SpecializationId: 999); // Non-existent

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-up", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Specialization with ID 999 not found");
    }

    [Fact]
    public async Task SignIn_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        await SeedSpecialization();
        await SeedUser();
        
        var command = new SignIn(
            Email: "test@example.com",
            Password: "TestPassword123!");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-in", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var token = await response.Content.ReadFromJsonAsync<JwtDto>();
        token.Should().NotBeNull();
        token!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task SignIn_WithInvalidPassword_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedSpecialization();
        await SeedUser();
        
        var command = new SignIn(
            Email: "test@example.com",
            Password: "WrongPassword!");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-in", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid credentials");
    }

    [Fact]
    public async Task SignIn_WithNonExistentEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new SignIn(
            Email: "nonexistent@example.com",
            Password: "Password123!");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-in", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invalid credentials");
    }

    [Fact]
    public async Task SignIn_WithInvalidEmailFormat_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new SignIn(
            Email: "invalid-email",
            Password: "Password123!");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-in", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task SeedSpecialization()
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
        await DbContext.SaveChangesAsync();
    }

    private async Task SeedUser()
    {
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