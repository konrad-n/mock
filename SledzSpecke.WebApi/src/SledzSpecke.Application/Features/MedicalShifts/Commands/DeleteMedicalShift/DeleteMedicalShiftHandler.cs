using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Features.MedicalShifts.Commands.DeleteMedicalShift;

public sealed class DeleteMedicalShiftHandler : IResultCommandHandler<DeleteMedicalShift>
{
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMedicalShiftHandler(
        IMedicalShiftRepository medicalShiftRepository,
        IInternshipRepository internshipRepository,
        IUnitOfWork unitOfWork)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _internshipRepository = internshipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteMedicalShift command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the medical shift
            var medicalShift = await _medicalShiftRepository.GetByIdAsync(command.Id);
            if (medicalShift is null)
            {
                return Result.Failure($"Medical shift with ID {command.Id} not found.", "SHIFT_NOT_FOUND");
            }

            // Check if shift can be deleted
            if (!medicalShift.CanBeDeleted)
            {
                return Result.Failure("Cannot delete an approved medical shift.", "SHIFT_ALREADY_APPROVED");
            }

            // Delete the medical shift
            await _medicalShiftRepository.DeleteAsync(command.Id);
            
            // Note: Save changes is already called in the repository DeleteAsync method
            // await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while deleting the medical shift: {ex.Message}", "SHIFT_DELETE_FAILED");
        }
    }
}