using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Queries;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicationsController : BaseController
{
    private readonly ICommandHandler<CreatePublication> _createPublicationHandler;
    private readonly IQueryHandler<GetUserPublications, IEnumerable<PublicationDto>> _getUserPublicationsHandler;

    public PublicationsController(
        ICommandHandler<CreatePublication> createPublicationHandler,
        IQueryHandler<GetUserPublications, IEnumerable<PublicationDto>> getUserPublicationsHandler) : base()
    {
        _createPublicationHandler = createPublicationHandler;
        _getUserPublicationsHandler = getUserPublicationsHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePublication([FromBody] CreatePublicationRequest request)
    {
        var command = new CreatePublication(
            new SpecializationId(request.SpecializationId),
            new UserId(request.UserId),
            (PublicationType)request.Type,
            request.Title,
            request.PublicationDate,
            request.Authors,
            request.Journal,
            request.Publisher);

        await _createPublicationHandler.HandleAsync(command);
        return Ok();
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPublications(int userId, [FromQuery] int? specializationId = null)
    {
        var query = new GetUserPublications(userId, specializationId);
        var publications = await _getUserPublicationsHandler.HandleAsync(query);
        return Ok(publications);
    }

    [HttpGet("user/{userId}/peer-reviewed")]
    public async Task<IActionResult> GetPeerReviewedPublications(Guid userId)
    {
        // This would need a query handler implementation
        return Ok();
    }

    [HttpGet("user/{userId}/first-author")]
    public async Task<IActionResult> GetFirstAuthorPublications(Guid userId)
    {
        // This would need a query handler implementation
        return Ok();
    }

    [HttpGet("user/{userId}/specialization/{specializationId}/impact-score")]
    public async Task<IActionResult> GetTotalImpactScore(Guid userId, Guid specializationId)
    {
        // This would need a query handler implementation
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePublication(Guid id, [FromBody] UpdatePublicationRequest request)
    {
        // This would need an update publication command and handler
        return Ok();
    }
}

public record CreatePublicationRequest(
    int SpecializationId,
    int UserId,
    int Type,
    string Title,
    DateTime PublicationDate,
    string? Authors = null,
    string? Journal = null,
    string? Publisher = null);

public record UpdatePublicationRequest(
    string Title,
    string? Authors,
    string? Journal,
    string? Publisher,
    string? Abstract,
    string? Keywords,
    bool IsFirstAuthor,
    bool IsCorrespondingAuthor,
    bool IsPeerReviewed);