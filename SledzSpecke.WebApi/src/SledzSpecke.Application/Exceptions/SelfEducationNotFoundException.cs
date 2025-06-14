using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class SelfEducationNotFoundException : NotFoundException
{
    public SelfEducationNotFoundException(Guid selfEducationId) 
        : base($"Self-education activity with ID {selfEducationId} was not found.")
    {
        SelfEducationId = selfEducationId;
    }

    public Guid SelfEducationId { get; }
}