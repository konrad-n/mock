namespace SledzSpecke.Core.ValueObjects;

public record PublicationId(Guid Value)
{
    public static PublicationId New() => new(Guid.NewGuid());
    public static implicit operator Guid(PublicationId publicationId) => publicationId.Value;
    public static implicit operator PublicationId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}