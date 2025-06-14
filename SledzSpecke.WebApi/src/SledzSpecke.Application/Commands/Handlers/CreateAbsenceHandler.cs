using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class CreateAbsenceHandler : ICommandHandler<CreateAbsence>
{
    private readonly IAbsenceRepository _absenceRepository;

    public CreateAbsenceHandler(IAbsenceRepository absenceRepository)
    {
        _absenceRepository = absenceRepository;
    }

    public async Task HandleAsync(CreateAbsence command)
    {
        // Check for overlapping absences
        var hasOverlapping = await _absenceRepository.HasOverlappingAbsencesAsync(
            command.UserId, command.StartDate, command.EndDate);

        if (hasOverlapping)
        {
            throw new InvalidOperationException("Absence period overlaps with existing absence.");
        }

        var absence = Absence.Create(
            AbsenceId.New(),
            command.SpecializationId,
            command.UserId,
            command.Type,
            command.StartDate,
            command.EndDate,
            command.Description);

        await _absenceRepository.AddAsync(absence);
    }
}