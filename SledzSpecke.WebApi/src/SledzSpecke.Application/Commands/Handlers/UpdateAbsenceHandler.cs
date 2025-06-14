using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class UpdateAbsenceHandler : ICommandHandler<UpdateAbsence>
{
    private readonly IAbsenceRepository _absenceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateAbsenceHandler(
        IAbsenceRepository absenceRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _absenceRepository = absenceRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(UpdateAbsence command)
    {
        var absence = await _absenceRepository.GetByIdAsync(command.AbsenceId);
        if (absence is null)
        {
            throw new AbsenceNotFoundException(command.AbsenceId.Value);
        }

        var currentUserId = _userContextService.GetUserId();
        if (absence.UserId.Value != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only update your own absences.");
        }

        absence.UpdateDetails(
            command.Type,
            command.StartDate,
            command.EndDate,
            command.Description);

        await _absenceRepository.UpdateAsync(absence);
        await _unitOfWork.SaveChangesAsync();
    }
}