using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

/// <summary>
/// Enhanced version of DeleteMedicalShiftHandler using Result pattern
/// Demonstrates clean separation of concerns and improved error handling
/// </summary>
public class DeleteMedicalShiftHandlerEnhanced : ICommandHandler<DeleteMedicalShift>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IUserContextService _userContextService;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMedicalShiftHandlerEnhanced(
        IMedicalShiftRepository medicalShiftRepository,
        IUserContextService userContextService,
        IInternshipRepository internshipRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _userContextService = userContextService;
        _internshipRepository = internshipRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(DeleteMedicalShift command)
    {
        // Step 1: Load and validate entities
        var loadResult = await LoadAndValidateEntitiesAsync(command);
        if (loadResult.IsFailure)
        {
            throw new NotFoundException(loadResult.Error);
        }

        var (shift, internship, user) = loadResult.Value;

        // Step 2: Check authorization
        var authResult = CheckAuthorization(user, internship);
        if (authResult.IsFailure)
        {
            throw new UnauthorizedException(authResult.Error);
        }

        // Step 3: Check business rules
        var businessRuleResult = CheckDeleteRules(shift);
        if (businessRuleResult.IsFailure)
        {
            throw new BusinessRuleException(businessRuleResult.Error);
        }

        // Step 4: Delete the shift using Unit of Work
        await _medicalShiftRepository.DeleteAsync(shift.Id.Value);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Result<(Core.Entities.MedicalShift shift, Core.Entities.Internship internship, Core.Entities.User user)>>
        LoadAndValidateEntitiesAsync(DeleteMedicalShift command)
    {
        var userId = _userContextService.GetUserId();

        var shift = await _medicalShiftRepository.GetByIdAsync(command.ShiftId);
        if (shift == null)
        {
            return Result.Failure<(Core.Entities.MedicalShift, Core.Entities.Internship, Core.Entities.User)>(
                $"Medical shift with ID {command.ShiftId} not found.");
        }

        var internship = await _internshipRepository.GetByIdAsync(shift.InternshipId);
        if (internship == null)
        {
            return Result.Failure<(Core.Entities.MedicalShift, Core.Entities.Internship, Core.Entities.User)>(
                $"Internship with ID {shift.InternshipId} not found.");
        }

        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user == null)
        {
            return Result.Failure<(Core.Entities.MedicalShift, Core.Entities.Internship, Core.Entities.User)>(
                "User not found.");
        }

        return Result.Success((shift, internship, user));
    }

    private Result CheckAuthorization(Core.Entities.User user, Core.Entities.Internship internship)
    {
        // TODO: User-Specialization relationship needs to be redesigned
        // if (user.SpecializationId != internship.SpecializationId)
        // {
        //     return Result.Failure("You are not authorized to delete this medical shift.");
        // }

        return Result.Success();
    }

    private Result CheckDeleteRules(Core.Entities.MedicalShift shift)
    {
        if (shift.IsApproved)
        {
            return Result.Failure("Cannot delete an approved medical shift.");
        }

        if (shift.SyncStatus == SyncStatus.Synced)
        {
            return Result.Failure("Cannot delete a synced medical shift.");
        }

        return Result.Success();
    }
}