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
    private readonly IQueryHandler<GetPeerReviewedPublications, IEnumerable<PublicationDto>> _getPeerReviewedPublicationsHandler;
    private readonly IQueryHandler<GetFirstAuthorPublications, IEnumerable<PublicationDto>> _getFirstAuthorPublicationsHandler;
    private readonly IQueryHandler<GetPublicationImpactScore, decimal> _getPublicationImpactScoreHandler;
    private readonly ICommandHandler<UpdatePublication> _updatePublicationHandler;
    private readonly ICommandHandler<DeletePublication> _deletePublicationHandler;

    public PublicationsController(
        ICommandHandler<CreatePublication> createPublicationHandler,
        IQueryHandler<GetUserPublications, IEnumerable<PublicationDto>> getUserPublicationsHandler,
        IQueryHandler<GetPeerReviewedPublications, IEnumerable<PublicationDto>> getPeerReviewedPublicationsHandler,
        IQueryHandler<GetFirstAuthorPublications, IEnumerable<PublicationDto>> getFirstAuthorPublicationsHandler,
        IQueryHandler<GetPublicationImpactScore, decimal> getPublicationImpactScoreHandler,
        ICommandHandler<UpdatePublication> updatePublicationHandler,
        ICommandHandler<DeletePublication> deletePublicationHandler) : base()
    {
        _createPublicationHandler = createPublicationHandler;
        _getUserPublicationsHandler = getUserPublicationsHandler;
        _getPeerReviewedPublicationsHandler = getPeerReviewedPublicationsHandler;
        _getFirstAuthorPublicationsHandler = getFirstAuthorPublicationsHandler;
        _getPublicationImpactScoreHandler = getPublicationImpactScoreHandler;
        _updatePublicationHandler = updatePublicationHandler;
        _deletePublicationHandler = deletePublicationHandler;
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
    public async Task<ActionResult<IEnumerable<PublicationDto>>> GetPeerReviewedPublications(int userId)
    {
        var query = new GetPeerReviewedPublications(userId);
        var publications = await _getPeerReviewedPublicationsHandler.HandleAsync(query);
        return Ok(publications);
    }

    [HttpGet("user/{userId}/first-author")]
    public async Task<ActionResult<IEnumerable<PublicationDto>>> GetFirstAuthorPublications(int userId)
    {
        var query = new GetFirstAuthorPublications(userId);
        var publications = await _getFirstAuthorPublicationsHandler.HandleAsync(query);
        return Ok(publications);
    }

    [HttpGet("user/{userId}/specialization/{specializationId}/impact-score")]
    public async Task<ActionResult<decimal>> GetTotalImpactScore(int userId, int specializationId)
    {
        var query = new GetPublicationImpactScore(userId, specializationId);
        var score = await _getPublicationImpactScoreHandler.HandleAsync(query);
        return Ok(score);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePublication(Guid id, [FromBody] UpdatePublicationRequest request)
    {
        var command = new UpdatePublication(
            new PublicationId(id),
            request.Title,
            request.Authors,
            request.Journal,
            request.Publisher,
            request.Abstract,
            request.Keywords,
            request.IsFirstAuthor,
            request.IsCorrespondingAuthor,
            request.IsPeerReviewed);

        await _updatePublicationHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePublication(Guid id)
    {
        var command = new DeletePublication(new PublicationId(id));
        await _deletePublicationHandler.HandleAsync(command);
        return NoContent();
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