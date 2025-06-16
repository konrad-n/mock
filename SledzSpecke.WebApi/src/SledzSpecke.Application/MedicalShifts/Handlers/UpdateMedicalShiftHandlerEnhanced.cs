using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

/// <summary>
/// Enhanced version of UpdateMedicalShiftHandler using Result pattern
/// This demonstrates how to gradually migrate to MySpot patterns
/// </summary>
public class UpdateMedicalShiftHandlerEnhanced : ICommandHandler<UpdateMedicalShift>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IUserContextService _userContextService;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMedicalShiftHandlerEnhanced(
        IMedicalShiftRepository medicalShiftRepository,
        IUserContextService userContextService,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _userContextService = userContextService;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(UpdateMedicalShift command)
    {
        // Step 1: Validate and load entities
        var validationResult = await ValidateAndLoadEntitiesAsync(command);
        if (validationResult.IsFailure)
        {
            throw new ValidationException(validationResult.Error);
        }

        var (shift, internship, user, specialization) = validationResult.Value;

        // Step 2: Check business rules
        var authorizationResult = CheckAuthorization(user, internship);
        if (authorizationResult.IsFailure)
        {
            throw new UnauthorizedException(authorizationResult.Error);
        }

        var modificationResult = CheckCanModify(shift);
        if (modificationResult.IsFailure)
        {
            throw new BusinessRuleException(modificationResult.Error);
        }

        // Step 3: Validate input values
        var inputValidationResult = ValidateInputValues(command);
        if (inputValidationResult.IsFailure)
        {
            throw new ValidationException(inputValidationResult.Error);
        }

        // Step 4: Check date change rule
        if (command.Date.HasValue && command.Date.Value != shift.Date)
        {
            throw new BusinessRuleException("Cannot change the date of a medical shift. Please delete and create a new shift instead.");
        }

        // Step 5: Update the shift
        var hours = command.Hours ?? shift.Hours;
        var minutes = command.Minutes ?? shift.Minutes;
        var location = !string.IsNullOrWhiteSpace(command.Location) ? command.Location : shift.Location;

        shift.UpdateShiftDetails(hours, minutes, location);

        // Step 6: Validate final state
        var finalValidationResult = ValidateFinalState(shift);
        if (finalValidationResult.IsFailure)
        {
            throw new ValidationException(finalValidationResult.Error);
        }

        // Step 7: Persist changes using Unit of Work
        await _medicalShiftRepository.UpdateAsync(shift);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Result<(Core.Entities.MedicalShift shift, Core.Entities.Internship internship,
        Core.Entities.User user, Core.Entities.Specialization specialization)>> ValidateAndLoadEntitiesAsync(UpdateMedicalShift command)
    {
        var userId = _userContextService.GetUserId();

        var shift = await _medicalShiftRepository.GetByIdAsync(command.ShiftId);
        if (shift == null)
        {
            return Result.Failure<(Core.Entities.MedicalShift, Core.Entities.Internship, Core.Entities.User, Core.Entities.Specialization)>(
                $"Medical shift with ID {command.ShiftId} not found.");
        }

        var internship = await _internshipRepository.GetByIdAsync(shift.InternshipId);
        if (internship == null)
        {
            return Result.Failure<(Core.Entities.MedicalShift, Core.Entities.Internship, Core.Entities.User, Core.Entities.Specialization)>(
                $"Internship with ID {shift.InternshipId} not found.");
        }

        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user == null)
        {
            return Result.Failure<(Core.Entities.MedicalShift, Core.Entities.Internship, Core.Entities.User, Core.Entities.Specialization)>(
                "User not found.");
        }

        var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
        if (specialization == null)
        {
            return Result.Failure<(Core.Entities.MedicalShift, Core.Entities.Internship, Core.Entities.User, Core.Entities.Specialization)>(
                $"Specialization not found for internship {shift.InternshipId}.");
        }

        return Result.Success<(Core.Entities.MedicalShift, Core.Entities.Internship, Core.Entities.User, Core.Entities.Specialization)>(
            (shift, internship, user, specialization));
    }

    private Result CheckAuthorization(Core.Entities.User user, Core.Entities.Internship internship)
    {
        // TODO: User-Specialization relationship needs to be redesigned
        // if (user.SpecializationId != internship.SpecializationId)
        // {
        //     return Result.Failure("You are not authorized to update this medical shift.");
        // }

        return Result.Success();
    }

    private Result CheckCanModify(Core.Entities.MedicalShift shift)
    {
        if (shift.IsApproved)
        {
            return Result.Failure("Cannot update an approved medical shift.");
        }

        return Result.Success();
    }

    private Result ValidateInputValues(UpdateMedicalShift command)
    {
        if (command.Hours.HasValue && command.Hours.Value < 0)
        {
            return Result.Failure("Hours cannot be negative.");
        }

        // MAUI allows minutes > 59, normalization happens at summary level
        if (command.Minutes.HasValue && command.Minutes.Value < 0)
        {
            return Result.Failure("Minutes cannot be negative.");
        }

        return Result.Success();
    }

    private Result ValidateFinalState(Core.Entities.MedicalShift shift)
    {
        // MAUI implementation only validates that duration is greater than zero
        // No maximum duration limits are enforced
        var totalMinutes = shift.TotalMinutes;
        if (totalMinutes == 0)
        {
            return Result.Failure("Medical shift duration must be greater than zero.");
        }

        return Result.Success();
    }
}