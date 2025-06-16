using FluentAssertions;
using SledzSpecke.Core.ValueObjects;
using System;
using Xunit;

namespace SledzSpecke.Tests.Integration.ValueObjects;

public class DescriptionTests
{
    [Theory]
    [InlineData("Regular morning shift")]
    [InlineData("Emergency department rotation")]
    [InlineData("Patient consultation and examination")]
    [InlineData("Surgical procedure assistance")]
    [InlineData("A very detailed description with multiple sentences. Including special characters like: commas, periods, and semicolons; also parentheses (like this).")]
    public void Create_WithValidDescription_ShouldSucceed(string description)
    {
        // Act
        var result = () => new Description(description);

        // Assert
        result.Should().NotThrow();
        var descriptionValue = new Description(description);
        descriptionValue.Value.Should().Be(description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_WithEmptyOrWhitespace_ShouldThrow(string description)
    {
        // Act
        var result = () => new Description(description);

        // Assert
        result.Should().Throw<SledzSpecke.Core.Exceptions.DomainException>()
            .WithMessage("Description cannot be empty.");
    }

    [Fact]
    public void Create_WithNull_ShouldThrow()
    {
        // Act
        var result = () => new Description(null!);

        // Assert
        result.Should().Throw<SledzSpecke.Core.Exceptions.DomainException>()
            .WithMessage("Description cannot be empty.");
    }

    [Fact]
    public void Create_WithVeryLongDescription_ShouldSucceed()
    {
        // Arrange
        var longDescription = new string('A', 1000);

        // Act
        var result = () => new Description(longDescription);

        // Assert
        result.Should().NotThrow();
        var descriptionValue = new Description(longDescription);
        descriptionValue.Value.Should().Be(longDescription);
    }

    [Fact]
    public void Create_WithMultilineDescription_ShouldSucceed()
    {
        // Arrange
        var multilineDescription = @"First line of description.
Second line with more details.
Third line with final information.";

        // Act
        var result = () => new Description(multilineDescription);

        // Assert
        result.Should().NotThrow();
        var descriptionValue = new Description(multilineDescription);
        descriptionValue.Value.Should().Be(multilineDescription);
    }

    [Fact]
    public void Equals_WithSameDescription_ShouldReturnTrue()
    {
        // Arrange
        var description1 = new Description("Test description");
        var description2 = new Description("Test description");

        // Act & Assert
        description1.Should().Be(description2);
        (description1 == description2).Should().BeTrue();
        description1.GetHashCode().Should().Be(description2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentDescription_ShouldReturnFalse()
    {
        // Arrange
        var description1 = new Description("First description");
        var description2 = new Description("Second description");

        // Act & Assert
        description1.Should().NotBe(description2);
        (description1 == description2).Should().BeFalse();
        (description1 != description2).Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_ToDescription_ShouldWork()
    {
        // Arrange
        const string descriptionString = "Test description";

        // Act
        Description description = descriptionString;

        // Assert
        description.Value.Should().Be(descriptionString);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var description = new Description("Test description");

        // Act
        string descriptionString = description;

        // Assert
        descriptionString.Should().Be("Test description");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var description = new Description("Test description");

        // Act
        var result = description.ToString();

        // Assert
        result.Should().Be("Test description");
    }
}