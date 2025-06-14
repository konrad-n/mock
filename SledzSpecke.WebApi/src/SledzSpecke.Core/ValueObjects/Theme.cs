using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record Theme
{
    private static readonly HashSet<string> SupportedThemes = new()
    {
        "light",
        "dark",
        "auto"
    };

    public string Value { get; }

    public Theme(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidThemeException("Theme cannot be empty.");
        }

        value = value.Trim().ToLowerInvariant();

        if (!SupportedThemes.Contains(value))
        {
            throw new InvalidThemeException(
                $"Theme '{value}' is not supported. Supported themes: {string.Join(", ", SupportedThemes)}");
        }

        Value = value;
    }

    public static implicit operator string(Theme theme) => theme.Value;
    public static implicit operator Theme(string theme) => new(theme);
    
    public override string ToString() => Value;

    public string GetDisplayName() => Value switch
    {
        "light" => "Light",
        "dark" => "Dark",
        "auto" => "Auto (System)",
        _ => Value
    };

    // Common theme instances
    public static Theme Light => new("light");
    public static Theme Dark => new("dark");
    public static Theme Auto => new("auto");
}