using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class CompleteCourseHandler : ICommandHandler<CompleteCourse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;

    public CompleteCourseHandler(
        ICourseRepository courseRepository,
        IUserContextService userContextService,
        IUserRepository userRepository)
    {
        _courseRepository = courseRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
    }

    public async Task HandleAsync(CompleteCourse command)
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
            throw new UnauthorizedAccessException("You can only complete your own courses.");
        }

        // Update completion details
        // Note: The Course entity doesn't have UpdateCompletionDate method
        // For now, we'll just update the certificate if provided
        
        if (command.HasCertificate && !string.IsNullOrWhiteSpace(command.CertificateNumber))
        {
            course.SetCertificate(command.CertificateNumber);
        }

        await _courseRepository.UpdateAsync(course);
    }
}