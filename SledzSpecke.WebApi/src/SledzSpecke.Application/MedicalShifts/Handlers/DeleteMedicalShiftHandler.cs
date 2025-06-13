using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

public class DeleteMedicalShiftHandler : ICommandHandler<DeleteMedicalShift>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IUserContextService _userContextService;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserRepository _userRepository;

    public DeleteMedicalShiftHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IUserContextService userContextService,
        IInternshipRepository internshipRepository,
        IUserRepository userRepository)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _userContextService = userContextService;
        _internshipRepository = internshipRepository;
        _userRepository = userRepository;
    }

    public async Task HandleAsync(DeleteMedicalShift command)
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
            throw new UnauthorizedException("You are not authorized to delete this medical shift.");
        }

        // Check if the shift can be deleted
        if (shift.IsApproved)
        {
            throw new BusinessRuleException("Cannot delete an approved medical shift.");
        }

        if (shift.SyncStatus == SyncStatus.Synced)
        {
            throw new BusinessRuleException("Cannot delete a synced medical shift.");
        }

        await _medicalShiftRepository.DeleteAsync(shift.Id);
    }
}