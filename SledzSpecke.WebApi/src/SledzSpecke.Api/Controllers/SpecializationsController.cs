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

    // TODO: Implement once SMK detail handlers are created
    /*
    [HttpGet("{specializationId:int}/smk-details")]
    [ProducesResponseType(typeof(SpecializationSmkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SpecializationSmkDto>> GetSmkDetails(int specializationId)
        => await HandleAsync(new GetSpecializationSmkDetails(specializationId), _getSmkDetailsHandler);

    [HttpGet("user/{userId:int}")]
    [ProducesResponseType(typeof(IEnumerable<SpecializationSmkDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SpecializationSmkDto>>> GetByUser(int userId)
        => await HandleAsync(new GetSpecializationsByUser(userId), _getByUserHandler);
    */
}