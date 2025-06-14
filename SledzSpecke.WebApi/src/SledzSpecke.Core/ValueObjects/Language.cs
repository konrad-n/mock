using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record Language
{
    private static readonly HashSet<string> SupportedLanguages = new()
    {
        "en", // English
        "pl", // Polish
        "de", // German
        "es", // Spanish
        "fr"  // French
    };

    public string Value { get; }

    public Language(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidLanguageException("Language code cannot be empty.");
        }

        value = value.Trim().ToLowerInvariant();

        if (value.Length != 2)
        {
            throw new InvalidLanguageException("Language code must be 2 characters (ISO 639-1).");
        }

        if (!SupportedLanguages.Contains(value))
        {
            throw new InvalidLanguageException(
                $"Language '{value}' is not supported. Supported languages: {string.Join(", ", SupportedLanguages)}");
        }

        Value = value;
    }

    public static implicit operator string(Language language) => language.Value;
    public static implicit operator Language(string language) => new(language);
    
    public override string ToString() => Value;

    public string GetDisplayName() => Value switch
    {
        "en" => "English",
        "pl" => "Polski",
        "de" => "Deutsch",
        "es" => "Español",
        "fr" => "Français",
        _ => Value
    };

    // Common language instances
    public static Language English => new("en");
    public static Language Polish => new("pl");
    public static Language German => new("de");
    public static Language Spanish => new("es");
    public static Language French => new("fr");
}