using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class CreateAbsenceHandler : IResultCommandHandler<CreateAbsence>
{
    private readonly IAbsenceRepository _absenceRepository;

    public CreateAbsenceHandler(IAbsenceRepository absenceRepository)
    {
        _absenceRepository = absenceRepository;
    }

    public async Task<Result> HandleAsync(CreateAbsence command, CancellationToken cancellationToken = default)
    {
        // Get existing absences for overlap check
        var existingAbsences = await _absenceRepository.GetByUserIdAsync(command.UserId);

        var result = Absence.CreateWithOverlapCheck(
            AbsenceId.New(),
            command.SpecializationId,
            command.UserId,
            command.Type,
            command.StartDate,
            command.EndDate,
            existingAbsences,
            command.Description);
            
        if (!result.IsSuccess)
        {
            return Result.Failure(result.Error, result.ErrorCode);
        }

        await _absenceRepository.AddAsync(result.Value);
        return Result.Success();
    }
}