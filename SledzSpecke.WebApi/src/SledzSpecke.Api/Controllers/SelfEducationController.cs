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

    public SelfEducationController(
        ICommandHandler<CreateSelfEducation> createSelfEducationHandler,
        IQueryHandler<GetUserSelfEducation, IEnumerable<SelfEducationDto>> getUserSelfEducationHandler) : base()
    {
        _createSelfEducationHandler = createSelfEducationHandler;
        _getUserSelfEducationHandler = getUserSelfEducationHandler;
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
    public async Task<IActionResult> GetSelfEducationByYear(Guid userId, int year)
    {
        // This would need a query handler implementation
        return Ok();
    }

    [HttpGet("user/{userId}/specialization/{specializationId}/completed")]
    public async Task<IActionResult> GetCompletedActivities(Guid userId, Guid specializationId)
    {
        // This would need a query handler implementation
        return Ok();
    }

    [HttpGet("user/{userId}/specialization/{specializationId}/credit-hours")]
    public async Task<IActionResult> GetTotalCreditHours(Guid userId, Guid specializationId)
    {
        // This would need a query handler implementation
        return Ok();
    }

    [HttpGet("user/{userId}/specialization/{specializationId}/quality-score")]
    public async Task<IActionResult> GetTotalQualityScore(Guid userId, Guid specializationId)
    {
        // This would need a query handler implementation
        return Ok();
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteSelfEducation(Guid id, [FromBody] CompleteSelfEducationRequest request)
    {
        // This would need a complete self-education command and handler
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSelfEducation(Guid id, [FromBody] UpdateSelfEducationRequest request)
    {
        // This would need an update self-education command and handler
        return Ok();
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