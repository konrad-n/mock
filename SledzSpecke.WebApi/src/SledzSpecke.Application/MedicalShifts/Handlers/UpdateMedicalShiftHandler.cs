using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.MedicalShifts.Handlers;

public sealed class UpdateMedicalShiftHandler : IResultCommandHandler<UpdateMedicalShift>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IUserContextService _userContextService;
    private readonly IInternshipRepository _internshipRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMedicalShiftHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IUserContextService userContextService,
        IInternshipRepository internshipRepository,
        ISpecializationRepository specializationRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _userContextService = userContextService;
        _internshipRepository = internshipRepository;
        _specializationRepository = specializationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateMedicalShift command, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _userContextService.GetUserId();
            var medicalShiftId = new MedicalShiftId(command.ShiftId);
            var shift = await _medicalShiftRepository.GetByIdAsync(medicalShiftId, cancellationToken);

            if (shift == null)
            {
                return Result.Failure($"Medical shift with ID {command.ShiftId} not found.");
            }

            // Get internship to check ownership
            var internship = await _internshipRepository.GetByIdAsync(shift.InternshipId, cancellationToken);
            if (internship == null)
            {
                return Result.Failure($"Internship with ID {shift.InternshipId.Value} not found.");
            }

            // Get user to verify ownership through specialization
            var user = await _userRepository.GetByIdAsync(new UserId(userId), cancellationToken);
            // TODO: User-Specialization relationship needs to be redesigned
            // if (user == null || user.SpecializationId.Value != internship.SpecializationId.Value)
            // {
            //     return Result.Failure("You are not authorized to update this medical shift.");
            // }
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            // Check if the shift can be modified
            if (shift.IsApproved)
            {
                return Result.Failure("Cannot update an approved medical shift.");
            }

            // Check date change rule
            if (command.Date.HasValue && command.Date.Value != shift.Date)
            {
                return Result.Failure("Cannot change the date of a medical shift. Please delete and create a new shift instead.");
            }

            // Update the shift details
            var hours = command.Hours ?? shift.Hours;
            var minutes = command.Minutes ?? shift.Minutes;
            var location = !string.IsNullOrWhiteSpace(command.Location) ? command.Location : shift.Location;

            shift.UpdateShiftDetails(hours, minutes, location);

            // Validate final state
            if (shift.TotalMinutes == 0)
            {
                return Result.Failure("Medical shift duration must be greater than zero.");
            }

            await _medicalShiftRepository.UpdateAsync(shift, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Success();
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while updating the medical shift.");
        }
    }
}