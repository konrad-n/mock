using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class ApproveCourseHandler : IResultCommandHandler<ApproveCourse>
{
    private readonly ICourseRepository _courseRepository;

    public ApproveCourseHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<Result> HandleAsync(ApproveCourse command, CancellationToken cancellationToken = default)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(new CourseId(command.CourseId));
            if (course is null)
            {
                return Result.Failure($"Course with ID {command.CourseId} not found.");
            }

            course.Approve(command.ApproverName);
            await _courseRepository.UpdateAsync(course);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to approve course: {ex.Message}");
        }
    }
}