using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.DomainServices;

namespace SledzSpecke.Application.Features.MedicalShifts.Commands.UpdateMedicalShift;

public sealed class UpdateMedicalShiftHandler : IResultCommandHandler<UpdateMedicalShift>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IMedicalShiftValidationService _validationService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMedicalShiftHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IInternshipRepository internshipRepository,
        IMedicalShiftValidationService validationService,
        IUnitOfWork unitOfWork)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _internshipRepository = internshipRepository;
        _validationService = validationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateMedicalShift command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the medical shift
            var medicalShift = await _medicalShiftRepository.GetByIdAsync(command.Id);
            if (medicalShift is null)
            {
                return Result.Failure($"Medical shift with ID {command.Id} not found.", "SHIFT_NOT_FOUND");
            }

            // Get the internship to validate date range if date is being updated
            var internship = await _internshipRepository.GetByIdAsync(medicalShift.InternshipId.Value);
            if (internship is null)
            {
                return Result.Failure($"Internship with ID {medicalShift.InternshipId.Value} not found.", "INTERNSHIP_NOT_FOUND");
            }

            // Validate date if provided
            if (command.Date.HasValue)
            {
                if (command.Date.Value < internship.StartDate || command.Date.Value > internship.EndDate)
                {
                    return Result.Failure("Shift date must be within the internship period.", "DATE_OUT_OF_RANGE");
                }
            }

            // Build new values
            var newDate = command.Date ?? medicalShift.Date;
            var newHours = command.Hours ?? medicalShift.Duration.Hours;
            var newMinutes = command.Minutes ?? medicalShift.Duration.Minutes;
            var newLocation = command.Location ?? medicalShift.Location;
            
            // Create new duration
            var newDuration = new ShiftDuration(newHours, newMinutes);
            
            // Update the medical shift using domain method
            medicalShift.Update(newDate, newDuration, medicalShift.Type, newLocation, medicalShift.SupervisorName);

            // Update the medical shift
            await _medicalShiftRepository.UpdateAsync(medicalShift);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while updating the medical shift: {ex.Message}", "SHIFT_UPDATE_FAILED");
        }
    }
}