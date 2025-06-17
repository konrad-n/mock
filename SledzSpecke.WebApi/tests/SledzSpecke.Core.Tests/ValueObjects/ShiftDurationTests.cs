using FluentAssertions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;
using Xunit;

namespace SledzSpecke.Core.Tests.ValueObjects;

public class ShiftShiftDurationTests
{
    [Fact]
    public void ShiftDuration_WithNegativeHours_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new ShiftDuration(-1, 30);

        // Assert
        act.Should().Throw<InvalidShiftDurationException>()
            .WithMessage("*Hours cannot be negative*");
    }

    [Fact]
    public void ShiftDuration_WithNegativeMinutes_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new ShiftDuration(1, -30);

        // Assert
        act.Should().Throw<InvalidShiftDurationException>()
            .WithMessage("*Minutes cannot be negative*");
    }

    [Fact]
    public void ShiftDuration_WithZeroHoursAndZeroMinutes_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new ShiftDuration(0, 0);

        // Assert
        act.Should().Throw<InvalidShiftDurationException>()
            .WithMessage("*Minimum shift duration is 60 minutes*");
    }

    [Theory]
    [InlineData(1, 30, 90)]
    [InlineData(2, 0, 120)]
    [InlineData(1, 0, 60)]
    [InlineData(24, 0, 1440)]
    [InlineData(1, 1, 61)]
    public void ShiftDuration_WithValidValues_ShouldCalculateTotalMinutesCorrectly(int hours, int minutes, int expectedTotalMinutes)
    {
        // Arrange & Act
        var duration = new ShiftDuration(hours, minutes);

        // Assert
        duration.Hours.Should().Be(hours);
        duration.Minutes.Should().Be(minutes);
        duration.TotalMinutes.Should().Be(expectedTotalMinutes);
    }

    [Fact]
    public void ShiftDuration_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var duration1 = new ShiftDuration(2, 30);
        var duration2 = new ShiftDuration(2, 30);
        var duration3 = new ShiftDuration(2, 31);

        // Act & Assert
        duration1.Should().Be(duration2);
        duration1.Should().NotBe(duration3);
        (duration1 == duration2).Should().BeTrue();
        (duration1 != duration3).Should().BeTrue();
    }

    [Fact]
    public void ShiftDuration_ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var duration1 = new ShiftDuration(2, 30);
        var duration2 = new ShiftDuration(1, 45);
        var duration3 = new ShiftDuration(10, 0);

        // Act & Assert
        duration1.ToString().Should().Be("02:30");
        duration2.ToString().Should().Be("01:45");
        duration3.ToString().Should().Be("10:00");
    }

    [Theory]
    [InlineData(0, 60)]
    [InlineData(0, 120)]
    [InlineData(0, 999)]
    public void ShiftDuration_WithMinutesGreaterThan59_ShouldBeAllowed(int hours, int minutes)
    {
        // This is intentional based on the handover notes - allows minutes > 59
        // Arrange & Act
        var act = () => new ShiftDuration(hours, minutes);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ShiftDuration_Add_ShouldWorkCorrectly()
    {
        // Arrange
        var duration1 = new ShiftDuration(1, 30);
        var duration2 = new ShiftDuration(2, 45);

        // Act
        var result = duration1 + duration2;

        // Assert
        result.TotalMinutes.Should().Be(255); // 90 + 165
    }

    [Fact]
    public void ShiftDuration_FromMinutes_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var duration1 = ShiftDuration.FromMinutes(90);
        var duration2 = ShiftDuration.FromMinutes(150);
        var duration3 = ShiftDuration.FromMinutes(60);

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
    public void ShiftDuration_FromMinutes_WithInvalidValue_ShouldThrowException(int totalMinutes)
    {
        // Arrange & Act
        var act = () => ShiftDuration.FromMinutes(totalMinutes);

        // Assert
        act.Should().Throw<InvalidShiftDurationException>()
            .WithMessage("*Minimum shift duration is 60 minutes*");
    }

    [Fact]
    public void ShiftDuration_ComparisonOperators_ShouldWorkCorrectly()
    {
        // Arrange
        var duration1 = new ShiftDuration(1, 30);
        var duration2 = new ShiftDuration(2, 45);
        var duration3 = new ShiftDuration(1, 30);

        // Act & Assert
        (duration1 < duration2).Should().BeTrue();
        (duration2 > duration1).Should().BeTrue();
        (duration1 <= duration3).Should().BeTrue();
        (duration1 >= duration3).Should().BeTrue();
        (duration1 <= duration2).Should().BeTrue();
        (duration2 >= duration1).Should().BeTrue();
    }
}