using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record SmkVersion
{
    private static readonly string[] ValidVersions = { "old", "new" };

    public string Value { get; }

    public SmkVersion(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("SMK version cannot be empty.");

        value = value.ToLowerInvariant();

        if (!ValidVersions.Contains(value))
            throw new DomainException($"Invalid SMK version. Valid values are: {string.Join(", ", ValidVersions)}");

        Value = value;
    }

    public static SmkVersion Old => new("old");
    public static SmkVersion New => new("new");

    public bool IsOld => Value == "old";
    public bool IsNew => Value == "new";

    public static implicit operator string(SmkVersion version) => version.Value;
    public static implicit operator SmkVersion(string version) => new(version);

    public override string ToString() => Value;
}