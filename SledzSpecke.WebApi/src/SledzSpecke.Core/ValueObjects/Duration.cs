using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record Duration
{
    public int Hours { get; }
    public int Minutes { get; }
    public int TotalMinutes => Hours * 60 + Minutes;

    public Duration(int hours, int minutes)
    {
        if (hours < 0)
            throw new DomainException("Hours cannot be negative.");

        if (minutes < 0)
            throw new DomainException("Minutes cannot be negative.");

        // Note: Following MAUI implementation, we allow minutes > 59
        // Normalization happens at the summary/display level
        Hours = hours;
        Minutes = minutes;

        if (TotalMinutes == 0)
            throw new DomainException("Duration must be greater than zero.");
    }

    public static Duration FromMinutes(int totalMinutes)
    {
        if (totalMinutes <= 0)
            throw new DomainException("Total minutes must be greater than zero.");

        return new Duration(totalMinutes / 60, totalMinutes % 60);
    }

    public Duration Add(Duration other)
    {
        var totalMinutes = TotalMinutes + other.TotalMinutes;
        return FromMinutes(totalMinutes);
    }

    public override string ToString()
    {
        return Minutes > 0 ? $"{Hours}h {Minutes}m" : $"{Hours}h";
    }

    // Comparison operators
    public static bool operator >(Duration left, Duration right) => left.TotalMinutes > right.TotalMinutes;
    public static bool operator <(Duration left, Duration right) => left.TotalMinutes < right.TotalMinutes;
    public static bool operator >=(Duration left, Duration right) => left.TotalMinutes >= right.TotalMinutes;
    public static bool operator <=(Duration left, Duration right) => left.TotalMinutes <= right.TotalMinutes;
}