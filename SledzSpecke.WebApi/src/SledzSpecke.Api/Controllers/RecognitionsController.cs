using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Queries;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecognitionsController : BaseController
{
    private readonly ICommandHandler<CreateRecognition> _createRecognitionHandler;
    private readonly IQueryHandler<GetUserRecognitions, IEnumerable<RecognitionDto>> _getUserRecognitionsHandler;

    public RecognitionsController(
        ICommandHandler<CreateRecognition> createRecognitionHandler,
        IQueryHandler<GetUserRecognitions, IEnumerable<RecognitionDto>> getUserRecognitionsHandler) : base()
    {
        _createRecognitionHandler = createRecognitionHandler;
        _getUserRecognitionsHandler = getUserRecognitionsHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecognition([FromBody] CreateRecognitionRequest request)
    {
        var command = new CreateRecognition(
            new SpecializationId(request.SpecializationId),
            new UserId(request.UserId),
            (RecognitionType)request.Type,
            request.Title,
            request.StartDate,
            request.EndDate,
            request.DaysReduction,
            request.Description,
            request.Institution);

        await _createRecognitionHandler.HandleAsync(command);
        return Ok();
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserRecognitions(int userId, [FromQuery] int? specializationId = null)
    {
        var query = new GetUserRecognitions(userId, specializationId);
        var recognitions = await _getUserRecognitionsHandler.HandleAsync(query);
        return Ok(recognitions);
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveRecognition(Guid id, [FromBody] ApproveRecognitionRequest request)
    {
        // This would need an approve recognition command and handler
        return Ok();
    }

    [HttpGet("user/{userId}/specialization/{specializationId}/total-reduction")]
    public async Task<IActionResult> GetTotalReductionDays(Guid userId, Guid specializationId)
    {
        // This would need a query handler implementation
        return Ok();
    }
}

public record CreateRecognitionRequest(
    int SpecializationId,
    int UserId,
    int Type,
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    int DaysReduction,
    string? Description = null,
    string? Institution = null);

public record ApproveRecognitionRequest(int ApprovedBy);