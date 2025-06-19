using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.Commands.AddMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.Commands.UpdateMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.Commands.DeleteMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.Queries.GetMedicalShiftById;
using SledzSpecke.Application.Features.MedicalShifts.Queries.GetMedicalShiftStatistics;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using System.ComponentModel.DataAnnotations;

namespace SledzSpecke.Api.Controllers;

[Authorize]
public class MedicalShiftsController : BaseController
{
    private readonly ICommandHandler<AddMedicalShift, int> _addMedicalShiftHandler;
    private readonly ICommandHandler<UpdateMedicalShift> _updateMedicalShiftHandler;
    private readonly ICommandHandler<DeleteMedicalShift> _deleteMedicalShiftHandler;
    private readonly IQueryHandler<GetUserMedicalShifts, IEnumerable<MedicalShiftDto>> _getUserMedicalShiftsHandler;
    private readonly IResultQueryHandler<GetMedicalShiftById, MedicalShiftDto> _getMedicalShiftByIdHandler;
    private readonly IResultQueryHandler<GetMedicalShiftStatistics, MedicalShiftStatisticsDto> _getMedicalShiftStatisticsHandler;
    private readonly IUserContextService _userContextService;

    public MedicalShiftsController(
        ICommandHandler<AddMedicalShift, int> addMedicalShiftHandler,
        ICommandHandler<UpdateMedicalShift> updateMedicalShiftHandler,
        ICommandHandler<DeleteMedicalShift> deleteMedicalShiftHandler,
        IQueryHandler<GetUserMedicalShifts, IEnumerable<MedicalShiftDto>> getUserMedicalShiftsHandler,
        IResultQueryHandler<GetMedicalShiftById, MedicalShiftDto> getMedicalShiftByIdHandler,
        IResultQueryHandler<GetMedicalShiftStatistics, MedicalShiftStatisticsDto> getMedicalShiftStatisticsHandler,
        IUserContextService userContextService)
    {
        _addMedicalShiftHandler = addMedicalShiftHandler;
        _updateMedicalShiftHandler = updateMedicalShiftHandler;
        _deleteMedicalShiftHandler = deleteMedicalShiftHandler;
        _getUserMedicalShiftsHandler = getUserMedicalShiftsHandler;
        _getMedicalShiftByIdHandler = getMedicalShiftByIdHandler;
        _getMedicalShiftStatisticsHandler = getMedicalShiftStatisticsHandler;
        _userContextService = userContextService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicalShiftDto>>> GetUserMedicalShifts(
        [FromQuery] int? internshipId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var userId = _userContextService.GetUserId();
        var query = new GetUserMedicalShifts(userId, internshipId, startDate, endDate);
        var result = await _getUserMedicalShiftsHandler.HandleAsync(query);
        return Ok(result);
    }

    [HttpGet("{shiftId:int}")]
    public async Task<ActionResult<MedicalShiftDto>> GetMedicalShiftById(int shiftId)
    {
        var query = new GetMedicalShiftById(shiftId);
        var result = await _getMedicalShiftByIdHandler.HandleAsync(query);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
            
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddMedicalShift([FromBody] AddMedicalShiftRequest request)
    {
        var command = new AddMedicalShift(
            request.InternshipId,
            request.Date,
            request.Hours,
            request.Minutes,
            request.Location,
            request.Year);

        var shiftId = await _addMedicalShiftHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetMedicalShiftById), new { shiftId }, shiftId);
    }

    [HttpPut("{shiftId:int}")]
    public async Task<ActionResult> UpdateMedicalShift(int shiftId, [FromBody] UpdateMedicalShiftRequest request)
    {
        var command = new UpdateMedicalShift(
            shiftId,
            request.Date,
            request.Hours,
            request.Minutes,
            request.Location);

        return await HandleAsync(command, _updateMedicalShiftHandler);
    }

    [HttpDelete("{shiftId:int}")]
    public async Task<ActionResult> DeleteMedicalShift(int shiftId)
    {
        var command = new DeleteMedicalShift(shiftId);
        return await HandleAsync(command, _deleteMedicalShiftHandler);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<MedicalShiftStatisticsDto>> GetMedicalShiftStatistics(
        [FromQuery] int year,
        [FromQuery] int? month = null)
    {
        var userId = _userContextService.GetUserId();
        var query = new GetMedicalShiftStatistics { UserId = userId, Year = year, Month = month };
        var result = await _getMedicalShiftStatisticsHandler.HandleAsync(query);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
            
        return Ok(result.Value);
    }
}

public class AddMedicalShiftRequest
{
    [Required]
    public int InternshipId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    [Range(0, 24, ErrorMessage = "Hours must be between 0 and 24")]
    public int Hours { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Minutes cannot be negative")]
    public int Minutes { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 6, ErrorMessage = "Medical year must be between 1 and 6")]
    public int Year { get; set; }
}

public class UpdateMedicalShiftRequest
{
    public DateTime? Date { get; set; }
    
    [Range(0, 24, ErrorMessage = "Hours must be between 0 and 24")]
    public int? Hours { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Minutes cannot be negative")]
    public int? Minutes { get; set; }
    
    [MaxLength(200)]
    public string? Location { get; set; }
}