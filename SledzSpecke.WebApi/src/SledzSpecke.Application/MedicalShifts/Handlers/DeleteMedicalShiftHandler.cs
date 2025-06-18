using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

public sealed class DeleteMedicalShiftHandler : IResultCommandHandler<DeleteMedicalShift>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IUserContextService _userContextService;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMedicalShiftHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IUserContextService userContextService,
        IInternshipRepository internshipRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _userContextService = userContextService;
        _internshipRepository = internshipRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteMedicalShift command, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userContextService.GetUserId();
            var medicalShiftId = new MedicalShiftId(command.ShiftId);
            var shift = await _medicalShiftRepository.GetByIdAsync(command.ShiftId);

            if (shift == null)
            {
                return Result.Failure($"Medical shift with ID {command.ShiftId} not found.");
            }

            // Get internship to check ownership
            var internship = await _internshipRepository.GetByIdAsync(shift.InternshipId);
            if (internship == null)
            {
                return Result.Failure($"Internship with ID {shift.InternshipId.Value} not found.");
            }

            // Get user to verify ownership through specialization
            var user = await _userRepository.GetByIdAsync(new UserId(userId), cancellationToken);
            // TODO: User-Specialization relationship needs to be redesigned
            // if (user == null || user.SpecializationId.Value != internship.SpecializationId.Value)
            // {
            //     return Result.Failure("You are not authorized to delete this medical shift.");
            // }
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // Check if the shift can be deleted
            if (shift.IsApproved)
            {
                return Result.Failure("Cannot delete an approved medical shift.");
            }

            if (shift.SyncStatus == SyncStatus.Synced)
            {
                return Result.Failure("Cannot delete a synced medical shift.");
            }

            await _medicalShiftRepository.DeleteAsync(command.ShiftId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success();
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while deleting the medical shift.");
        }
    }
}