using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

/// <summary>
/// Enhanced version of AddMedicalShiftHandler using Result pattern
/// Demonstrates gradual migration to MySpot patterns with improved error handling
/// </summary>
public class AddMedicalShiftHandlerEnhanced : ICommandHandler<AddMedicalShift, int>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ISpecializationValidationService _validationService;
    private readonly IYearCalculationService _yearCalculationService;
    private readonly IUnitOfWork _unitOfWork;

    public AddMedicalShiftHandlerEnhanced(
        IMedicalShiftRepository medicalShiftRepository,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        ISpecializationValidationService validationService,
        IYearCalculationService yearCalculationService,
        IUnitOfWork unitOfWork)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _validationService = validationService;
        _yearCalculationService = yearCalculationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> HandleAsync(AddMedicalShift command)
    {
        // Step 1: Validate and load entities
        var loadResult = await LoadRequiredEntitiesAsync(command);
        if (loadResult.IsFailure)
        {
            throw new ValidationException(loadResult.Error);
        }

        var (internship, specialization) = loadResult.Value;

        // Step 2: Validate command inputs
        var inputValidationResult = ValidateInputs(command);
        if (inputValidationResult.IsFailure)
        {
            throw new ValidationException(inputValidationResult.Error);
        }

        // Step 3: Validate SMK version specific rules
        var smkValidationResult = ValidateForSmkVersion(command, specialization);
        if (smkValidationResult.IsFailure)
        {
            throw new BusinessRuleException(smkValidationResult.Error);
        }

        // Step 4: Validate date range for New SMK
        if (specialization.SmkVersion == SmkVersion.New)
        {
            var dateValidationResult = ValidateDateRange(command, internship);
            if (dateValidationResult.IsFailure)
            {
                throw new InvalidDateRangeException(dateValidationResult.Error);
            }
        }

        // Step 5: Create medical shift
        var createResult = CreateMedicalShift(command, internship);
        if (createResult.IsFailure)
        {
            throw new InvalidOperationException(createResult.Error);
        }

        var medicalShift = createResult.Value;

        // Step 6: Validate using template service
        var templateValidationResult = await ValidateWithTemplateAsync(medicalShift, specialization);
        if (templateValidationResult.IsFailure)
        {
            throw new InvalidOperationException(templateValidationResult.Error);
        }

        // Step 7: Save the medical shift using Unit of Work
        var shiftId = await _medicalShiftRepository.AddAsync(medicalShift);
        await _unitOfWork.SaveChangesAsync();

        return shiftId;
    }

    private async Task<Result<(Internship internship, Specialization specialization)>>
        LoadRequiredEntitiesAsync(AddMedicalShift command)
    {
        var internship = await _internshipRepository.GetByIdAsync(new InternshipId(command.InternshipId));
        if (internship is null)
        {
            return Result.Failure<(Internship, Specialization)>(
                $"Internship with ID {command.InternshipId} not found.");
        }

        var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
        if (specialization is null)
        {
            return Result.Failure<(Internship, Specialization)>(
                $"Specialization with ID {internship.SpecializationId.Value} not found.");
        }

        return Result.Success((internship, specialization));
    }

    private Result ValidateInputs(AddMedicalShift command)
    {
        if (command.Hours < 0)
        {
            return Result.Failure("Hours cannot be negative.");
        }

        if (command.Minutes < 0)
        {
            return Result.Failure("Minutes cannot be negative.");
        }

        // AI HINT: MAUI allows minutes > 59 (e.g., 90 minutes is valid)
        // DO NOT add validation like "if (command.Minutes > 59)" - this is intentional!
        // Normalization happens at display/summary level via TimeNormalizationHelper

        if (string.IsNullOrWhiteSpace(command.Location))
        {
            return Result.Failure("Location is required.");
        }

        if (command.Location.Length > 100)
        {
            return Result.Failure("Location name cannot exceed 100 characters.");
        }

        return Result.Success();
    }

    private Result ValidateForSmkVersion(AddMedicalShift command, Specialization specialization)
    {
        if (specialization.SmkVersion.IsOld)
        {
            return ValidateOldSmkShift(command, specialization);
        }
        else if (specialization.SmkVersion.IsNew)
        {
            return ValidateNewSmkShift(command, specialization);
        }
        else
        {
            return Result.Failure($"Unknown SMK version: {specialization.SmkVersion}");
        }
    }

    private Result ValidateOldSmkShift(AddMedicalShift command, Specialization specialization)
    {
        var availableYears = _yearCalculationService.GetAvailableYears(specialization);

        // Allow year 0 for unassigned shifts
        if (command.Year != 0 && !availableYears.Contains(command.Year))
        {
            return Result.Failure(
                $"Year must be 0 (unassigned) or between {availableYears.Min()} and {availableYears.Max()} for this specialization.");
        }

        // MAUI implementation does not enforce maximum shift duration
        // Only check that total duration is greater than zero
        if (command.Hours == 0 && command.Minutes == 0)
        {
            return Result.Failure("Shift duration must be greater than zero.");
        }

        return Result.Success();
    }

    private Result ValidateNewSmkShift(AddMedicalShift command, Specialization specialization)
    {
        if (command.Year <= 0)
        {
            return Result.Failure("Year must be provided for New SMK.");
        }

        // MAUI implementation does not enforce maximum shift duration
        // Only check that total duration is greater than zero
        if (command.Hours == 0 && command.Minutes == 0)
        {
            return Result.Failure("Shift duration must be greater than zero.");
        }

        return Result.Success();
    }

    private Result ValidateDateRange(AddMedicalShift command, Internship internship)
    {
        if (command.Date < internship.StartDate || command.Date > internship.EndDate)
        {
            return Result.Failure(
                "Medical shift date must be within the internship period for New SMK.");
        }

        return Result.Success();
    }

    private Result<MedicalShift> CreateMedicalShift(AddMedicalShift command, Internship internship)
    {
        try
        {
            var medicalShiftId = new MedicalShiftId(0); // Will be assigned by repository
            var medicalShift = MedicalShift.Create(
                medicalShiftId,
                internship.Id,
                command.Date,
                command.Hours,
                command.Minutes,
                command.Location,
                command.Year
            );

            return Result.Success(medicalShift);
        }
        catch (Exception ex)
        {
            return Result.Failure<MedicalShift>($"Failed to create medical shift: {ex.Message}");
        }
    }

    private async Task<Result> ValidateWithTemplateAsync(MedicalShift medicalShift, Specialization specialization)
    {
        var validationResult = await _validationService.ValidateMedicalShiftAsync(
            medicalShift,
            specialization.Id.Value);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors);
            return Result.Failure($"Medical shift validation failed: {errors}");
        }

        return Result.Success();
    }
}