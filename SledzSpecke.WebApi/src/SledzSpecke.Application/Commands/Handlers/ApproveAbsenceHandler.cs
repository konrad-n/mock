using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Constants;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class ApproveAbsenceHandler : IResultCommandHandler<ApproveAbsence>
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

    public async Task<Result> HandleAsync(ApproveAbsence command)
    {
        var absence = await _absenceRepository.GetByIdAsync(command.AbsenceId);
        if (absence is null)
        {
            return Result.Failure($"Absence with ID {command.AbsenceId.Value} not found.", ErrorCodes.ABSENCE_NOT_FOUND);
        }

        // Verify the current user has permission to approve
        // In a real system, you might check if the user is a supervisor or admin
        var currentUserId = _userContextService.GetUserId();

        try
        {
            absence.Approve(new Core.ValueObjects.UserId(command.ApprovedBy));

            await _absenceRepository.UpdateAsync(absence);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to approve absence: {ex.Message}", ErrorCodes.DOMAIN_ERROR);
        }
    }
}