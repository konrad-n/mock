using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class ApproveAbsenceHandler : ICommandHandler<ApproveAbsence>
{
    private readonly IAbsenceRepository _absenceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public ApproveAbsenceHandler(
        IAbsenceRepository absenceRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _absenceRepository = absenceRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(ApproveAbsence command)
    {
        var absence = await _absenceRepository.GetByIdAsync(command.AbsenceId);
        if (absence is null)
        {
            throw new AbsenceNotFoundException(command.AbsenceId.Value);
        }

        // Verify the current user has permission to approve
        // In a real system, you might check if the user is a supervisor or admin
        var currentUserId = _userContextService.GetUserId();
        
        absence.Approve(new Core.ValueObjects.UserId(command.ApprovedBy));

        await _absenceRepository.UpdateAsync(absence);
        await _unitOfWork.SaveChangesAsync();
    }
}