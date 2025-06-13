using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class SpecializationNotFoundException : CustomException
{
    public int SpecializationId { get; }

    public SpecializationNotFoundException(int specializationId) 
        : base($"Specialization with ID {specializationId} was not found.")
    {
        SpecializationId = specializationId;
    }
}