using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetCourseByIdHandler : IQueryHandler<GetCourseById, CourseDto>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;
    private readonly ISpecializationRepository _specializationRepository;

    public GetCourseByIdHandler(
        ICourseRepository courseRepository,
        IUserContextService userContextService,
        IUserRepository userRepository,
        ISpecializationRepository specializationRepository)
    {
        _courseRepository = courseRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
        _specializationRepository = specializationRepository;
    }

    public async Task<CourseDto> HandleAsync(GetCourseById query)
    {
        var course = await _courseRepository.GetByIdAsync(new CourseId(query.CourseId));
        if (course is null)
        {
            throw new NotFoundException($"Course with ID {query.CourseId} not found.");
        }

        // Check if the course belongs to the current user
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user is null || user.SpecializationId != course.SpecializationId)
        {
            throw new UnauthorizedAccessException("You can only view your own courses.");
        }

        return new CourseDto
        {
            Id = course.Id.Value,
            SpecializationId = course.SpecializationId.Value,
            ModuleId = course.ModuleId?.Value,
            CourseType = course.CourseType.ToString(),
            CourseName = course.CourseName,
            CourseNumber = course.CourseNumber,
            InstitutionName = course.InstitutionName,
            CompletionDate = course.CompletionDate,
            HasCertificate = course.HasCertificate,
            CertificateNumber = course.CertificateNumber,
            IsApproved = course.IsApproved,
            ApprovalDate = course.ApprovalDate,
            ApproverName = course.ApproverName,
            SyncStatus = course.SyncStatus.ToString(),
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }
}