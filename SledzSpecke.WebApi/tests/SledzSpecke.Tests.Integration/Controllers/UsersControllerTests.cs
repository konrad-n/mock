using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using AddressDto = SledzSpecke.Application.Commands.AddressDto;
using SledzSpecke.Tests.Integration.Common;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace SledzSpecke.Tests.Integration.Controllers;

public class UsersControllerTests : IntegrationTestBase
{
    private readonly HttpClient _authenticatedClient;

    public UsersControllerTests(SledzSpeckeApiFactory factory) : base(factory)
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
    public async Task SignUp_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var command = new SignUp(
            Email: "newuser@example.com",
            Password: "SecurePassword123!",
            FirstName: "John",
            LastName: "Doe",
            Pesel: "90010123456",
            PwzNumber: "1234567",
            PhoneNumber: "+48123456789",
            DateOfBirth: new DateTime(1990, 1, 1),
            CorrespondenceAddress: new SledzSpecke.Application.Commands.AddressDto(
                Street: "Main Street",
                HouseNumber: "123",
                ApartmentNumber: "4A",
                PostalCode: "00-001",
                City: "Warsaw",
                Province: "Mazowieckie"
            ));

        // Act
        var response = await Client.PostAsJsonAsync("/users", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task SignUp_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new SignUp(
            Email: "invalid-email",
            Password: "SecurePassword123!",
            FirstName: "John",
            LastName: "Doe",
            Pesel: "92010112345",
            PwzNumber: "1234567",
            PhoneNumber: "+48123456789",
            DateOfBirth: new DateTime(1992, 1, 1),
            CorrespondenceAddress: new AddressDto(
                "ul. Testowa",
                "1",
                null,
                "00-001",
                "Warsaw",
                "Mazowieckie"
            ));

        // Act
        var response = await Client.PostAsJsonAsync("/users", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("InvalidEmail");
        content.Should().Contain("Invalid email format");
    }

    [Fact]
    public async Task SignUp_WithEmptyFullName_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new SignUp(
            Email: "newuser@example.com",
            Password: "SecurePassword123!",
            FirstName: "",  // Empty first name
            LastName: "Doe",
            Pesel: "92010112345",
            PwzNumber: "1234567",
            PhoneNumber: "+48123456789",
            DateOfBirth: new DateTime(1992, 1, 1),
            CorrespondenceAddress: new AddressDto(
                "ul. Testowa",
                "1",
                null,
                "00-001",
                "Warsaw",
                "Mazowieckie"
            ));

        // Act
        var response = await Client.PostAsJsonAsync("/users", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("InvalidFullName");
        content.Should().Contain("Full name cannot be empty");
    }

    [Fact]
    public async Task SignIn_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        await SignUpUser("testuser@example.com", "TestPassword123!", "Test User");
        
        var command = new SignIn(
            Username: "testuser",
            Password: "TestPassword123!");

        // Act
        var response = await Client.PostAsJsonAsync("/users/sign-in", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var token = await response.Content.ReadFromJsonAsync<JwtDto>();
        token.Should().NotBeNull();
        token!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task SignIn_WithInvalidCredentials_ShouldReturnBadRequest()
    {
        // Arrange
        await SignUpUser("testuser@example.com", "TestPassword123!", "Test User");
        
        var command = new SignIn(
            Email: "testuser@example.com",
            Password: "WrongPassword!");

        // Act
        var response = await Client.PostAsJsonAsync("/users/sign-in", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("InvalidCredentials");
    }

    [Fact]
    public async Task SignIn_WithNonExistentUser_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new SignIn(
            Email: "nonexistent@example.com",
            Password: "Password123!");

        // Act
        var response = await Client.PostAsJsonAsync("/users/sign-in", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("InvalidCredentials");
    }

    [Fact]
    public async Task GetMe_WithAuthentication_ShouldReturnUserInfo()
    {
        // Act
        var response = await _authenticatedClient.GetAsync("/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(TestAuthHandler.TestUserId.ToString());
        content.Should().Contain(TestAuthHandler.TestUserEmail);
    }

    [Fact]
    public async Task GetMe_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task SignUpUser(string email, string password, string fullName)
    {
        var command = new SignUp(
            Email: email,
            Password: password,
            FullName: fullName,
            Role: "User");
            
        await Client.PostAsJsonAsync("/users", command);
    }
}