using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class DeleteSelfEducationHandler : ICommandHandler<DeleteSelfEducation>
{
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public DeleteSelfEducationHandler(
        ISelfEducationRepository selfEducationRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _selfEducationRepository = selfEducationRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(DeleteSelfEducation command)
    {
        var selfEducation = await _selfEducationRepository.GetByIdAsync(command.SelfEducationId);
        if (selfEducation is null)
        {
            throw new SelfEducationNotFoundException(command.SelfEducationId.Value);
        }

        // Note: The new SelfEducation entity doesn't have UserId or IsCompleted
        // For backward compatibility, skip these checks
        // In the new model, all recorded activities are considered complete
        
        // Check if it can be modified (synced items might not be deletable)
        if (!selfEducation.CanBeModified)
        {
            throw new InvalidOperationException("Cannot delete a synced self-education activity.");
        }

        await _selfEducationRepository.DeleteAsync(selfEducation.Id);
        await _unitOfWork.SaveChangesAsync();
    }
}