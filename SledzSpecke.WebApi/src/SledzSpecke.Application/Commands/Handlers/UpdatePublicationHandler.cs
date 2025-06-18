using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class UpdatePublicationHandler : IResultCommandHandler<UpdatePublication>
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

    public async Task<Result> HandleAsync(UpdatePublication command, CancellationToken cancellationToken = default)
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
                return Result.Failure("You can only update your own publications.");
            }

            // Check if publication can be modified
            if (!publication.CanBeModified)
            {
                return Result.Failure("This publication cannot be modified. It may be synced.");
            }

            // Validate title is not empty
            if (string.IsNullOrWhiteSpace(command.Title))
            {
                return Result.Failure("Publication title cannot be empty.");
            }

            // Update the publication details
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

            return Result.Success();
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred while updating the publication.");
        }
    }
}