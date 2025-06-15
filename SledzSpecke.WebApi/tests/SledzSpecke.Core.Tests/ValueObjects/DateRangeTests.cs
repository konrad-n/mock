using FluentAssertions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;
using Xunit;

namespace SledzSpecke.Core.Tests.ValueObjects;

public class DateRangeTests
{
    [Fact]
    public void DateRange_WithEndDateBeforeStartDate_ShouldThrowException()
    {
        // Arrange
        var startDate = new DateTime(2025, 6, 15);
        var endDate = new DateTime(2025, 6, 14);

        // Act
        var act = () => new DateRange(startDate, endDate);

        // Assert
        act.Should().Throw<InvalidDateRangeException>();
    }

    [Fact]
    public void DateRange_WithEqualDates_ShouldCreateInstance()
    {
        // Arrange
        var date = new DateTime(2025, 6, 15);

        // Act
        var dateRange = new DateRange(date, date);

        // Assert
        dateRange.StartDate.Should().Be(date);
        dateRange.EndDate.Should().Be(date);
        dateRange.TotalDays.Should().Be(1);
    }

    [Theory]
    [InlineData("2025-06-01", "2025-06-01", 1)]
    [InlineData("2025-06-01", "2025-06-02", 2)]
    [InlineData("2025-06-01", "2025-06-30", 30)]
    [InlineData("2025-01-01", "2025-12-31", 365)]
    public void DateRange_Days_ShouldCalculateCorrectly(string start, string end, int expectedDays)
    {
        // Arrange
        var startDate = DateTime.Parse(start);
        var endDate = DateTime.Parse(end);

        // Act
        var dateRange = new DateRange(startDate, endDate);

        // Assert
        dateRange.TotalDays.Should().Be(expectedDays);
    }

    [Fact]
    public void DateRange_Contains_ShouldReturnTrueForDateWithinRange()
    {
        // Arrange
        var dateRange = new DateRange(
            new DateTime(2025, 6, 1),
            new DateTime(2025, 6, 30)
        );

        // Act & Assert
        dateRange.Contains(new DateTime(2025, 6, 1)).Should().BeTrue();
        dateRange.Contains(new DateTime(2025, 6, 15)).Should().BeTrue();
        dateRange.Contains(new DateTime(2025, 6, 30)).Should().BeTrue();
    }

    [Fact]
    public void DateRange_Contains_ShouldReturnFalseForDateOutsideRange()
    {
        // Arrange
        var dateRange = new DateRange(
            new DateTime(2025, 6, 1),
            new DateTime(2025, 6, 30)
        );

        // Act & Assert
        dateRange.Contains(new DateTime(2025, 5, 31)).Should().BeFalse();
        dateRange.Contains(new DateTime(2025, 7, 1)).Should().BeFalse();
    }

    [Theory]
    [InlineData("2025-06-01", "2025-06-10", "2025-06-05", "2025-06-15", true)]  // Partial overlap
    [InlineData("2025-06-01", "2025-06-10", "2025-06-10", "2025-06-15", true)]  // Touch at boundary
    [InlineData("2025-06-01", "2025-06-10", "2025-06-11", "2025-06-15", false)] // No overlap
    [InlineData("2025-06-05", "2025-06-15", "2025-06-01", "2025-06-10", true)]  // Partial overlap (reversed)
    [InlineData("2025-06-01", "2025-06-30", "2025-06-10", "2025-06-20", true)]  // Fully contained
    [InlineData("2025-06-10", "2025-06-20", "2025-06-01", "2025-06-30", true)]  // Contains other
    public void DateRange_Overlaps_ShouldDetectCorrectly(
        string start1, string end1, string start2, string end2, bool expectedOverlap)
    {
        // Arrange
        var range1 = new DateRange(DateTime.Parse(start1), DateTime.Parse(end1));
        var range2 = new DateRange(DateTime.Parse(start2), DateTime.Parse(end2));

        // Act & Assert
        range1.Overlaps(range2).Should().Be(expectedOverlap);
        range2.Overlaps(range1).Should().Be(expectedOverlap); // Should be symmetric
    }

    [Fact]
    public void DateRange_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var range1 = new DateRange(
            new DateTime(2025, 6, 1),
            new DateTime(2025, 6, 30)
        );
        var range2 = new DateRange(
            new DateTime(2025, 6, 1),
            new DateTime(2025, 6, 30)
        );
        var range3 = new DateRange(
            new DateTime(2025, 6, 1),
            new DateTime(2025, 6, 29)
        );

        // Act & Assert
        range1.Should().Be(range2);
        range1.Should().NotBe(range3);
        (range1 == range2).Should().BeTrue();
        (range1 != range3).Should().BeTrue();
    }

    [Fact]
    public void DateRange_CreateForMonth_ShouldCreateCorrectRange()
    {
        // Act
        var januaryRange = DateRange.CreateMonthRange(2025, 1);
        var februaryRange = DateRange.CreateMonthRange(2025, 2);
        var leapFebruaryRange = DateRange.CreateMonthRange(2024, 2);

        // Assert
        januaryRange.StartDate.Should().Be(new DateTime(2025, 1, 1));
        januaryRange.EndDate.Should().Be(new DateTime(2025, 1, 31));
        januaryRange.TotalDays.Should().Be(31);

        februaryRange.StartDate.Should().Be(new DateTime(2025, 2, 1));
        februaryRange.EndDate.Should().Be(new DateTime(2025, 2, 28));
        februaryRange.TotalDays.Should().Be(28);

        leapFebruaryRange.StartDate.Should().Be(new DateTime(2024, 2, 1));
        leapFebruaryRange.EndDate.Should().Be(new DateTime(2024, 2, 29));
        leapFebruaryRange.TotalDays.Should().Be(29);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(-1)]
    public void DateRange_CreateForMonth_WithInvalidMonth_ShouldThrowException(int month)
    {
        // Act
        var act = () => DateRange.CreateMonthRange(2025, month);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void DateRange_CreateForYear_ShouldCreateCorrectRange()
    {
        // Act
        var yearRange = DateRange.CreateYearRange(2025);
        var leapYearRange = DateRange.CreateYearRange(2024);

        // Assert
        yearRange.StartDate.Should().Be(new DateTime(2025, 1, 1));
        yearRange.EndDate.Should().Be(new DateTime(2025, 12, 31));
        yearRange.TotalDays.Should().Be(365);

        leapYearRange.StartDate.Should().Be(new DateTime(2024, 1, 1));
        leapYearRange.EndDate.Should().Be(new DateTime(2024, 12, 31));
        leapYearRange.TotalDays.Should().Be(366);
    }

    [Fact]
    public void DateRange_ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var dateRange = new DateRange(
            new DateTime(2025, 6, 1),
            new DateTime(2025, 6, 30)
        );

        // Act
        var result = dateRange.ToString();

        // Assert
        // Records have a default ToString that includes all properties
        result.Should().Contain("StartDate = 06/01/2025");
        result.Should().Contain("EndDate = 06/30/2025");
    }
}