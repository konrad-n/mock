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

    public AddMedicalShiftHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
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

        // Validate shift based on SMK version
        ValidateShiftForSmkVersion(command, specialization.SmkVersion);

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

        // Save the medical shift
        var shiftId = await _medicalShiftRepository.AddAsync(medicalShift);

        return shiftId;
    }

    private static void ValidateShiftForSmkVersion(AddMedicalShift command, SmkVersion smkVersion)
    {
        // Common validations
        if (command.Hours < 0 || command.Minutes < 0)
        {
            throw new ArgumentException("Shift duration cannot be negative.");
        }

        if (command.Minutes > 59)
        {
            throw new ArgumentException("Minutes must be between 0 and 59.");
        }

        if (string.IsNullOrWhiteSpace(command.Location))
        {
            throw new ArgumentException("Location is required.");
        }

        // SMK version-specific validations
        switch (smkVersion)
        {
            case SmkVersion.Old:
                ValidateOldSmkShift(command);
                break;
            case SmkVersion.New:
                ValidateNewSmkShift(command);
                break;
            default:
                throw new InvalidOperationException($"Unknown SMK version: {smkVersion}");
        }
    }

    private static void ValidateOldSmkShift(AddMedicalShift command)
    {
        // Old SMK specific validations
        if (command.Year < 1 || command.Year > 5)
        {
            throw new ArgumentException("Year must be between 1 and 5 for Old SMK.");
        }

        // Old SMK typically allows longer shifts (up to 24 hours)
        var totalHours = command.Hours + (command.Minutes / 60.0);
        if (totalHours > 24)
        {
            throw new ArgumentException("Shift duration cannot exceed 24 hours for Old SMK.");
        }

        if (totalHours == 0)
        {
            throw new ArgumentException("Shift duration must be greater than zero.");
        }

        // Location validation for Old SMK - should be a department/unit name
        if (command.Location.Length > 100)
        {
            throw new ArgumentException("Location name cannot exceed 100 characters.");
        }
    }

    private static void ValidateNewSmkShift(AddMedicalShift command)
    {
        // New SMK specific validations
        // New SMK doesn't use year field in the same way, but we still validate it's provided
        if (command.Year <= 0)
        {
            throw new ArgumentException("Year must be provided for New SMK.");
        }

        // New SMK has stricter shift duration limits (typically max 16 hours)
        var totalHours = command.Hours + (command.Minutes / 60.0);
        if (totalHours > 16)
        {
            throw new ArgumentException("Shift duration cannot exceed 16 hours for New SMK.");
        }

        if (totalHours < 4)
        {
            throw new ArgumentException("Shift duration must be at least 4 hours for New SMK.");
        }

        // Location validation for New SMK - should match internship requirements
        if (command.Location.Length > 100)
        {
            throw new ArgumentException("Location name cannot exceed 100 characters.");
        }
    }
}