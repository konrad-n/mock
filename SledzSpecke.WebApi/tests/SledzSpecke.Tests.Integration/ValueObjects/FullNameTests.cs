using FluentAssertions;
using SledzSpecke.Core.ValueObjects;
using System;
using Xunit;

namespace SledzSpecke.Tests.Integration.ValueObjects;

public class FullNameTests
{
    [Theory]
    [InlineData("John Doe")]
    [InlineData("Jane Smith")]
    [InlineData("Dr. John Doe Jr.")]
    [InlineData("Mary-Jane O'Connor")]
    [InlineData("José García")]
    [InlineData("François Müller")]
    public void Create_WithValidName_ShouldSucceed(string name)
    {
        // Act
        var result = () => new FullName(name);

        // Assert
        result.Should().NotThrow();
        var fullName = new FullName(name);
        fullName.Value.Should().Be(name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_WithEmptyOrWhitespace_ShouldThrow(string name)
    {
        // Act
        var result = () => new FullName(name);

        // Assert
        result.Should().Throw<SledzSpecke.Core.Exceptions.DomainException>()
            .WithMessage("Full name cannot be empty.");
    }

    [Fact]
    public void Create_WithNull_ShouldThrow()
    {
        // Act
        var result = () => new FullName(null!);

        // Assert
        result.Should().Throw<SledzSpecke.Core.Exceptions.DomainException>()
            .WithMessage("Full name cannot be empty.");
    }

    [Fact]
    public void Create_WithVeryLongName_ShouldSucceed()
    {
        // Arrange
        var longName = new string('A', 200);

        // Act
        var result = () => new FullName(longName);

        // Assert
        result.Should().NotThrow();
    }

    [Fact]
    public void Equals_WithSameName_ShouldReturnTrue()
    {
        // Arrange
        var name1 = new FullName("John Doe");
        var name2 = new FullName("John Doe");

        // Act & Assert
        name1.Should().Be(name2);
        (name1 == name2).Should().BeTrue();
        name1.GetHashCode().Should().Be(name2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentName_ShouldReturnFalse()
    {
        // Arrange
        var name1 = new FullName("John Doe");
        var name2 = new FullName("Jane Doe");

        // Act & Assert
        name1.Should().NotBe(name2);
        (name1 == name2).Should().BeFalse();
        (name1 != name2).Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_ToFullName_ShouldWork()
    {
        // Arrange
        const string nameString = "John Doe";

        // Act
        FullName fullName = nameString;

        // Assert
        fullName.Value.Should().Be(nameString);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var fullName = new FullName("John Doe");

        // Act
        string nameString = fullName;

        // Assert
        nameString.Should().Be("John Doe");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var fullName = new FullName("John Doe");

        // Act
        var result = fullName.ToString();

        // Assert
        result.Should().Be("John Doe");
    }
}