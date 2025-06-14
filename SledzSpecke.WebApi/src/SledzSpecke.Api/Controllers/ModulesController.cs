using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;

namespace SledzSpecke.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ModulesController : BaseController
{
    private readonly ICommandHandler<SwitchModule> _switchModuleHandler;
    private readonly IQueryHandler<GetAvailableModules, IEnumerable<ModuleListDto>> _getAvailableModulesHandler;

    public ModulesController(
        ICommandHandler<SwitchModule> switchModuleHandler,
        IQueryHandler<GetAvailableModules, IEnumerable<ModuleListDto>> getAvailableModulesHandler)
    {
        _switchModuleHandler = switchModuleHandler;
        _getAvailableModulesHandler = getAvailableModulesHandler;
    }

    [HttpGet("specialization/{specializationId:int}")]
    public async Task<ActionResult<IEnumerable<ModuleListDto>>> GetAvailableModules(int specializationId)
        => await HandleAsync(new GetAvailableModules(specializationId), _getAvailableModulesHandler);

    [HttpPut("switch")]
    public async Task<ActionResult> SwitchModule([FromBody] SwitchModuleRequest request)
    {
        var command = new SwitchModule(request.SpecializationId, request.ModuleId);
        return await HandleAsync(command, _switchModuleHandler);
    }
}

public class SwitchModuleRequest
{
    public int SpecializationId { get; set; }
    public int ModuleId { get; set; }
}