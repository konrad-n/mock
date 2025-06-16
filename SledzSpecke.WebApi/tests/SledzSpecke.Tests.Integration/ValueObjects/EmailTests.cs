using FluentAssertions;
using SledzSpecke.Core.ValueObjects;
using System;
using Xunit;

namespace SledzSpecke.Tests.Integration.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@example.com")]
    [InlineData("user+tag@example.com")]
    [InlineData("user@subdomain.example.com")]
    [InlineData("user123@example.co.uk")]
    public void Create_WithValidEmail_ShouldSucceed(string email)
    {
        // Act
        var result = () => new Email(email);

        // Assert
        result.Should().NotThrow();
        var emailValue = new Email(email);
        emailValue.Value.Should().Be(email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@.com")]
    [InlineData("user@example")]
    [InlineData("user @example.com")]
    [InlineData("user@example .com")]
    [InlineData("user@@example.com")]
    public void Create_WithInvalidEmail_ShouldThrow(string email)
    {
        // Act
        var result = () => new Email(email);

        // Assert
        result.Should().Throw<SledzSpecke.Core.Exceptions.InvalidEmailException>();
    }

    [Fact]
    public void Create_WithNull_ShouldThrow()
    {
        // Act
        var result = () => new Email(null!);

        // Assert
        result.Should().Throw<SledzSpecke.Core.Exceptions.InvalidEmailException>();
    }

    [Fact]
    public void Equals_WithSameEmail_ShouldReturnTrue()
    {
        // Arrange
        var email1 = new Email("user@example.com");
        var email2 = new Email("user@example.com");

        // Act & Assert
        email1.Should().Be(email2);
        (email1 == email2).Should().BeTrue();
        email1.GetHashCode().Should().Be(email2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentEmail_ShouldReturnFalse()
    {
        // Arrange
        var email1 = new Email("user1@example.com");
        var email2 = new Email("user2@example.com");

        // Act & Assert
        email1.Should().NotBe(email2);
        (email1 == email2).Should().BeFalse();
        (email1 != email2).Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_ToEmail_ShouldWork()
    {
        // Arrange
        const string emailString = "user@example.com";

        // Act
        Email email = emailString;

        // Assert
        email.Value.Should().Be(emailString);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var email = new Email("user@example.com");

        // Act
        string emailString = email;

        // Assert
        emailString.Should().Be("user@example.com");
    }
}