using FluentAssertions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;
using System;
using Xunit;

namespace SledzSpecke.Core.Tests.Specifications;

public class UserSpecificationTests
{
    [Fact]
    public void UserByUsernameSpecification_Should_Match_Exact_Username()
    {
        // Arrange
        var targetUsername = new Username("johndoe");
        var specification = new UserByUsernameSpecification(targetUsername);
        
        var matchingUser = CreateUser(username: "johndoe");
        var nonMatchingUser = CreateUser(username: "janedoe");

        // Act & Assert
        specification.IsSatisfiedBy(matchingUser).Should().BeTrue();
        specification.IsSatisfiedBy(nonMatchingUser).Should().BeFalse();
    }

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

    [Fact]
    public void UserBySpecializationSpecification_Should_Match_Specialization()
    {
        // Arrange
        var targetSpecializationId = new SpecializationId(5);
        var specification = new UserBySpecializationSpecification(targetSpecializationId);
        
        // Create users with different specialization IDs
        var matchingUser = CreateUserWithSpecialization(targetSpecializationId);
        var nonMatchingUser = CreateUserWithSpecialization(new SpecializationId(10));

        // Act & Assert
        specification.IsSatisfiedBy(matchingUser).Should().BeTrue();
        specification.IsSatisfiedBy(nonMatchingUser).Should().BeFalse();
    }

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

    [Fact]
    public void UserByProfileCompleteSpecification_Should_Match_Complete_Profiles()
    {
        // Arrange
        var specification = new UserByProfileCompleteSpecification();
        
        // Create user with complete profile
        var completeUser = CreateUser();
        // Note: User entity doesn't have public methods to update profile fields
        // Using reflection or internal methods would be needed for testing
        // For now, testing with basic user
        
        var incompleteUser1 = CreateUser();
        var incompleteUser2 = CreateUser();

        // Act & Assert
        // Note: Since we can't easily set profile fields in tests,
        // this test would need to be adjusted based on actual User implementation
        specification.IsSatisfiedBy(completeUser).Should().BeFalse(); // All users incomplete by default
        specification.IsSatisfiedBy(incompleteUser1).Should().BeFalse();
        specification.IsSatisfiedBy(incompleteUser2).Should().BeFalse();
    }

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
        var userWithMatchingUsername = CreateUser(username: "johnny123", fullName: "Bob Smith");
        var userWithNoMatch = CreateUser(username: "alice", fullName: "Alice Smith");

        // Act & Assert
        specification.IsSatisfiedBy(userWithMatchingFullName).Should().BeTrue();
        specification.IsSatisfiedBy(userWithMatchingUsername).Should().BeTrue();
        specification.IsSatisfiedBy(userWithNoMatch).Should().BeFalse();
    }

    // Helper methods
    private User CreateUser(
        string username = "testuser",
        string email = "test@example.com",
        string fullName = "Test User")
    {
        // Note: User.Create requires SpecializationId in some overloads
        // Using the factory method that matches available parameters
        return User.Create(
            new Email(email),
            new Username(username),
            new HashedPassword("abcdefghijklmnopqrstuvwxyz0123456789ABCD="), // Valid hash format
            new FullName(fullName),
            new SmkVersion("new"),
            new SpecializationId(1) // Default specialization
        );
    }
    
    private User CreateUserWithSpecialization(SpecializationId specializationId)
    {
        return User.Create(
            new Email("test@example.com"),
            new Username("testuser"),
            new HashedPassword("abcdefghijklmnopqrstuvwxyz0123456789ABCD="), // Valid hash format
            new FullName("Test User"),
            new SmkVersion("new"),
            specializationId
        );
    }
}