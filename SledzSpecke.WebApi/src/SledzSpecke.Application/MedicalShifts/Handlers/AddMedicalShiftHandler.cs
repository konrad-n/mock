using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

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

    public async Task<Result<int>> HandleAsync(AddMedicalShift command)
    {
        try
        {
            // Validate internship exists
            var internshipId = new InternshipId(command.InternshipId);
            var internship = await _internshipRepository.GetByIdAsync(internshipId);
            if (internship is null)
            {
                return Result.Failure<int>($"Internship with ID {command.InternshipId} not found.");
            }

            // Validate specialization exists
            var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
            if (specialization is null)
            {
                return Result.Failure<int>($"Specialization with ID {internship.SpecializationId.Value} not found.");
            }

            // Validate date range for New SMK
            if (specialization.SmkVersion == SmkVersion.New)
            {
                if (command.Date < internship.StartDate || command.Date > internship.EndDate)
                {
                    return Result.Failure<int>("Medical shift date must be within the internship period for New SMK.");
                }
            }

            // Validate year
            if (specialization.SmkVersion.IsOld)
            {
                var availableYears = _yearCalculationService.GetAvailableYears(specialization);
                // Allow year 0 for unassigned shifts
                if (command.Year != 0 && !availableYears.Contains(command.Year))
                {
                    return Result.Failure<int>(
                        $"Year must be 0 (unassigned) or between {availableYears.Min()} and {availableYears.Max()} for this specialization.");
                }
            }
            else if (specialization.SmkVersion.IsNew)
            {
                if (command.Year <= 0)
                {
                    return Result.Failure<int>("Year must be provided for New SMK.");
                }
            }

            // Create medical shift
            var medicalShiftId = new MedicalShiftId(0); // Will be assigned by repository
            var medicalShift = MedicalShift.Create(
                medicalShiftId,
                internshipId,
                command.Date,
                command.Hours,
                command.Minutes,
                command.Location,
                command.Year
            );

            // Validate using template service
            var validationResult = await _validationService.ValidateMedicalShiftAsync(
                medicalShift,
                specialization.Id.Value);

            if (!validationResult.IsValid)
            {
                return Result.Failure<int>($"Medical shift validation failed: {string.Join(", ", validationResult.Errors)}");
            }

            // Save the medical shift
            var shiftId = await _medicalShiftRepository.AddAsync(medicalShift);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success(shiftId);
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure<int>(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure<int>("An error occurred while adding the medical shift.");
        }
    }
}