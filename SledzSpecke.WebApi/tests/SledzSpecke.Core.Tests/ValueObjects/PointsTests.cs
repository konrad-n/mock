using FluentAssertions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;
using Xunit;

namespace SledzSpecke.Core.Tests.ValueObjects;

public class PointsTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(-0.1)]
    [InlineData(-100)]
    public void Points_WithNegativeValue_ShouldThrowException(decimal value)
    {
        // Arrange & Act
        var act = () => new Points(value);

        // Assert
        act.Should().Throw<InvalidPointsException>()
            .WithMessage($"Points value '{value}' is invalid. Must be between 0 and 1000.");
    }

    [Theory]
    [InlineData(1000.1)]
    [InlineData(1001)]
    [InlineData(9999)]
    public void Points_WithValueGreaterThan1000_ShouldThrowException(decimal value)
    {
        // Arrange & Act
        var act = () => new Points(value);

        // Assert
        act.Should().Throw<InvalidPointsException>()
            .WithMessage($"Points value '{value}' is invalid. Must be between 0 and 1000.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.5)]
    [InlineData(10)]
    [InlineData(100.75)]
    [InlineData(1000)]
    public void Points_WithValidValue_ShouldCreateInstance(decimal value)
    {
        // Arrange & Act
        var points = new Points(value);

        // Assert
        points.Value.Should().Be(value);
    }

    [Fact]
    public void Points_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var points1 = new Points(10.5m);
        var points2 = new Points(10.5m);
        var points3 = new Points(10.6m);

        // Act & Assert
        points1.Should().Be(points2);
        points1.Should().NotBe(points3);
        (points1 == points2).Should().BeTrue();
        (points1 != points3).Should().BeTrue();
    }

    [Fact]
    public void Points_ExplicitOperator_FromDecimal_ShouldWorkCorrectly()
    {
        // Arrange
        decimal value = 50.5m;

        // Act
        var points = (Points)value;

        // Assert
        points.Value.Should().Be(50.5m);
    }

    [Fact]
    public void Points_ExplicitOperator_FromDecimal_WithInvalidValue_ShouldThrowException()
    {
        // Arrange
        decimal value = -10m;

        // Act
        var act = () => (Points)value;

        // Assert
        act.Should().Throw<InvalidPointsException>();
    }

    [Fact]
    public void Points_Addition_ShouldWorkCorrectly()
    {
        // Arrange
        var points1 = new Points(10.5m);
        var points2 = new Points(20.3m);

        // Act
        var result = points1 + points2;

        // Assert
        result.Value.Should().Be(30.8m);
    }

    [Fact]
    public void Points_Addition_ExceedingMaxValue_ShouldThrowException()
    {
        // Arrange
        var points1 = new Points(800m);
        var points2 = new Points(300m);

        // Act
        var act = () => points1 + points2;

        // Assert
        act.Should().Throw<InvalidPointsException>()
            .WithMessage("Points value '1100' is invalid. Must be between 0 and 1000.");
    }

    [Fact]
    public void Points_Subtraction_ShouldWorkCorrectly()
    {
        // Arrange
        var points1 = new Points(50.5m);
        var points2 = new Points(20.3m);

        // Act
        var result = points1 - points2;

        // Assert
        result.Value.Should().Be(30.2m);
    }

    [Fact]
    public void Points_Subtraction_ResultingInNegative_ShouldThrowException()
    {
        // Arrange
        var points1 = new Points(20m);
        var points2 = new Points(30m);

        // Act
        var act = () => points1 - points2;

        // Assert
        act.Should().Throw<InvalidPointsException>()
            .WithMessage("Points value '-10' is invalid. Must be between 0 and 1000.");
    }

    [Fact]
    public void Points_ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var points1 = new Points(10m);
        var points2 = new Points(10.5m);
        var points3 = new Points(0m);

        // Act & Assert
        // Records have a default ToString that includes all properties
        points1.ToString().Should().Contain("Value = 10");
        points2.ToString().Should().Contain("Value = 10.5");
        points3.ToString().Should().Contain("Value = 0");
    }

    [Fact]
    public void Points_ImplicitOperator_ToDecimal_ShouldWorkCorrectly()
    {
        // Arrange
        var points = new Points(50.5m);

        // Act
        decimal value = points;

        // Assert
        value.Should().Be(50.5m);
    }

    [Fact]
    public void Points_ComparisonOperators_ShouldWorkCorrectly()
    {
        // Arrange
        var points1 = new Points(10m);
        var points2 = new Points(20m);
        var points3 = new Points(10m);

        // Act & Assert
        (points1 < points2).Should().BeTrue();
        (points2 > points1).Should().BeTrue();
        (points1 <= points3).Should().BeTrue();
        (points1 >= points3).Should().BeTrue();
        (points1 <= points2).Should().BeTrue();
        (points2 >= points1).Should().BeTrue();
    }
}