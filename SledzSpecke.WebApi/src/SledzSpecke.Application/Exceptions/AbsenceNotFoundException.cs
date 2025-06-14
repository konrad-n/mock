using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class AbsenceNotFoundException : NotFoundException
{
    public AbsenceNotFoundException(Guid absenceId) 
        : base($"Absence with ID {absenceId} was not found.")
    {
        AbsenceId = absenceId;
    }

    public Guid AbsenceId { get; }
}