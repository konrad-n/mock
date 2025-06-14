using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Queries;

namespace SledzSpecke.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalculationsController : BaseController
{
    private readonly IQueryHandler<CalculateInternshipDays, int> _calculateInternshipDaysHandler;

    public CalculationsController(
        IQueryHandler<CalculateInternshipDays, int> calculateInternshipDaysHandler)
    {
        _calculateInternshipDaysHandler = calculateInternshipDaysHandler;
    }

    [HttpPost("internship-days")]
    public async Task<ActionResult<int>> CalculateInternshipDays([FromBody] CalculateDaysRequest request)
    {
        var query = new CalculateInternshipDays(request.StartDate, request.EndDate);
        var days = await _calculateInternshipDaysHandler.HandleAsync(query);
        return Ok(new { days });
    }

    [HttpPost("normalize-time")]
    public ActionResult<TimeNormalizationResponse> NormalizeTime([FromBody] TimeNormalizationRequest request)
    {
        // Time normalization logic - converts minutes > 59 to hours
        var totalMinutes = request.Hours * 60 + request.Minutes;
        var normalizedHours = totalMinutes / 60;
        var normalizedMinutes = totalMinutes % 60;

        return Ok(new TimeNormalizationResponse
        {
            Hours = normalizedHours,
            Minutes = normalizedMinutes,
            TotalMinutes = totalMinutes
        });
    }

    [HttpGet("required-shift-hours")]
    public ActionResult<RequiredShiftHoursResponse> GetRequiredShiftHours(
        [FromQuery] int durationInDays, 
        [FromQuery] decimal weeklyHours = 10.083m)
    {
        // Calculate required shift hours based on module duration
        // Formula: (duration in days / 7) * weekly hours
        var weeks = durationInDays / 7.0;
        var requiredHours = (int)Math.Ceiling(weeks * (double)weeklyHours);

        return Ok(new RequiredShiftHoursResponse
        {
            RequiredHours = requiredHours,
            Weeks = weeks,
            WeeklyHours = weeklyHours
        });
    }
}

public class CalculateDaysRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class TimeNormalizationRequest
{
    public int Hours { get; set; }
    public int Minutes { get; set; }
}

public class TimeNormalizationResponse
{
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int TotalMinutes { get; set; }
}

public class RequiredShiftHoursResponse
{
    public int RequiredHours { get; set; }
    public double Weeks { get; set; }
    public decimal WeeklyHours { get; set; }
}