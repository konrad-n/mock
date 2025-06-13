using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class ApproveCourseHandler : ICommandHandler<ApproveCourse>
{
    private readonly ICourseRepository _courseRepository;

    public ApproveCourseHandler(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task HandleAsync(ApproveCourse command)
    {
        var course = await _courseRepository.GetByIdAsync(new CourseId(command.CourseId));
        if (course is null)
        {
            throw new InvalidOperationException($"Course with ID {command.CourseId} not found.");
        }

        course.Approve(command.ApproverName);
        await _courseRepository.UpdateAsync(course);
    }
}