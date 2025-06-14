using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class DeleteCourseHandler : ICommandHandler<DeleteCourse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;

    public DeleteCourseHandler(
        ICourseRepository courseRepository,
        IUserContextService userContextService,
        IUserRepository userRepository)
    {
        _courseRepository = courseRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
    }

    public async Task HandleAsync(DeleteCourse command)
    {
        var course = await _courseRepository.GetByIdAsync(new CourseId(command.CourseId));
        if (course is null)
        {
            throw new NotFoundException($"Course with ID {command.CourseId} not found.");
        }

        // Check if the course belongs to the current user
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user is null || user.SpecializationId != course.SpecializationId)
        {
            throw new UnauthorizedAccessException("You can only delete your own courses.");
        }

        // Check if course can be deleted
        if (course.IsApproved)
        {
            throw new InvalidOperationException("Cannot delete an approved course.");
        }

        if (course.SyncStatus == SyncStatus.Synced)
        {
            throw new InvalidOperationException("Cannot delete a synced course.");
        }

        await _courseRepository.DeleteAsync(new CourseId(command.CourseId));
    }
}