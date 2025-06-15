using FluentAssertions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;
using Xunit;

namespace SledzSpecke.Core.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("user.name@example.com")]
    [InlineData("user+tag@example.co.uk")]
    [InlineData("user_name@example-domain.com")]
    [InlineData("123@example.com")]
    [InlineData("user@subdomain.example.com")]
    public void Email_WithValidEmail_ShouldCreateInstance(string email)
    {
        // Act
        var emailObj = new Email(email);

        // Assert
        emailObj.Value.Should().Be(email.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Email_WithEmptyOrWhitespace_ShouldThrowException(string? email)
    {
        // Act
        var act = () => new Email(email!);

        // Assert
        act.Should().Throw<InvalidEmailException>();
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user @example.com")]
    [InlineData("user@example .com")]
    [InlineData("user@@example.com")]
    [InlineData("user.example.com")]
    [InlineData("user@.com")]
    [InlineData("user@example..com")]
    [InlineData("user@example.com.")]
    [InlineData(".user@example.com")]
    public void Email_WithInvalidFormat_ShouldThrowException(string email)
    {
        // Act
        var act = () => new Email(email);

        // Assert
        act.Should().Throw<InvalidEmailException>();
    }

    [Fact]
    public void Email_ShouldNormalizeToLowerCase()
    {
        // Arrange
        var upperCaseEmail = "User@EXAMPLE.COM";
        var mixedCaseEmail = "UsEr@ExAmPlE.cOm";

        // Act
        var email1 = new Email(upperCaseEmail);
        var email2 = new Email(mixedCaseEmail);

        // Assert
        email1.Value.Should().Be("user@example.com");
        email2.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void Email_Equality_ShouldBeCaseInsensitive()
    {
        // Arrange
        var email1 = new Email("user@example.com");
        var email2 = new Email("USER@EXAMPLE.COM");
        var email3 = new Email("different@example.com");

        // Act & Assert
        email1.Should().Be(email2);
        email1.Should().NotBe(email3);
        (email1 == email2).Should().BeTrue();
        (email1 != email3).Should().BeTrue();
    }

    [Fact]
    public void Email_ImplicitStringConversion_ShouldReturnValue()
    {
        // Arrange
        var email = new Email("user@example.com");

        // Act
        string emailString = email;

        // Assert
        emailString.Should().Be("user@example.com");
    }

    [Fact]
    public void Email_ToString_ShouldReturnValue()
    {
        // Arrange
        var email = new Email("user@example.com");

        // Act
        var result = email.ToString();

        // Assert
        result.Should().Be("user@example.com");
    }

    [Fact]
    public void Email_WithTooLongEmail_ShouldThrowException()
    {
        // Arrange
        var longEmail = new string('a', 90) + "@example.com"; // 102 characters total (max is 100)

        // Act
        var act = () => new Email(longEmail);

        // Assert
        act.Should().Throw<InvalidEmailException>()
            .WithMessage("*exceeds maximum length of 100 characters");
    }

    [Fact]
    public void Email_ImplicitOperator_FromString_ShouldWorkCorrectly()
    {
        // Arrange
        string emailString = "user@example.com";

        // Act
        Email email = emailString;

        // Assert
        email.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void Email_HashCode_ShouldBeConsistentWithEquality()
    {
        // Arrange
        var email1 = new Email("user@example.com");
        var email2 = new Email("USER@EXAMPLE.COM");
        var email3 = new Email("different@example.com");

        // Act & Assert
        email1.GetHashCode().Should().Be(email2.GetHashCode());
        email1.GetHashCode().Should().NotBe(email3.GetHashCode());
    }
}