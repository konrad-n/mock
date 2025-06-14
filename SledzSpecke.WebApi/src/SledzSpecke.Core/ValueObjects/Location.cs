using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record Location
{
    public string Value { get; }

    public Location(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Location cannot be empty.");

        if (value.Length > 200)
            throw new DomainException("Location is too long. Maximum length is 200 characters.");

        Value = value.Trim();
    }

    public static implicit operator string(Location location) => location.Value;
    public static implicit operator Location(string location) => new(location);

    public override string ToString() => Value;
}