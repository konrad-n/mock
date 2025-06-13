using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;

namespace SledzSpecke.Api.Controllers;

[Authorize]
public class SpecializationsController : BaseController
{
    private readonly IQueryHandler<GetSpecialization, SpecializationDto> _getSpecializationHandler;
    private readonly IQueryHandler<GetSpecializationStatistics, SpecializationStatisticsDto> _getStatisticsHandler;

    public SpecializationsController(
        IQueryHandler<GetSpecialization, SpecializationDto> getSpecializationHandler,
        IQueryHandler<GetSpecializationStatistics, SpecializationStatisticsDto> getStatisticsHandler)
    {
        _getSpecializationHandler = getSpecializationHandler;
        _getStatisticsHandler = getStatisticsHandler;
    }

    [HttpGet("{specializationId:int}")]
    public async Task<ActionResult<SpecializationDto>> GetSpecialization(int specializationId)
        => await HandleAsync(new GetSpecialization(specializationId), _getSpecializationHandler);

    [HttpGet("{specializationId:int}/statistics")]
    public async Task<ActionResult<SpecializationStatisticsDto>> GetStatistics(int specializationId)
        => await HandleAsync(new GetSpecializationStatistics(specializationId), _getStatisticsHandler);
}