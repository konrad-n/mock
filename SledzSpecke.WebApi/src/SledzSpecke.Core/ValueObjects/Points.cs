using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record Points
{
    public decimal Value { get; }

    public Points(decimal value)
    {
        if (value < 0)
            throw new InvalidPointsException(value);

        if (value > 1000)
            throw new InvalidPointsException(value);

        Value = value;
    }

    public static Points Zero => new Points(0);

    public static Points operator +(Points left, Points right)
        => new Points(left.Value + right.Value);

    public static Points operator -(Points left, Points right)
        => new Points(left.Value - right.Value);

    public static bool operator >(Points left, Points right)
        => left.Value > right.Value;

    public static bool operator <(Points left, Points right)
        => left.Value < right.Value;

    public static bool operator >=(Points left, Points right)
        => left.Value >= right.Value;

    public static bool operator <=(Points left, Points right)
        => left.Value <= right.Value;

    public static implicit operator decimal(Points points) => points.Value;
    public static explicit operator Points(decimal value) => new Points(value);
}