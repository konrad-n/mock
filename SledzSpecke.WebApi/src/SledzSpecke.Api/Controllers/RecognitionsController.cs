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
    private readonly ICommandHandler<ApproveRecognition> _approveRecognitionHandler;
    private readonly IQueryHandler<GetTotalReductionDays, int> _getTotalReductionDaysHandler;
    private readonly ICommandHandler<UpdateRecognition> _updateRecognitionHandler;
    private readonly ICommandHandler<DeleteRecognition> _deleteRecognitionHandler;

    public RecognitionsController(
        ICommandHandler<CreateRecognition> createRecognitionHandler,
        IQueryHandler<GetUserRecognitions, IEnumerable<RecognitionDto>> getUserRecognitionsHandler,
        ICommandHandler<ApproveRecognition> approveRecognitionHandler,
        IQueryHandler<GetTotalReductionDays, int> getTotalReductionDaysHandler,
        ICommandHandler<UpdateRecognition> updateRecognitionHandler,
        ICommandHandler<DeleteRecognition> deleteRecognitionHandler) : base()
    {
        _createRecognitionHandler = createRecognitionHandler;
        _getUserRecognitionsHandler = getUserRecognitionsHandler;
        _approveRecognitionHandler = approveRecognitionHandler;
        _getTotalReductionDaysHandler = getTotalReductionDaysHandler;
        _updateRecognitionHandler = updateRecognitionHandler;
        _deleteRecognitionHandler = deleteRecognitionHandler;
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
        var command = new ApproveRecognition(new RecognitionId(id), request.ApprovedBy);
        await _approveRecognitionHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpGet("user/{userId}/specialization/{specializationId}/total-reduction")]
    public async Task<ActionResult<int>> GetTotalReductionDays(int userId, int specializationId)
    {
        var query = new GetTotalReductionDays(userId, specializationId);
        var totalDays = await _getTotalReductionDaysHandler.HandleAsync(query);
        return Ok(totalDays);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRecognition(Guid id, [FromBody] UpdateRecognitionRequest request)
    {
        var command = new UpdateRecognition(
            new RecognitionId(id),
            (RecognitionType)request.Type,
            request.Title,
            request.Description,
            request.Institution,
            request.StartDate,
            request.EndDate,
            request.DaysReduction);

        await _updateRecognitionHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecognition(Guid id)
    {
        var command = new DeleteRecognition(new RecognitionId(id));
        await _deleteRecognitionHandler.HandleAsync(command);
        return NoContent();
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

public record UpdateRecognitionRequest(
    int Type,
    string Title,
    string? Description,
    string? Institution,
    DateTime StartDate,
    DateTime EndDate,
    int DaysReduction);