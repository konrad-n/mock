using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record JournalName
{
    public string Value { get; }

    public JournalName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidJournalNameException("Journal name cannot be empty.");
        }

        if (value.Length > 300)
        {
            throw new InvalidJournalNameException("Journal name cannot exceed 300 characters.");
        }

        if (value.Length < 2)
        {
            throw new InvalidJournalNameException("Journal name must be at least 2 characters long.");
        }

        Value = value.Trim();
    }

    public static implicit operator string(JournalName name) => name.Value;
    public static implicit operator JournalName(string name) => new(name);
    
    public override string ToString() => Value;
}