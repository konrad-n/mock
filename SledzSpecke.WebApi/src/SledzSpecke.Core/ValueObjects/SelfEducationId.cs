namespace SledzSpecke.Core.ValueObjects;

public record SelfEducationId(Guid Value)
{
    public static SelfEducationId New() => new(Guid.NewGuid());
    public static implicit operator Guid(SelfEducationId selfEducationId) => selfEducationId.Value;
    public static implicit operator SelfEducationId(Guid value) => new(value);
    
    public override string ToString() => Value.ToString();
}