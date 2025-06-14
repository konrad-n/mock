using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;

namespace SledzSpecke.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : BaseController
{
    private readonly IQueryHandler<GetDashboardOverview, DashboardOverviewDto> _getDashboardOverviewHandler;
    private readonly IQueryHandler<GetModuleProgress, SpecializationStatisticsDto> _getModuleProgressHandler;
    private readonly IUserContextService _userContextService;

    public DashboardController(
        IQueryHandler<GetDashboardOverview, DashboardOverviewDto> getDashboardOverviewHandler,
        IQueryHandler<GetModuleProgress, SpecializationStatisticsDto> getModuleProgressHandler,
        IUserContextService userContextService)
    {
        _getDashboardOverviewHandler = getDashboardOverviewHandler;
        _getModuleProgressHandler = getModuleProgressHandler;
        _userContextService = userContextService;
    }

    [HttpGet("overview")]
    public async Task<ActionResult<DashboardOverviewDto>> GetDashboardOverview()
    {
        var userId = _userContextService.GetUserId();
        return await HandleAsync(new GetDashboardOverview(userId), _getDashboardOverviewHandler);
    }

    [HttpGet("progress/{specializationId:int}")]
    public async Task<ActionResult<SpecializationStatisticsDto>> GetProgress(int specializationId, [FromQuery] int? moduleId = null)
    {
        return await HandleAsync(new GetModuleProgress(specializationId, moduleId), _getModuleProgressHandler);
    }

    [HttpGet("statistics/{specializationId:int}")]
    public async Task<ActionResult<SpecializationStatisticsDto>> GetStatistics(int specializationId, [FromQuery] int? moduleId = null)
    {
        // Same as progress but can be extended with more detailed statistics
        return await HandleAsync(new GetModuleProgress(specializationId, moduleId), _getModuleProgressHandler);
    }
}