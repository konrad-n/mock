using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EducationalActivitiesController : ControllerBase
{
    private readonly IQueryHandler<GetEducationalActivities, IEnumerable<EducationalActivityDto>> _getActivitiesHandler;
    private readonly IQueryHandler<GetEducationalActivityById, EducationalActivityDto?> _getActivityByIdHandler;
    private readonly IQueryHandler<GetEducationalActivitiesByType, IEnumerable<EducationalActivityDto>> _getActivitiesByTypeHandler;
    private readonly IResultCommandHandler<CreateEducationalActivity, int> _createHandler;
    private readonly IResultCommandHandler<UpdateEducationalActivity> _updateHandler;
    private readonly IResultCommandHandler<DeleteEducationalActivity> _deleteHandler;

    public EducationalActivitiesController(
        IQueryHandler<GetEducationalActivities, IEnumerable<EducationalActivityDto>> getActivitiesHandler,
        IQueryHandler<GetEducationalActivityById, EducationalActivityDto?> getActivityByIdHandler,
        IQueryHandler<GetEducationalActivitiesByType, IEnumerable<EducationalActivityDto>> getActivitiesByTypeHandler,
        IResultCommandHandler<CreateEducationalActivity, int> createHandler,
        IResultCommandHandler<UpdateEducationalActivity> updateHandler,
        IResultCommandHandler<DeleteEducationalActivity> deleteHandler)
    {
        _getActivitiesHandler = getActivitiesHandler;
        _getActivityByIdHandler = getActivityByIdHandler;
        _getActivitiesByTypeHandler = getActivitiesByTypeHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    /// <summary>
    /// Get all educational activities for a specialization
    /// </summary>
    [HttpGet("specialization/{specializationId}")]
    public async Task<ActionResult<IEnumerable<EducationalActivityDto>>> GetActivities(int specializationId)
    {
        var activities = await _getActivitiesHandler.HandleAsync(new GetEducationalActivities(specializationId));
        return Ok(activities);
    }

    /// <summary>
    /// Get educational activity by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EducationalActivityDto>> GetActivity(int id)
    {
        var activity = await _getActivityByIdHandler.HandleAsync(new GetEducationalActivityById(id));
        if (activity is null)
            return NotFound();
        
        return Ok(activity);
    }

    /// <summary>
    /// Get educational activities by type
    /// </summary>
    [HttpGet("specialization/{specializationId}/type/{type}")]
    public async Task<ActionResult<IEnumerable<EducationalActivityDto>>> GetActivitiesByType(
        int specializationId, 
        string type)
    {
        var activities = await _getActivitiesByTypeHandler.HandleAsync(
            new GetEducationalActivitiesByType(specializationId, type));
        return Ok(activities);
    }

    /// <summary>
    /// Create a new educational activity
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<int>> CreateActivity(CreateEducationalActivity command)
    {
        var result = await _createHandler.HandleAsync(command);
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }
        
        return CreatedAtAction(nameof(GetActivity), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Update an educational activity
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateActivity(int id, UpdateEducationalActivity command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { error = "ID mismatch" });
        }

        var result = await _updateHandler.HandleAsync(command);
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }
        
        return Ok();
    }

    /// <summary>
    /// Delete an educational activity
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteActivity(int id)
    {
        var result = await _deleteHandler.HandleAsync(new DeleteEducationalActivity(id));
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }
        
        return NoContent();
    }
}