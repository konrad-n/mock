using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Constants;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Features.MedicalShifts.Commands.AddMedicalShift;

public sealed class AddMedicalShiftHandler : IResultCommandHandler<AddMedicalShift, int>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ISpecializationValidationService _validationService;
    private readonly IYearCalculationService _yearCalculationService;
    private readonly IUnitOfWork _unitOfWork;

    public AddMedicalShiftHandler(
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

    public async Task<Result<int>> HandleAsync(AddMedicalShift command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate internship exists
            var internshipId = new InternshipId(command.InternshipId);
            var internship = await _internshipRepository.GetByIdAsync(internshipId);
            if (internship is null)
            {
                return Result.Failure<int>($"Internship with ID {command.InternshipId} not found.", Core.Constants.ErrorCodes.INTERNSHIP_NOT_FOUND);
            }

            // Validate specialization exists
            var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
            if (specialization is null)
            {
                return Result.Failure<int>($"Specialization with ID {internship.SpecializationId.Value} not found.", Core.Constants.ErrorCodes.SPECIALIZATION_NOT_FOUND);
            }

            // Get available years for validation
            var availableYears = _yearCalculationService.GetAvailableYears(specialization);

            // Use domain method to add medical shift with all business logic
            var result = internship.AddMedicalShift(
                command.Date,
                command.Hours,
                command.Minutes,
                command.Location,
                command.Year,
                specialization.SmkVersion,
                availableYears.ToArray()
            );

            if (result.IsFailure)
            {
                return Result.Failure<int>(result.Error, result.ErrorCode);
            }

            var medicalShift = result.Value;

            // Validate using template service (external validation)
            var validationResult = await _validationService.ValidateMedicalShiftAsync(
                medicalShift,
                specialization.Id.Value);

            if (!validationResult.IsValid)
            {
                return Result.Failure<int>($"Medical shift validation failed: {string.Join(", ", validationResult.Errors)}", ErrorCodes.VALIDATION_ERROR);
            }

            // Update the internship (with the new shift already added)
            await _internshipRepository.UpdateAsync(internship);
            
            // Save changes and dispatch domain events
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(medicalShift.Id.Value);
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure<int>(ex.Message, ErrorCodes.VALIDATION_ERROR);
        }
        catch (Exception)
        {
            return Result.Failure<int>("An error occurred while adding the medical shift.", ErrorCodes.INTERNAL_ERROR);
        }
    }
}