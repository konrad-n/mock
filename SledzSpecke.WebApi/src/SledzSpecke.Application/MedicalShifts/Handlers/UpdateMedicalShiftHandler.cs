using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

public class UpdateMedicalShiftHandler : ICommandHandler<UpdateMedicalShift>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IUserContextService _userContextService;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUserRepository _userRepository;

    public UpdateMedicalShiftHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IUserContextService userContextService,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        IUserRepository userRepository)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _userContextService = userContextService;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _userRepository = userRepository;
    }

    public async Task HandleAsync(UpdateMedicalShift command)
    {
        var userId = _userContextService.GetUserId();
        var shift = await _medicalShiftRepository.GetByIdAsync(command.ShiftId);

        if (shift == null)
        {
            throw new NotFoundException($"Medical shift with ID {command.ShiftId} not found.");
        }

        // Get internship to check ownership
        var internship = await _internshipRepository.GetByIdAsync(shift.InternshipId);
        if (internship == null)
        {
            throw new NotFoundException($"Internship with ID {shift.InternshipId} not found.");
        }
        
        // Get user to verify ownership through specialization
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user == null || user.SpecializationId != internship.SpecializationId)
        {
            throw new UnauthorizedException("You are not authorized to update this medical shift.");
        }

        // Check if the shift can be modified
        if (shift.IsApproved)
        {
            throw new BusinessRuleException("Cannot update an approved medical shift.");
        }

        // Prepare update values
        var hours = command.Hours ?? shift.Hours;
        var minutes = command.Minutes ?? shift.Minutes;
        var location = !string.IsNullOrWhiteSpace(command.Location) ? command.Location : shift.Location;
        
        // Validate values before update
        if (command.Hours.HasValue && (command.Hours.Value < 0 || command.Hours.Value > 23))
        {
            throw new ValidationException("Hours must be between 0 and 23.");
        }
        
        if (command.Minutes.HasValue && (command.Minutes.Value < 0 || command.Minutes.Value > 59))
        {
            throw new ValidationException("Minutes must be between 0 and 59.");
        }
        
        // Note: Date changes are not supported through the UpdateShiftDetails method
        // If date update is needed, we'd need a separate method in MedicalShift entity
        if (command.Date.HasValue && command.Date.Value != shift.Date)
        {
            throw new BusinessRuleException("Cannot change the date of a medical shift. Please delete and create a new shift instead.");
        }
        
        // Update the shift using the proper method
        shift.UpdateShiftDetails(hours, minutes, location);

        // Get specialization to check SMK version specific rules
        var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
        if (specialization == null)
        {
            throw new NotFoundException($"Specialization not found for internship {shift.InternshipId}.");
        }
        
        // For New SMK, validate total duration doesn't exceed 16 hours
        if (specialization.SmkVersion == SmkVersion.New)
        {
            var totalMinutes = shift.TotalMinutes;
            if (totalMinutes > 960) // 16 hours in minutes
            {
                throw new ValidationException("Medical shift duration cannot exceed 16 hours for New SMK.");
            }
            if (totalMinutes < 240) // 4 hours in minutes
            {
                throw new ValidationException("Medical shift duration must be at least 4 hours for New SMK.");
            }
        }

        await _medicalShiftRepository.UpdateAsync(shift);
    }
}