using FluentAssertions;
using SledzSpecke.Core.ValueObjects;
using System;
using Xunit;

namespace SledzSpecke.Tests.Integration.ValueObjects;

public class LocationTests
{
    [Theory]
    [InlineData("Hospital A")]
    [InlineData("Emergency Room")]
    [InlineData("123 Main Street, City")]
    [InlineData("Building 5, Floor 3, Room 301")]
    [InlineData("ICU Ward - Station 2")]
    public void Create_WithValidLocation_ShouldSucceed(string location)
    {
        // Act
        var result = () => new Location(location);

        // Assert
        result.Should().NotThrow();
        var locationValue = new Location(location);
        locationValue.Value.Should().Be(location);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_WithEmptyOrWhitespace_ShouldThrow(string location)
    {
        // Act
        var result = () => new Location(location);

        // Assert
        result.Should().Throw<SledzSpecke.Core.Exceptions.DomainException>()
            .WithMessage("Location cannot be empty.");
    }

    [Fact]
    public void Create_WithNull_ShouldThrow()
    {
        // Act
        var result = () => new Location(null!);

        // Assert
        result.Should().Throw<SledzSpecke.Core.Exceptions.DomainException>()
            .WithMessage("Location cannot be empty.");
    }

    [Fact]
    public void Create_WithSpecialCharacters_ShouldSucceed()
    {
        // Arrange
        var location = "St. Mary's Hospital - Ward #3 (2nd Floor)";

        // Act
        var result = () => new Location(location);

        // Assert
        result.Should().NotThrow();
        var locationValue = new Location(location);
        locationValue.Value.Should().Be(location);
    }

    [Fact]
    public void Equals_WithSameLocation_ShouldReturnTrue()
    {
        // Arrange
        var location1 = new Location("Hospital A");
        var location2 = new Location("Hospital A");

        // Act & Assert
        location1.Should().Be(location2);
        (location1 == location2).Should().BeTrue();
        location1.GetHashCode().Should().Be(location2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentLocation_ShouldReturnFalse()
    {
        // Arrange
        var location1 = new Location("Hospital A");
        var location2 = new Location("Hospital B");

        // Act & Assert
        location1.Should().NotBe(location2);
        (location1 == location2).Should().BeFalse();
        (location1 != location2).Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_ToLocation_ShouldWork()
    {
        // Arrange
        const string locationString = "Hospital A";

        // Act
        Location location = locationString;

        // Assert
        location.Value.Should().Be(locationString);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var location = new Location("Hospital A");

        // Act
        string locationString = location;

        // Assert
        locationString.Should().Be("Hospital A");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var location = new Location("Hospital A");

        // Act
        var result = location.ToString();

        // Assert
        result.Should().Be("Hospital A");
    }
}