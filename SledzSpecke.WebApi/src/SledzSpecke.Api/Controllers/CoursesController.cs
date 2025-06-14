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
    private readonly ICommandHandler<UpdateCourse> _updateCourseHandler;
    private readonly ICommandHandler<DeleteCourse> _deleteCourseHandler;
    private readonly ICommandHandler<CompleteCourse> _completeCourseHandler;
    private readonly IQueryHandler<GetCourses, IEnumerable<CourseDto>> _getCoursesHandler;
    private readonly IQueryHandler<GetCourseById, CourseDto> _getCourseByIdHandler;

    public CoursesController(
        ICommandHandler<CreateCourse, int> createCourseHandler,
        ICommandHandler<ApproveCourse> approveCourseHandler,
        ICommandHandler<UpdateCourse> updateCourseHandler,
        ICommandHandler<DeleteCourse> deleteCourseHandler,
        ICommandHandler<CompleteCourse> completeCourseHandler,
        IQueryHandler<GetCourses, IEnumerable<CourseDto>> getCoursesHandler,
        IQueryHandler<GetCourseById, CourseDto> getCourseByIdHandler)
    {
        _createCourseHandler = createCourseHandler;
        _approveCourseHandler = approveCourseHandler;
        _updateCourseHandler = updateCourseHandler;
        _deleteCourseHandler = deleteCourseHandler;
        _completeCourseHandler = completeCourseHandler;
        _getCoursesHandler = getCoursesHandler;
        _getCourseByIdHandler = getCourseByIdHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses(
        [FromQuery] int specializationId, [FromQuery] int? moduleId = null, [FromQuery] string? courseType = null)
        => await HandleAsync(new GetCourses(specializationId, moduleId, courseType), _getCoursesHandler);

    [HttpGet("{courseId:int}")]
    public async Task<ActionResult<CourseDto>> GetCourse(int courseId)
        => await HandleAsync(new GetCourseById(courseId), _getCourseByIdHandler);

    [HttpPost]
    public async Task<ActionResult<int>> CreateCourse([FromBody] CreateCourse command)
    {
        var courseId = await _createCourseHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetCourses), new { specializationId = command.SpecializationId }, courseId);
    }

    [HttpPut("{courseId:int}")]
    public async Task<ActionResult> UpdateCourse(int courseId, [FromBody] UpdateCourseRequest request)
    {
        var command = new UpdateCourse(
            courseId,
            request.CourseName,
            request.CourseNumber,
            request.InstitutionName,
            request.CompletionDate,
            request.HasCertificate,
            request.CertificateNumber,
            request.ModuleId);
        return await HandleAsync(command, _updateCourseHandler);
    }

    [HttpDelete("{courseId:int}")]
    public async Task<ActionResult> DeleteCourse(int courseId)
    {
        var command = new DeleteCourse(courseId);
        return await HandleAsync(command, _deleteCourseHandler);
    }

    [HttpPost("{courseId:int}/complete")]
    public async Task<ActionResult> CompleteCourse(int courseId, [FromBody] CompleteCourseRequest request)
    {
        var command = new CompleteCourse(
            courseId,
            request.CompletionDate,
            request.HasCertificate,
            request.CertificateNumber);
        return await HandleAsync(command, _completeCourseHandler);
    }

    [HttpPost("{courseId:int}/approve")]
    public async Task<ActionResult> ApproveCourse(int courseId, [FromBody] ApproveCourseRequest request)
    {
        var command = new ApproveCourse(courseId, request.ApproverName);
        return await HandleAsync(command, _approveCourseHandler);
    }
}

public class UpdateCourseRequest
{
    public string? CourseName { get; set; }
    public string? CourseNumber { get; set; }
    public string? InstitutionName { get; set; }
    public DateTime? CompletionDate { get; set; }
    public bool? HasCertificate { get; set; }
    public string? CertificateNumber { get; set; }
    public int? ModuleId { get; set; }
}

public class CompleteCourseRequest
{
    public DateTime CompletionDate { get; set; }
    public bool HasCertificate { get; set; }
    public string? CertificateNumber { get; set; }
}

public class ApproveCourseRequest
{
    public string ApproverName { get; set; } = string.Empty;
}