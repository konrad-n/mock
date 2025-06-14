using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class DeleteAbsenceHandler : ICommandHandler<DeleteAbsence>
{
    private readonly IAbsenceRepository _absenceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public DeleteAbsenceHandler(
        IAbsenceRepository absenceRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _absenceRepository = absenceRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(DeleteAbsence command)
    {
        var absence = await _absenceRepository.GetByIdAsync(command.AbsenceId);
        if (absence is null)
        {
            throw new AbsenceNotFoundException(command.AbsenceId.Value);
        }

        var currentUserId = _userContextService.GetUserId();
        if (absence.UserId.Value != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only delete your own absences.");
        }

        if (absence.IsApproved)
        {
            throw new InvalidOperationException("Cannot delete an approved absence.");
        }

        await _absenceRepository.DeleteAsync(absence.Id);
        await _unitOfWork.SaveChangesAsync();
    }
}