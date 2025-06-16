using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class CompleteSelfEducationHandler : ICommandHandler<CompleteSelfEducation>
{
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public CompleteSelfEducationHandler(
        ISelfEducationRepository selfEducationRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _selfEducationRepository = selfEducationRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(CompleteSelfEducation command)
    {
        var selfEducation = await _selfEducationRepository.GetByIdAsync(command.SelfEducationId);
        if (selfEducation is null)
        {
            throw new SelfEducationNotFoundException(command.SelfEducationId.Value);
        }

        // Note: The new SelfEducation entity doesn't have UserId or Complete method
        // Self-education activities are considered complete when recorded
        // For backward compatibility, we'll just update the sync status
        
        // No-op since activities are complete when created in the new model
        // Just update the repository to trigger any necessary updates

        await _selfEducationRepository.UpdateAsync(selfEducation);
        await _unitOfWork.SaveChangesAsync();
    }
}