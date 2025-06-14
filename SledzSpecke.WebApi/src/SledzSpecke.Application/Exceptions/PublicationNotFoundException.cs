using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class PublicationNotFoundException : NotFoundException
{
    public PublicationNotFoundException(Guid publicationId) 
        : base($"Publication with ID {publicationId} was not found.")
    {
        PublicationId = publicationId;
    }

    public Guid PublicationId { get; }
}