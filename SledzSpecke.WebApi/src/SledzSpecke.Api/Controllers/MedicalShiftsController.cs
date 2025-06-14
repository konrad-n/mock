using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;

namespace SledzSpecke.Api.Controllers;

[Authorize]
public class MedicalShiftsController : BaseController
{
    private readonly ICommandHandler<AddMedicalShift, int> _addMedicalShiftHandler;
    private readonly ICommandHandler<UpdateMedicalShift> _updateMedicalShiftHandler;
    private readonly ICommandHandler<DeleteMedicalShift> _deleteMedicalShiftHandler;
    private readonly IQueryHandler<GetUserMedicalShifts, IEnumerable<MedicalShiftDto>> _getUserMedicalShiftsHandler;
    private readonly IQueryHandler<GetMedicalShiftById, MedicalShiftDto> _getMedicalShiftByIdHandler;
    private readonly IQueryHandler<GetMedicalShiftStatistics, MedicalShiftSummaryDto> _getMedicalShiftStatisticsHandler;
    private readonly IUserContextService _userContextService;

    public MedicalShiftsController(
        ICommandHandler<AddMedicalShift, int> addMedicalShiftHandler,
        ICommandHandler<UpdateMedicalShift> updateMedicalShiftHandler,
        ICommandHandler<DeleteMedicalShift> deleteMedicalShiftHandler,
        IQueryHandler<GetUserMedicalShifts, IEnumerable<MedicalShiftDto>> getUserMedicalShiftsHandler,
        IQueryHandler<GetMedicalShiftById, MedicalShiftDto> getMedicalShiftByIdHandler,
        IQueryHandler<GetMedicalShiftStatistics, MedicalShiftSummaryDto> getMedicalShiftStatisticsHandler,
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
        return await HandleAsync(query, _getUserMedicalShiftsHandler);
    }

    [HttpGet("{shiftId:int}")]
    public async Task<ActionResult<MedicalShiftDto>> GetMedicalShiftById(int shiftId)
    {
        var query = new GetMedicalShiftById(shiftId);
        return await HandleAsync(query, _getMedicalShiftByIdHandler);
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
    public async Task<ActionResult<MedicalShiftSummaryDto>> GetMedicalShiftStatistics(
        [FromQuery] int? year = null,
        [FromQuery] int? internshipRequirementId = null)
    {
        var query = new GetMedicalShiftStatistics(year, internshipRequirementId);
        return await HandleAsync(query, _getMedicalShiftStatisticsHandler);
    }
}

public class AddMedicalShiftRequest
{
    public int InternshipId { get; set; }
    public DateTime Date { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Year { get; set; }
}

public class UpdateMedicalShiftRequest
{
    public DateTime? Date { get; set; }
    public int? Hours { get; set; }
    public int? Minutes { get; set; }
    public string? Location { get; set; }
}