using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class UpdatePublicationHandler : ICommandHandler<UpdatePublication>
{
    private readonly IPublicationRepository _publicationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdatePublicationHandler(
        IPublicationRepository publicationRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _publicationRepository = publicationRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(UpdatePublication command)
    {
        var publication = await _publicationRepository.GetByIdAsync(command.PublicationId);
        if (publication is null)
        {
            throw new PublicationNotFoundException(command.PublicationId.Value);
        }

        var currentUserId = _userContextService.GetUserId();
        if (publication.UserId.Value != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only update your own publications.");
        }

        publication.UpdateDetails(
            command.Title,
            command.Authors,
            command.Journal,
            command.Publisher,
            command.Abstract,
            command.Keywords,
            command.IsFirstAuthor,
            command.IsCorrespondingAuthor,
            command.IsPeerReviewed);

        await _publicationRepository.UpdateAsync(publication);
        await _unitOfWork.SaveChangesAsync();
    }
}