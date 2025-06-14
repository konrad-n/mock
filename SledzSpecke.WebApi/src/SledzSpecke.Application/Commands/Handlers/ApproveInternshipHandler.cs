using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class ApproveInternshipHandler : IResultCommandHandler<ApproveInternship>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveInternshipHandler(
        IInternshipRepository internshipRepository,
        IUnitOfWork unitOfWork)
    {
        _internshipRepository = internshipRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(ApproveInternship command)
    {
        try
        {
            var internshipId = new InternshipId(command.InternshipId);
            var internship = await _internshipRepository.GetByIdAsync(internshipId);
            if (internship is null)
            {
                return Result.Failure($"Internship with ID {command.InternshipId} not found.");
            }

            if (internship.IsApproved)
            {
                return Result.Failure("Internship is already approved.");
            }

            var approverName = new PersonName(command.ApproverName);
            internship.Approve(approverName);
            
            await _internshipRepository.UpdateAsync(internship);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while approving the internship.");
        }
    }
}