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

        var currentUserId = _userContextService.GetUserId();
        if (selfEducation.UserId.Value != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only delete your own self-education activities.");
        }

        if (selfEducation.IsCompleted)
        {
            throw new InvalidOperationException("Cannot delete a completed self-education activity.");
        }

        await _selfEducationRepository.DeleteAsync(selfEducation.Id);
        await _unitOfWork.SaveChangesAsync();
    }
}