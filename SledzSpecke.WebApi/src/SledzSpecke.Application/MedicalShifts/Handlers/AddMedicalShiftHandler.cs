using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

public class AddMedicalShiftHandler : ICommandHandler<AddMedicalShift, int>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ISpecializationValidationService _validationService;
    private readonly IYearCalculationService _yearCalculationService;

    public AddMedicalShiftHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        ISpecializationValidationService validationService,
        IYearCalculationService yearCalculationService)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _validationService = validationService;
        _yearCalculationService = yearCalculationService;
    }

    public async Task<int> HandleAsync(AddMedicalShift command)
    {
        // Validate internship exists
        var internship = await _internshipRepository.GetByIdAsync(new InternshipId(command.InternshipId));
        if (internship is null)
        {
            throw new InternshipNotFoundException(command.InternshipId);
        }

        // Get specialization to determine SMK version
        var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
        if (specialization is null)
        {
            throw new SpecializationNotFoundException(internship.SpecializationId.Value);
        }

        // Validate shift based on SMK version and year calculation
        ValidateShiftForSmkVersion(command, specialization);

        // Create medical shift
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

        // Additional validation for date range based on internship
        if (specialization.SmkVersion == SmkVersion.New)
        {
            // For New SMK, medical shifts should be within internship date range
            if (command.Date < internship.StartDate || command.Date > internship.EndDate)
            {
                throw new InvalidDateRangeException(
                    "Medical shift date must be within the internship period for New SMK.");
            }
        }

        // Validate using template service before saving
        var validationResult = await _validationService.ValidateMedicalShiftAsync(medicalShift, specialization.Id.Value);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Medical shift validation failed: {string.Join(", ", validationResult.Errors)}");
        }

        // Save the medical shift
        var shiftId = await _medicalShiftRepository.AddAsync(medicalShift);

        return shiftId;
    }

    private void ValidateShiftForSmkVersion(AddMedicalShift command, Specialization specialization)
    {
        // Common validations - align with MAUI implementation
        // Only check that duration is greater than zero (hours + minutes > 0)
        if (command.Hours < 0 || command.Minutes < 0)
        {
            throw new ArgumentException("Shift duration cannot be negative.");
        }

        // AI HINT: MAUI allows minutes > 59 (e.g., 90 minutes is valid)
        // DO NOT add validation like "if (command.Minutes > 59)" - this is intentional!
        // Normalization happens at display/summary level via TimeNormalizationHelper
        // Previous implementations incorrectly restricted this

        if (string.IsNullOrWhiteSpace(command.Location))
        {
            throw new ArgumentException("Location is required.");
        }

        // SMK version-specific validations
        if (specialization.SmkVersion.IsOld)
        {
            ValidateOldSmkShift(command, specialization);
        }
        else if (specialization.SmkVersion.IsNew)
        {
            ValidateNewSmkShift(command, specialization);
        }
        else
        {
            throw new InvalidOperationException($"Unknown SMK version: {specialization.SmkVersion}");
        }
    }

    private void ValidateOldSmkShift(AddMedicalShift command, Specialization specialization)
    {
        // Old SMK specific validations
        var availableYears = _yearCalculationService.GetAvailableYears(specialization);

        // Allow year 0 for unassigned shifts
        if (command.Year != 0 && !availableYears.Contains(command.Year))
        {
            throw new ArgumentException($"Year must be 0 (unassigned) or between {availableYears.Min()} and {availableYears.Max()} for this specialization.");
        }

        // MAUI implementation does not enforce maximum shift duration for Old SMK
        // Only check that total duration is greater than zero
        if (command.Hours == 0 && command.Minutes == 0)
        {
            throw new ArgumentException("Shift duration must be greater than zero.");
        }

        // Location validation for Old SMK - should be a department/unit name
        if (command.Location.Length > 100)
        {
            throw new ArgumentException("Location name cannot exceed 100 characters.");
        }
    }

    private void ValidateNewSmkShift(AddMedicalShift command, Specialization specialization)
    {
        // New SMK specific validations
        // New SMK doesn't use year field in the same way, but we still validate it's provided
        if (command.Year <= 0)
        {
            throw new ArgumentException("Year must be provided for New SMK.");
        }

        // MAUI implementation does not enforce maximum shift duration for New SMK
        // Only check that total duration is greater than zero
        if (command.Hours == 0 && command.Minutes == 0)
        {
            throw new ArgumentException("Shift duration must be greater than zero.");
        }

        // Location validation for New SMK - should match internship requirements
        if (command.Location.Length > 100)
        {
            throw new ArgumentException("Location name cannot exceed 100 characters.");
        }

        // For New SMK, we should validate that the shift is within an active internship period
        // This is done by ensuring the InternshipId is valid and the date is within internship dates
        // The actual internship validation is done when fetching the internship
    }
}