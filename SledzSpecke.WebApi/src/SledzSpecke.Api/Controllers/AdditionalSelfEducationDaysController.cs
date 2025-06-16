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
    // TODO: Implement handlers for AdditionalSelfEducationDays operations
    // This controller will be implemented once the handlers are created
    // The entity structure needs ModuleId and InternshipId, not SpecializationId
    
    public AdditionalSelfEducationDaysController()
    {
        // Handlers will be injected once implemented
    }

    [HttpGet("placeholder")]
    public IActionResult Placeholder()
    {
        return Ok(new { message = "AdditionalSelfEducationDays endpoints will be implemented soon" });
    }
}