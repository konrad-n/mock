using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class CreatePublicationHandler : ICommandHandler<CreatePublication>
{
    private readonly IPublicationRepository _publicationRepository;

    public CreatePublicationHandler(IPublicationRepository publicationRepository)
    {
        _publicationRepository = publicationRepository;
    }

    public async Task HandleAsync(CreatePublication command)
    {
        var publication = Publication.Create(
            PublicationId.New(),
            command.SpecializationId,
            command.UserId,
            command.Type,
            command.Title,
            command.PublicationDate);

        if (!string.IsNullOrEmpty(command.Authors) || !string.IsNullOrEmpty(command.Journal) || !string.IsNullOrEmpty(command.Publisher))
        {
            publication.UpdateBasicDetails(
                command.Type,
                command.Title,
                command.PublicationDate,
                command.Authors,
                command.Journal,
                command.Publisher);
        }

        await _publicationRepository.AddAsync(publication);
    }
}