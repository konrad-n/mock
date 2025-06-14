namespace SledzSpecke.Core.ValueObjects;

public record RecognitionId(Guid Value)
{
    public static RecognitionId New() => new(Guid.NewGuid());
    public static implicit operator Guid(RecognitionId recognitionId) => recognitionId.Value;
    public static implicit operator RecognitionId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}