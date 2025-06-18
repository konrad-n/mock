using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class DeletePublicationHandler : IResultCommandHandler<DeletePublication>
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

    public async Task<Result> HandleAsync(DeletePublication command, CancellationToken cancellationToken = default)
    {
        try
        {
            var publication = await _publicationRepository.GetByIdAsync(command.PublicationId);
            if (publication is null)
            {
                return Result.Failure($"Publication with ID {command.PublicationId.Value} not found.");
            }

            var currentUserId = _userContextService.GetUserId();
            if (publication.UserId.Value != (int)currentUserId)
            {
                return Result.Failure("You can only delete your own publications.");
            }

            // Check if publication can be deleted
            if (!publication.CanBeModified)
            {
                return Result.Failure("This publication cannot be deleted. It may be synced.");
            }

            // Check sync status explicitly
            if (publication.SyncStatus == SyncStatus.Synced)
            {
                return Result.Failure("Cannot delete a synced publication. Contact your administrator if deletion is required.");
            }

            await _publicationRepository.DeleteAsync(publication.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while deleting the publication.");
        }
    }
}