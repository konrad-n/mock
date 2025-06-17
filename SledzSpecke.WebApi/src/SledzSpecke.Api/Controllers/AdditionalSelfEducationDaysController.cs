using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/additional-self-education-days")]
public class AdditionalSelfEducationDaysController : BaseResultController
{
    private readonly ICommandHandler<AddAdditionalSelfEducationDays, int> _addHandler;
    private readonly IResultCommandHandler<UpdateAdditionalSelfEducationDays> _updateHandler;
    private readonly IResultCommandHandler<DeleteAdditionalSelfEducationDays> _deleteHandler;
    private readonly IQueryHandler<GetAdditionalSelfEducationDaysBySpecialization, IEnumerable<AdditionalSelfEducationDaysDto>> _getBySpecializationHandler;
    private readonly IQueryHandler<GetAdditionalSelfEducationDaysByModule, IEnumerable<AdditionalSelfEducationDaysDto>> _getByModuleHandler;
    private readonly IQueryHandler<GetAdditionalSelfEducationDaysById, AdditionalSelfEducationDaysDto> _getByIdHandler;

    public AdditionalSelfEducationDaysController(
        ICommandHandler<AddAdditionalSelfEducationDays, int> addHandler,
        IResultCommandHandler<UpdateAdditionalSelfEducationDays> updateHandler,
        IResultCommandHandler<DeleteAdditionalSelfEducationDays> deleteHandler,
        IQueryHandler<GetAdditionalSelfEducationDaysBySpecialization, IEnumerable<AdditionalSelfEducationDaysDto>> getBySpecializationHandler,
        IQueryHandler<GetAdditionalSelfEducationDaysByModule, IEnumerable<AdditionalSelfEducationDaysDto>> getByModuleHandler,
        IQueryHandler<GetAdditionalSelfEducationDaysById, AdditionalSelfEducationDaysDto> getByIdHandler)
    {
        _addHandler = addHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getBySpecializationHandler = getBySpecializationHandler;
        _getByModuleHandler = getByModuleHandler;
        _getByIdHandler = getByIdHandler;
    }

    /// <summary>
    /// Get all additional self-education days for a specialization
    /// </summary>
    /// <param name="specializationId">The ID of the specialization</param>
    /// <returns>List of additional self-education days grouped by year</returns>
    [HttpGet("specialization/{specializationId}")]
    [ProducesResponseType(typeof(IEnumerable<AdditionalSelfEducationDaysDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySpecialization(int specializationId)
    {
        var query = new GetAdditionalSelfEducationDaysBySpecialization(specializationId);
        var result = await _getBySpecializationHandler.HandleAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Add additional self-education days
    /// </summary>
    /// <param name="dto">The creation data</param>
    /// <returns>The ID of the created record</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] CreateAdditionalSelfEducationDaysDto dto)
    {
        var command = new AddAdditionalSelfEducationDays(
            dto.SpecializationId,
            dto.Year,
            dto.DaysUsed,
            dto.Comment);

        var id = await _addHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Update additional self-education days
    /// </summary>
    /// <param name="id">The ID of the record to update</param>
    /// <param name="dto">The update data</param>
    /// <returns>Success or error result</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAdditionalSelfEducationDaysDto dto)
    {
        var command = new UpdateAdditionalSelfEducationDays(
            id,
            dto.DaysUsed,
            dto.Comment);

        var result = await _updateHandler.HandleAsync(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete additional self-education days
    /// </summary>
    /// <param name="id">The ID of the record to delete</param>
    /// <returns>Success or error result</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteAdditionalSelfEducationDays(id);
        var result = await _deleteHandler.HandleAsync(command);
        return HandleResult(result);
    }

    /// <summary>
    /// Get additional self-education days by module
    /// </summary>
    /// <param name="moduleId">Module identifier</param>
    /// <returns>List of additional self-education days</returns>
    [HttpGet("module/{moduleId:int}")]
    [ProducesResponseType(typeof(IEnumerable<AdditionalSelfEducationDaysDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AdditionalSelfEducationDaysDto>>> GetByModule(int moduleId)
    {
        var result = await _getByModuleHandler.HandleAsync(new GetAdditionalSelfEducationDaysByModule(moduleId));
        return Ok(result);
    }

    /// <summary>
    /// Get additional self-education days by ID
    /// </summary>
    /// <param name="id">Record identifier</param>
    /// <returns>Additional self-education days details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AdditionalSelfEducationDaysDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdditionalSelfEducationDaysDto>> GetById(int id)
    {
        var result = await _getByIdHandler.HandleAsync(new GetAdditionalSelfEducationDaysById(id));
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}