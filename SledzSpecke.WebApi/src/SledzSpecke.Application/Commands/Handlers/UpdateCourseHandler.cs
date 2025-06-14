using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class UpdateCourseHandler : ICommandHandler<UpdateCourse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;
    private readonly IModuleRepository _moduleRepository;

    public UpdateCourseHandler(
        ICourseRepository courseRepository,
        IUserContextService userContextService,
        IUserRepository userRepository,
        IModuleRepository moduleRepository)
    {
        _courseRepository = courseRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
        _moduleRepository = moduleRepository;
    }

    public async Task HandleAsync(UpdateCourse command)
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
            throw new UnauthorizedAccessException("You can only update your own courses.");
        }

        // Check if course can be modified
        if (course.IsApproved)
        {
            throw new InvalidOperationException("Cannot modify an approved course.");
        }

        // Update course properties
        // Note: The Course entity currently doesn't have all update methods
        // We'll need to add them or use a different approach
        
        if (!string.IsNullOrWhiteSpace(command.CourseNumber))
        {
            course.SetCourseNumber(command.CourseNumber);
        }

        if (command.HasCertificate.HasValue && command.HasCertificate.Value && !string.IsNullOrWhiteSpace(command.CertificateNumber))
        {
            course.SetCertificate(command.CertificateNumber);
        }
        else if (command.HasCertificate.HasValue && !command.HasCertificate.Value)
        {
            course.RemoveCertificate();
        }

        if (command.ModuleId.HasValue)
        {
            var module = await _moduleRepository.GetByIdAsync(new ModuleId(command.ModuleId.Value));
            if (module is null)
            {
                throw new NotFoundException($"Module with ID {command.ModuleId.Value} not found.");
            }
            course.AssignToModule(new ModuleId(command.ModuleId.Value));
        }

        await _courseRepository.UpdateAsync(course);
    }
}