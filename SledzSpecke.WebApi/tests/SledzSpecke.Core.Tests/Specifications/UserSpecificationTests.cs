using FluentAssertions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Specifications;
using System;
using Xunit;

namespace SledzSpecke.Core.Tests.Specifications;

public class UserSpecificationTests
{
    // NOTE: UserByUsernameSpecification does not exist in the current implementation
    // The User entity doesn't have a Username property, it uses Email for authentication

    [Fact]
    public void UserByEmailSpecification_Should_Match_Exact_Email()
    {
        // Arrange
        var targetEmail = new Email("john@example.com");
        var specification = new UserByEmailSpecification(targetEmail);
        
        var matchingUser = CreateUser(email: "john@example.com");
        var nonMatchingUser = CreateUser(email: "jane@example.com");

        // Act & Assert
        specification.IsSatisfiedBy(matchingUser).Should().BeTrue();
        specification.IsSatisfiedBy(nonMatchingUser).Should().BeFalse();
    }

    // NOTE: UserBySpecializationSpecification does not exist in the current implementation
    // Users don't directly have specializations - specializations reference users via UserId

    [Fact]
    public void UserByRecentActivitySpecification_Should_Match_Recent_Logins()
    {
        // Arrange
        var daysThreshold = 30;
        var specification = new UserByRecentActivitySpecification(daysThreshold);
        
        var recentUser = CreateUser();
        recentUser.RecordLogin(); // Sets LastLoginAt to now
        
        var inactiveUser = CreateUser();
        // User hasn't logged in (LastLoginAt is null)

        // Act & Assert
        specification.IsSatisfiedBy(recentUser).Should().BeTrue();
        specification.IsSatisfiedBy(inactiveUser).Should().BeFalse();
    }

    // NOTE: UserByProfileCompleteSpecification does not exist in the current implementation

    [Fact]
    public void UserByFullNameSpecification_Should_Match_Partial_Names()
    {
        // Arrange
        var searchTerm = "doe";
        var specification = new UserByFullNameSpecification(searchTerm);
        
        var johnDoe = CreateUser(fullName: "John Doe");
        var janeDoe = CreateUser(fullName: "Jane Doe");
        var bobSmith = CreateUser(fullName: "Bob Smith");
        var doesNotMatch = CreateUser(fullName: "Alice Johnson");

        // Act & Assert
        specification.IsSatisfiedBy(johnDoe).Should().BeTrue();
        specification.IsSatisfiedBy(janeDoe).Should().BeTrue();
        specification.IsSatisfiedBy(doesNotMatch).Should().BeFalse();
        specification.IsSatisfiedBy(bobSmith).Should().BeFalse();
    }

    [Fact]
    public void UserByProvinceSpecification_Should_Match_Province()
    {
        // Arrange
        var specification = new UserByProvinceSpecification("Mazowieckie");
        
        var userInMazowieckie = CreateUser();
        var userInOtherProvince = CreateUserWithAddress(province: "Pomorskie");

        // Act & Assert
        specification.IsSatisfiedBy(userInMazowieckie).Should().BeTrue();
        specification.IsSatisfiedBy(userInOtherProvince).Should().BeFalse();
    }

    [Fact]
    public void UserByCitySpecification_Should_Match_City()
    {
        // Arrange
        var specification = new UserByCitySpecification("Warsaw");
        
        var userInWarsaw = CreateUser();
        var userInKrakow = CreateUserWithAddress(city: "Krakow");

        // Act & Assert
        specification.IsSatisfiedBy(userInWarsaw).Should().BeTrue();
        specification.IsSatisfiedBy(userInKrakow).Should().BeFalse();
    }

    [Fact]
    public void UserByFullNameSpecification_Should_Be_Case_Insensitive()
    {
        // Arrange
        var specification = new UserByFullNameSpecification("JOHN");
        var user = CreateUser(fullName: "John Doe");

        // Act & Assert
        specification.IsSatisfiedBy(user).Should().BeTrue();
    }

    [Fact]
    public void UserSpecificationExtensions_GetActiveUsers_Should_Filter_By_Activity()
    {
        // Arrange
        var specification = UserSpecificationExtensions.GetActiveUsers(7); // Last 7 days
        
        var activeUser = CreateUser();
        activeUser.RecordLogin();
        
        var inactiveUser = CreateUser();

        // Act & Assert
        specification.IsSatisfiedBy(activeUser).Should().BeTrue();
        specification.IsSatisfiedBy(inactiveUser).Should().BeFalse();
    }

    [Fact]
    public void UserSpecificationExtensions_SearchUsers_Should_Search_Multiple_Fields()
    {
        // Arrange
        var searchTerm = "john";
        var specification = UserSpecificationExtensions.SearchUsers(searchTerm);
        
        var userWithMatchingFullName = CreateUser(fullName: "John Doe");
        var userWithMatchingEmail = CreateUser(email: "john@example.com", fullName: "Bob Smith");
        var userWithNoMatch = CreateUser(email: "alice@example.com", fullName: "Alice Smith");

        // Act & Assert
        specification.IsSatisfiedBy(userWithMatchingFullName).Should().BeTrue();
        specification.IsSatisfiedBy(userWithMatchingEmail).Should().BeTrue();
        specification.IsSatisfiedBy(userWithNoMatch).Should().BeFalse();
    }

    // Helper methods
    private static int _testUserId = 1;
    
    private User CreateUser(
        string email = "test@example.com",
        string fullName = "Test User")
    {
        var names = fullName.Split(' ');
        var firstName = names.Length > 0 ? names[0] : "Test";
        var lastName = names.Length > 1 ? names[names.Length - 1] : "User";
        
        return User.CreateWithId(
            new UserId(_testUserId++),
            new Email(email),
            new HashedPassword("tL8XQn5ScIhHqxKNMQJfYGD3GmjptUPgxlrXH1zVBvI="), // Valid base64 hash format
            new FirstName(firstName),
            null, // SecondName
            new LastName(lastName),
            new Pesel("44051401458"), // Valid PESEL with correct checksum
            new PwzNumber(PwzNumber.GenerateValidPwz(_testUserId)), // Valid PWZ with correct checksum
            new PhoneNumber("+48123456789"), // Valid phone number
            new DateTime(1944, 5, 14), // Date of birth matching PESEL 44051401458
            new Address("Test Street", "1", null, "00-000", "Warsaw", "Mazowieckie", "Poland"),
            DateTime.UtcNow
        );
    }
    
    private User CreateUserWithAddress(
        string city = "Warsaw",
        string province = "Mazowieckie")
    {
        return User.CreateWithId(
            new UserId(_testUserId++),
            new Email($"test{_testUserId}@example.com"),
            new HashedPassword("tL8XQn5ScIhHqxKNMQJfYGD3GmjptUPgxlrXH1zVBvI="),
            new FirstName("Test"),
            null,
            new LastName("User"),
            new Pesel("44051401458"),
            new PwzNumber(PwzNumber.GenerateValidPwz(_testUserId)), // Valid PWZ with correct checksum
            new PhoneNumber("+48123456789"),
            new DateTime(1944, 5, 14),
            new Address("Test Street", "1", null, "00-000", city, province, "Poland"),
            DateTime.UtcNow
        );
    }
}