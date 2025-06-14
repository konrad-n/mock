using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Queries;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SelfEducationController : BaseController
{
    private readonly ICommandHandler<CreateSelfEducation> _createSelfEducationHandler;
    private readonly IQueryHandler<GetUserSelfEducation, IEnumerable<SelfEducationDto>> _getUserSelfEducationHandler;
    private readonly IQueryHandler<GetSelfEducationByYear, IEnumerable<SelfEducationDto>> _getSelfEducationByYearHandler;
    private readonly IQueryHandler<GetCompletedSelfEducation, IEnumerable<SelfEducationDto>> _getCompletedSelfEducationHandler;
    private readonly IQueryHandler<GetTotalCreditHours, int> _getTotalCreditHoursHandler;
    private readonly IQueryHandler<GetTotalQualityScore, decimal> _getTotalQualityScoreHandler;
    private readonly ICommandHandler<CompleteSelfEducation> _completeSelfEducationHandler;
    private readonly ICommandHandler<UpdateSelfEducation> _updateSelfEducationHandler;
    private readonly ICommandHandler<DeleteSelfEducation> _deleteSelfEducationHandler;

    public SelfEducationController(
        ICommandHandler<CreateSelfEducation> createSelfEducationHandler,
        IQueryHandler<GetUserSelfEducation, IEnumerable<SelfEducationDto>> getUserSelfEducationHandler,
        IQueryHandler<GetSelfEducationByYear, IEnumerable<SelfEducationDto>> getSelfEducationByYearHandler,
        IQueryHandler<GetCompletedSelfEducation, IEnumerable<SelfEducationDto>> getCompletedSelfEducationHandler,
        IQueryHandler<GetTotalCreditHours, int> getTotalCreditHoursHandler,
        IQueryHandler<GetTotalQualityScore, decimal> getTotalQualityScoreHandler,
        ICommandHandler<CompleteSelfEducation> completeSelfEducationHandler,
        ICommandHandler<UpdateSelfEducation> updateSelfEducationHandler,
        ICommandHandler<DeleteSelfEducation> deleteSelfEducationHandler) : base()
    {
        _createSelfEducationHandler = createSelfEducationHandler;
        _getUserSelfEducationHandler = getUserSelfEducationHandler;
        _getSelfEducationByYearHandler = getSelfEducationByYearHandler;
        _getCompletedSelfEducationHandler = getCompletedSelfEducationHandler;
        _getTotalCreditHoursHandler = getTotalCreditHoursHandler;
        _getTotalQualityScoreHandler = getTotalQualityScoreHandler;
        _completeSelfEducationHandler = completeSelfEducationHandler;
        _updateSelfEducationHandler = updateSelfEducationHandler;
        _deleteSelfEducationHandler = deleteSelfEducationHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSelfEducation([FromBody] CreateSelfEducationRequest request)
    {
        var command = new CreateSelfEducation(
            new SpecializationId(request.SpecializationId),
            new UserId(request.UserId),
            (SelfEducationType)request.Type,
            request.Year,
            request.Title,
            request.CreditHours,
            request.Description,
            request.Provider);

        await _createSelfEducationHandler.HandleAsync(command);
        return Ok();
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserSelfEducation(int userId, [FromQuery] int? specializationId = null)
    {
        var query = new GetUserSelfEducation(userId, specializationId);
        var selfEducation = await _getUserSelfEducationHandler.HandleAsync(query);
        return Ok(selfEducation);
    }

    [HttpGet("user/{userId}/year/{year}")]
    public async Task<ActionResult<IEnumerable<SelfEducationDto>>> GetSelfEducationByYear(int userId, int year)
    {
        var query = new GetSelfEducationByYear(userId, year);
        var activities = await _getSelfEducationByYearHandler.HandleAsync(query);
        return Ok(activities);
    }

    [HttpGet("user/{userId}/specialization/{specializationId}/completed")]
    public async Task<ActionResult<IEnumerable<SelfEducationDto>>> GetCompletedActivities(int userId, int specializationId)
    {
        var query = new GetCompletedSelfEducation(userId, specializationId);
        var activities = await _getCompletedSelfEducationHandler.HandleAsync(query);
        return Ok(activities);
    }

    [HttpGet("user/{userId}/specialization/{specializationId}/credit-hours")]
    public async Task<ActionResult<int>> GetTotalCreditHours(int userId, int specializationId)
    {
        var query = new GetTotalCreditHours(userId, specializationId);
        var totalHours = await _getTotalCreditHoursHandler.HandleAsync(query);
        return Ok(totalHours);
    }

    [HttpGet("user/{userId}/specialization/{specializationId}/quality-score")]
    public async Task<ActionResult<decimal>> GetTotalQualityScore(int userId, int specializationId)
    {
        var query = new GetTotalQualityScore(userId, specializationId);
        var score = await _getTotalQualityScoreHandler.HandleAsync(query);
        return Ok(score);
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteSelfEducation(Guid id, [FromBody] CompleteSelfEducationRequest request)
    {
        var command = new CompleteSelfEducation(
            new SelfEducationId(id),
            request.CompletedAt,
            request.CertificatePath);

        await _completeSelfEducationHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSelfEducation(Guid id, [FromBody] UpdateSelfEducationRequest request)
    {
        var command = new UpdateSelfEducation(
            new SelfEducationId(id),
            request.Title,
            request.Description,
            request.Provider,
            request.CreditHours,
            request.StartDate,
            request.EndDate,
            request.DurationHours);

        await _updateSelfEducationHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSelfEducation(Guid id)
    {
        var command = new DeleteSelfEducation(new SelfEducationId(id));
        await _deleteSelfEducationHandler.HandleAsync(command);
        return NoContent();
    }
}

public record CreateSelfEducationRequest(
    int SpecializationId,
    int UserId,
    int Type,
    int Year,
    string Title,
    int CreditHours,
    string? Description = null,
    string? Provider = null);

public record CompleteSelfEducationRequest(
    DateTime? CompletedAt = null,
    string? CertificatePath = null);

public record UpdateSelfEducationRequest(
    string Title,
    string? Description,
    string? Provider,
    int CreditHours,
    DateTime? StartDate,
    DateTime? EndDate,
    int? DurationHours);