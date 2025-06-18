using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.DomainServices;

namespace SledzSpecke.Application.Features.MedicalShifts.Commands.AddMedicalShift;

public sealed class AddMedicalShiftHandler : IResultCommandHandler<AddMedicalShift, int>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IMedicalShiftValidationService _validationService;
    private readonly IDurationCalculationService _durationCalculationService;
    private readonly IUnitOfWork _unitOfWork;

    public AddMedicalShiftHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        IMedicalShiftValidationService validationService,
        IDurationCalculationService durationCalculationService,
        IUnitOfWork unitOfWork)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _validationService = validationService;
        _durationCalculationService = durationCalculationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> HandleAsync(AddMedicalShift command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate internship exists
            var internship = await _internshipRepository.GetByIdAsync(command.InternshipId);
            if (internship is null)
            {
                return Result<int>.Failure($"Internship with ID {command.InternshipId} not found.", "INTERNSHIP_NOT_FOUND");
            }

            // Validate date within internship period
            if (command.Date < internship.StartDate || command.Date > internship.EndDate)
            {
                return Result<int>.Failure("Shift date must be within the internship period.", "DATE_OUT_OF_RANGE");
            }

            // Create duration value object
            var duration = new ShiftDuration(command.Hours, command.Minutes);

            // Get the specialization to access SMK version
            var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId.Value);
            if (specialization is null)
            {
                return Result<int>.Failure($"Specialization with ID {internship.SpecializationId.Value} not found.", "SPECIALIZATION_NOT_FOUND");
            }
            
            // Get SMK version and available years from specialization
            var smkVersion = specialization.SmkVersion;
            var availableYears = new[] { 1, 2, 3, 4, 5, 6 }; // Standard medical education years
            
            // Use domain method to add medical shift
            var addResult = internship.AddMedicalShift(
                command.Date,
                command.Hours,
                command.Minutes,
                command.Location,
                command.Year,
                smkVersion,
                availableYears);

            if (addResult.IsFailure)
            {
                return Result<int>.Failure(addResult.Error, addResult.ErrorCode);
            }

            var medicalShift = addResult.Value;

            // Update internship (already modified by domain method)
            await _internshipRepository.UpdateAsync(internship);
            
            // Save the medical shift separately
            var shiftIdValue = await _medicalShiftRepository.AddAsync(medicalShift);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(shiftIdValue);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"An error occurred while adding the medical shift: {ex.Message}", "SHIFT_ADD_FAILED");
        }
    }
}