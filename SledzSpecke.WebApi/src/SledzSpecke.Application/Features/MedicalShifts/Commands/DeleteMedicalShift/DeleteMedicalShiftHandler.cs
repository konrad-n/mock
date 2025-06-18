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

            // Get the internship to remove the shift from its collection
            var internship = await _internshipRepository.GetByIdAsync(medicalShift.InternshipId.Value);
            if (internship is null)
            {
                return Result.Failure($"Internship with ID {medicalShift.InternshipId.Value} not found.", "INTERNSHIP_NOT_FOUND");
            }

            // Use domain method to remove medical shift
            var removeResult = internship.RemoveMedicalShift(medicalShift.Id);
            if (removeResult.IsFailure)
            {
                return Result.Failure(removeResult.Error, removeResult.ErrorCode);
            }

            // Update internship (already modified by domain method)
            await _internshipRepository.UpdateAsync(internship);
            
            // Delete the medical shift
            await _medicalShiftRepository.DeleteAsync(medicalShift);
            
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"An error occurred while deleting the medical shift: {ex.Message}", "SHIFT_DELETE_FAILED");
        }
    }
}