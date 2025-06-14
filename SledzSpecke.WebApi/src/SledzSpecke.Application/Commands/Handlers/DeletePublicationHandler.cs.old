using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class DeletePublicationHandler : ICommandHandler<DeletePublication>
{
    private readonly IPublicationRepository _publicationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public DeletePublicationHandler(
        IPublicationRepository publicationRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _publicationRepository = publicationRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(DeletePublication command)
    {
        var publication = await _publicationRepository.GetByIdAsync(command.PublicationId);
        if (publication is null)
        {
            throw new PublicationNotFoundException(command.PublicationId.Value);
        }

        var currentUserId = _userContextService.GetUserId();
        if (publication.UserId.Value != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only delete your own publications.");
        }

        await _publicationRepository.DeleteAsync(publication.Id);
        await _unitOfWork.SaveChangesAsync();
    }
}