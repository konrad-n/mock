using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record PublicationTitle
{
    public string Value { get; }

    public PublicationTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPublicationTitleException("Publication title cannot be empty.");
        }

        if (value.Length > 500)
        {
            throw new InvalidPublicationTitleException("Publication title cannot exceed 500 characters.");
        }

        if (value.Length < 5)
        {
            throw new InvalidPublicationTitleException("Publication title must be at least 5 characters long.");
        }

        Value = value.Trim();
    }

    public static implicit operator string(PublicationTitle title) => title.Value;
    public static implicit operator PublicationTitle(string title) => new(title);
    
    public override string ToString() => Value;
}