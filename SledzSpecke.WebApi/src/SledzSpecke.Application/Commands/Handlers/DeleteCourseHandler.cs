using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class DeleteCourseHandler : IResultCommandHandler<DeleteCourse>
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

    public async Task<Result> HandleAsync(DeleteCourse command)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(new CourseId(command.CourseId));
            if (course is null)
            {
                return Result.Failure($"Course with ID {command.CourseId} not found.");
            }

            // Check if the course belongs to the current user
            var userId = _userContextService.GetUserId();
            var user = await _userRepository.GetByIdAsync(new UserId(userId));
            if (user is null || user.SpecializationId != course.SpecializationId)
            {
                return Result.Failure("You can only delete your own courses.");
            }

            // Check if course can be deleted
            if (course.IsApproved)
            {
                return Result.Failure("Cannot delete an approved course.");
            }

            if (course.SyncStatus == SyncStatus.Synced)
            {
                return Result.Failure("Cannot delete a synced course.");
            }

            await _courseRepository.DeleteAsync(new CourseId(command.CourseId));
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete course: {ex.Message}");
        }
    }
}