using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class RecognitionNotFoundException : NotFoundException
{
    public RecognitionNotFoundException(Guid recognitionId) 
        : base($"Recognition with ID {recognitionId} was not found.")
    {
        RecognitionId = recognitionId;
    }

    public Guid RecognitionId { get; }
}