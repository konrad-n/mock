using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;

namespace SledzSpecke.Api.Controllers;

[Authorize]
public class CoursesController : BaseController
{
    private readonly ICommandHandler<CreateCourse, int> _createCourseHandler;
    private readonly ICommandHandler<ApproveCourse> _approveCourseHandler;
    private readonly IQueryHandler<GetCourses, IEnumerable<CourseDto>> _getCoursesHandler;

    public CoursesController(
        ICommandHandler<CreateCourse, int> createCourseHandler,
        ICommandHandler<ApproveCourse> approveCourseHandler,
        IQueryHandler<GetCourses, IEnumerable<CourseDto>> getCoursesHandler)
    {
        _createCourseHandler = createCourseHandler;
        _approveCourseHandler = approveCourseHandler;
        _getCoursesHandler = getCoursesHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses(
        [FromQuery] int specializationId, [FromQuery] int? moduleId = null, [FromQuery] string? courseType = null)
        => await HandleAsync(new GetCourses(specializationId, moduleId, courseType), _getCoursesHandler);

    [HttpPost]
    public async Task<ActionResult<int>> CreateCourse([FromBody] CreateCourse command)
    {
        var courseId = await _createCourseHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetCourses), new { specializationId = command.SpecializationId }, courseId);
    }

    [HttpPost("{courseId:int}/approve")]
    public async Task<ActionResult> ApproveCourse(int courseId, [FromBody] ApproveCourseRequest request)
    {
        var command = new ApproveCourse(courseId, request.ApproverName);
        return await HandleAsync(command, _approveCourseHandler);
    }
}

public class ApproveCourseRequest
{
    public string ApproverName { get; set; } = string.Empty;
}