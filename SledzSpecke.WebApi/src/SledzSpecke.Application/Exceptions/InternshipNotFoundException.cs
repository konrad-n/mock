using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class InternshipNotFoundException : CustomException
{
    public int InternshipId { get; }

    public InternshipNotFoundException(int internshipId) 
        : base($"Internship with ID {internshipId} was not found.")
    {
        InternshipId = internshipId;
    }
}