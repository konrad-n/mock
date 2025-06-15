using FluentAssertions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;
using Xunit;

namespace SledzSpecke.Core.Tests.ValueObjects;

public class DurationTests
{
    [Fact]
    public void Duration_WithNegativeHours_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Duration(-1, 30);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Hours cannot be negative.");
    }

    [Fact]
    public void Duration_WithNegativeMinutes_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Duration(1, -30);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Minutes cannot be negative.");
    }

    [Fact]
    public void Duration_WithZeroHoursAndZeroMinutes_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Duration(0, 0);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Duration must be greater than zero.");
    }

    [Theory]
    [InlineData(1, 30, 90)]
    [InlineData(2, 0, 120)]
    [InlineData(0, 45, 45)]
    [InlineData(24, 0, 1440)]
    [InlineData(0, 1, 1)]
    public void Duration_WithValidValues_ShouldCalculateTotalMinutesCorrectly(int hours, int minutes, int expectedTotalMinutes)
    {
        // Arrange & Act
        var duration = new Duration(hours, minutes);

        // Assert
        duration.Hours.Should().Be(hours);
        duration.Minutes.Should().Be(minutes);
        duration.TotalMinutes.Should().Be(expectedTotalMinutes);
    }

    [Fact]
    public void Duration_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var duration1 = new Duration(2, 30);
        var duration2 = new Duration(2, 30);
        var duration3 = new Duration(2, 31);

        // Act & Assert
        duration1.Should().Be(duration2);
        duration1.Should().NotBe(duration3);
        (duration1 == duration2).Should().BeTrue();
        (duration1 != duration3).Should().BeTrue();
    }

    [Fact]
    public void Duration_ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var duration1 = new Duration(2, 30);
        var duration2 = new Duration(0, 45);
        var duration3 = new Duration(10, 0);

        // Act & Assert
        duration1.ToString().Should().Be("2h 30m");
        duration2.ToString().Should().Be("0h 45m");
        duration3.ToString().Should().Be("10h");
    }

    [Theory]
    [InlineData(0, 60)]
    [InlineData(0, 120)]
    [InlineData(0, 999)]
    public void Duration_WithMinutesGreaterThan59_ShouldBeAllowed(int hours, int minutes)
    {
        // This is intentional based on the handover notes - allows minutes > 59
        // Arrange & Act
        var act = () => new Duration(hours, minutes);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Duration_Add_ShouldWorkCorrectly()
    {
        // Arrange
        var duration1 = new Duration(1, 30);
        var duration2 = new Duration(2, 45);

        // Act
        var result = duration1.Add(duration2);

        // Assert
        result.TotalMinutes.Should().Be(255); // 90 + 165
    }

    [Fact]
    public void Duration_FromMinutes_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var duration1 = Duration.FromMinutes(90);
        var duration2 = Duration.FromMinutes(150);
        var duration3 = Duration.FromMinutes(60);

        // Assert
        duration1.Hours.Should().Be(1);
        duration1.Minutes.Should().Be(30);
        duration2.Hours.Should().Be(2);
        duration2.Minutes.Should().Be(30);
        duration3.Hours.Should().Be(1);
        duration3.Minutes.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Duration_FromMinutes_WithInvalidValue_ShouldThrowException(int totalMinutes)
    {
        // Arrange & Act
        var act = () => Duration.FromMinutes(totalMinutes);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Total minutes must be greater than zero.");
    }

    [Fact]
    public void Duration_ComparisonOperators_ShouldWorkCorrectly()
    {
        // Arrange
        var duration1 = new Duration(1, 30);
        var duration2 = new Duration(2, 45);
        var duration3 = new Duration(1, 30);

        // Act & Assert
        (duration1 < duration2).Should().BeTrue();
        (duration2 > duration1).Should().BeTrue();
        (duration1 <= duration3).Should().BeTrue();
        (duration1 >= duration3).Should().BeTrue();
        (duration1 <= duration2).Should().BeTrue();
        (duration2 >= duration1).Should().BeTrue();
    }
}