using FluentAssertions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using AddressDto = SledzSpecke.Application.Commands.AddressDto;
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
    public AuthControllerTests(SledzSpeckeApiFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task SignUp_WithValidData_ShouldReturnOk()
    {
        // Arrange
        await SeedSpecialization();
        
        var command = new SignUp(
            Email: "newuser@example.com",
            Password: "SecurePassword123!",
            FirstName: "John",
            LastName: "Doe",
            Pesel: "90010123456",
            PwzNumber: "1234567",
            PhoneNumber: "+48123456789",
            DateOfBirth: new DateTime(1990, 1, 1),
            CorrespondenceAddress: new AddressDto(
                Street: "Main Street",
                HouseNumber: "123",
                ApartmentNumber: "4A",
                PostalCode: "00-001",
                City: "Warsaw",
                Province: "Mazowieckie"
            ));

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-up", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify user was created in database
        var users = DbContext.Users.Where(u => u.Email.Value == "newuser@example.com").ToList();
        users.Should().HaveCount(1);
        var user = users.First();
        user.Email.Value.Should().Be("newuser@example.com");
        user.FirstName.Value.Should().Be("John");
        user.LastName.Value.Should().Be("Doe");
    }

    [Fact]
    public async Task SignUp_WithExistingEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedSpecialization();
        await SeedUser();
        
        var command = new SignUp(
            Email: "test@example.com", // Same as seeded user
            Password: "SecurePassword123!",
            FirstName: "Jane",
            LastName: "Doe",
            Pesel: "90020234567",
            PwzNumber: "2345678",
            PhoneNumber: "+48234567890",
            DateOfBirth: new DateTime(1990, 2, 2),
            CorrespondenceAddress: new AddressDto(
                Street: "Main Street",
                HouseNumber: "456",
                ApartmentNumber: null,
                PostalCode: "00-002",
                City: "Warsaw",
                Province: "Mazowieckie"
            ));

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-up", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("already");
    }

    [Fact]
    public async Task SignUp_WithExistingPesel_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedSpecialization();
        await SeedUser();
        
        var command = new SignUp(
            Email: "different@example.com",
            Password: "SecurePassword123!",
            FirstName: "Jane",
            LastName: "Doe",
            Pesel: "90030345678",
            PwzNumber: "3456789",
            PhoneNumber: "+48345678901",
            DateOfBirth: new DateTime(1990, 3, 3),
            CorrespondenceAddress: new AddressDto(
                Street: "Second Street",
                HouseNumber: "789",
                ApartmentNumber: "2B",
                PostalCode: "00-003",
                City: "Warsaw",
                Province: "Mazowieckie"
            ));

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-up", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("already");
    }

    [Fact]
    public async Task SignUp_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedSpecialization();
        
        var command = new SignUp(
            Email: "invalid-email",
            Password: "SecurePassword123!",
            FirstName: "John",
            LastName: "Doe",
            Pesel: "90040456789",
            PwzNumber: "4567890",
            PhoneNumber: "+48456789012",
            DateOfBirth: new DateTime(1990, 4, 4),
            CorrespondenceAddress: new AddressDto(
                Street: "Third Street",
                HouseNumber: "100",
                ApartmentNumber: null,
                PostalCode: "00-004",
                City: "Warsaw",
                Province: "Mazowieckie"
            ));

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-up", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SignUp_WithInvalidPesel_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedSpecialization();
        
        var command = new SignUp(
            Email: "newuser@example.com",
            Password: "SecurePassword123!",
            FirstName: "John",
            LastName: "Doe",
            Pesel: "invalid", // Invalid PESEL
            PwzNumber: "1234567",
            PhoneNumber: "+48123456789",
            DateOfBirth: new DateTime(1990, 1, 1),
            CorrespondenceAddress: new AddressDto(
                Street: "Main Street",
                HouseNumber: "123",
                ApartmentNumber: null,
                PostalCode: "00-001",
                City: "Warsaw",
                Province: "Mazowieckie"
            ));

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/sign-up", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
        var specialization = TestDataFactory.CreateSpecialization(userId: 1);
        
        await DbContext.Specializations.AddAsync(specialization);
        await DbContext.SaveChangesAsync();
    }

    private async Task SeedUser()
    {
        var user = User.CreateWithId(
            new UserId(1),
            new Email("test@example.com"),
            new HashedPassword(BCrypt.Net.BCrypt.HashPassword("TestPassword123!")), // Properly hashed
            new FirstName("Test"),
            null,
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