using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record DateRange
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new InvalidDateRangeException($"Invalid date range: start date '{startDate:yyyy-MM-dd}' cannot be after end date '{endDate:yyyy-MM-dd}'.");

        StartDate = startDate.Date; // Normalize to date only
        EndDate = endDate.Date;
    }

    public int TotalDays => (int)(EndDate - StartDate).TotalDays + 1;
    
    public bool Contains(DateTime date)
    {
        var normalizedDate = date.Date;
        return normalizedDate >= StartDate && normalizedDate <= EndDate;
    }

    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    public DateRange? GetOverlap(DateRange other)
    {
        if (!Overlaps(other))
            return null;

        var overlapStart = StartDate > other.StartDate ? StartDate : other.StartDate;
        var overlapEnd = EndDate < other.EndDate ? EndDate : other.EndDate;

        return new DateRange(overlapStart, overlapEnd);
    }

    public static DateRange CreateMonthRange(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return new DateRange(startDate, endDate);
    }

    public static DateRange CreateYearRange(int year)
    {
        var startDate = new DateTime(year, 1, 1);
        var endDate = new DateTime(year, 12, 31);
        return new DateRange(startDate, endDate);
    }
}