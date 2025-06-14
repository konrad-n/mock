using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class DeleteInternshipHandler : IResultCommandHandler<DeleteInternship>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInternshipHandler(
        IInternshipRepository internshipRepository,
        IUserRepository userRepository,
        IUserContextService userContextService,
        IUnitOfWork unitOfWork)
    {
        _internshipRepository = internshipRepository;
        _userRepository = userRepository;
        _userContextService = userContextService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteInternship command)
    {
        try
        {
            var internshipId = new InternshipId(command.InternshipId);
            var internship = await _internshipRepository.GetByIdAsync(internshipId);
            if (internship is null)
            {
                return Result.Failure($"Internship with ID {command.InternshipId} not found.");
            }

            // Check if the internship belongs to the current user
            var userId = _userContextService.GetUserId();
            var user = await _userRepository.GetByIdAsync(new UserId(userId));
            if (user is null || user.SpecializationId.Value != internship.SpecializationId.Value)
            {
                return Result.Failure("You can only delete your own internships.");
            }

            // Check if internship can be deleted
            if (!internship.CanBeDeleted())
            {
                return Result.Failure("Cannot delete synced or approved internship.");
            }

            await _internshipRepository.DeleteAsync(internshipId);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while deleting the internship.");
        }
    }
}