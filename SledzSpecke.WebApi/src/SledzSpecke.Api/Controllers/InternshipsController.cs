using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;

namespace SledzSpecke.Api.Controllers;

[Authorize]
public class InternshipsController : BaseResultController
{
    private readonly ICommandHandler<CreateInternship, int> _createInternshipHandler;
    private readonly IResultCommandHandler<UpdateInternship> _updateInternshipHandler;
    private readonly IResultCommandHandler<ApproveInternship> _approveInternshipHandler;
    private readonly IResultCommandHandler<MarkInternshipAsCompleted> _markInternshipAsCompletedHandler;
    private readonly IQueryHandler<GetInternships, IEnumerable<InternshipDto>> _getInternshipsHandler;
    private readonly IQueryHandler<GetInternshipById, InternshipDto> _getInternshipByIdHandler;
    private readonly IResultCommandHandler<DeleteInternship> _deleteInternshipHandler;

    public InternshipsController(
        ICommandHandler<CreateInternship, int> createInternshipHandler,
        IResultCommandHandler<UpdateInternship> updateInternshipHandler,
        IResultCommandHandler<ApproveInternship> approveInternshipHandler,
        IResultCommandHandler<MarkInternshipAsCompleted> markInternshipAsCompletedHandler,
        IQueryHandler<GetInternships, IEnumerable<InternshipDto>> getInternshipsHandler,
        IQueryHandler<GetInternshipById, InternshipDto> getInternshipByIdHandler,
        IResultCommandHandler<DeleteInternship> deleteInternshipHandler)
    {
        _createInternshipHandler = createInternshipHandler;
        _updateInternshipHandler = updateInternshipHandler;
        _approveInternshipHandler = approveInternshipHandler;
        _markInternshipAsCompletedHandler = markInternshipAsCompletedHandler;
        _getInternshipsHandler = getInternshipsHandler;
        _getInternshipByIdHandler = getInternshipByIdHandler;
        _deleteInternshipHandler = deleteInternshipHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InternshipDto>>> GetInternships(
        [FromQuery] int specializationId, [FromQuery] int? moduleId = null)
        => await HandleAsync(new GetInternships(specializationId, moduleId), _getInternshipsHandler);

    [HttpGet("{internshipId:int}")]
    public async Task<ActionResult<InternshipDto>> GetInternship(int internshipId)
        => await HandleAsync(new GetInternshipById(internshipId), _getInternshipByIdHandler);

    [HttpPost]
    public async Task<ActionResult<int>> CreateInternship([FromBody] CreateInternship command)
    {
        var internshipId = await _createInternshipHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetInternships), new { specializationId = command.SpecializationId }, internshipId);
    }

    [HttpPut("{internshipId:int}")]
    public async Task<ActionResult> UpdateInternship(int internshipId, [FromBody] UpdateInternshipRequest request)
    {
        var command = new UpdateInternship(
            internshipId,
            request.InstitutionName,
            request.DepartmentName,
            request.SupervisorName,
            request.StartDate,
            request.EndDate,
            request.ModuleId);

        return await HandleAsync(command, _updateInternshipHandler);
    }

    [HttpPost("{internshipId:int}/approve")]
    public async Task<ActionResult> ApproveInternship(int internshipId, [FromBody] ApproveInternshipRequest request)
    {
        var command = new ApproveInternship(internshipId, request.ApproverName);
        return await HandleAsync(command, _approveInternshipHandler);
    }

    [HttpPost("{internshipId:int}/complete")]
    public async Task<ActionResult> MarkInternshipAsCompleted(int internshipId)
    {
        var command = new MarkInternshipAsCompleted(internshipId);
        return await HandleAsync(command, _markInternshipAsCompletedHandler);
    }

    [HttpDelete("{internshipId:int}")]
    public async Task<ActionResult> DeleteInternship(int internshipId)
    {
        var command = new DeleteInternship(internshipId);
        return await HandleAsync(command, _deleteInternshipHandler);
    }
}

public class UpdateInternshipRequest
{
    public string? InstitutionName { get; set; }
    public string? DepartmentName { get; set; }
    public string? SupervisorName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? ModuleId { get; set; }
}

public class ApproveInternshipRequest
{
    public string ApproverName { get; set; } = string.Empty;
}