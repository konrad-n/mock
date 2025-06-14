using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class MarkInternshipAsCompletedHandler : IResultCommandHandler<MarkInternshipAsCompleted>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public MarkInternshipAsCompletedHandler(
        IInternshipRepository internshipRepository,
        IUserContextService userContextService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _internshipRepository = internshipRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<Result> HandleAsync(MarkInternshipAsCompleted command)
    {
        try
        {
            var internshipId = new InternshipId(command.InternshipId);
            var internship = await _internshipRepository.GetByIdAsync(internshipId);
            if (internship == null)
            {
                return Result.Failure($"Internship with ID {command.InternshipId} not found.");
            }

            // Verify user has access to this internship
            var userId = _userContextService.GetUserId();
            var user = await _userRepository.GetByIdAsync(new UserId(userId));
            if (user == null || user.SpecializationId.Value != internship.SpecializationId.Value)
            {
                return Result.Failure("You are not authorized to modify this internship.");
            }

            // Check if internship can be marked as completed
            if (internship.IsApproved)
            {
                return Result.Failure("Cannot modify an approved internship.");
            }

            if (internship.IsCompleted)
            {
                return Result.Failure("Internship is already marked as completed.");
            }

            // Validate that the internship has ended or is ending today
            if (internship.EndDate > _clock.Current().Date)
            {
                return Result.Failure("Cannot mark an internship as completed before its end date.");
            }

            // Mark as completed
            internship.MarkAsCompleted();

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
            return Result.Failure("An error occurred while marking the internship as completed.");
        }
    }
}