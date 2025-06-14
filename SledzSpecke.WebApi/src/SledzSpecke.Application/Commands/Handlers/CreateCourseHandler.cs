using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class CreateCourseHandler : IResultCommandHandler<CreateCourse, int>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IModuleRepository _moduleRepository;

    public CreateCourseHandler(
        ICourseRepository courseRepository,
        ISpecializationRepository specializationRepository,
        IModuleRepository moduleRepository)
    {
        _courseRepository = courseRepository;
        _specializationRepository = specializationRepository;
        _moduleRepository = moduleRepository;
    }

    public async Task<Result<int>> HandleAsync(CreateCourse command)
    {
        var specialization = await _specializationRepository.GetByIdAsync(command.SpecializationId);
        if (specialization is null)
        {
            return Result.Failure<int>($"Specialization with ID {command.SpecializationId} not found.");
        }

        if (command.ModuleId.HasValue)
        {
            var module = await _moduleRepository.GetByIdAsync(command.ModuleId.Value);
            if (module is null)
            {
                return Result.Failure<int>($"Module with ID {command.ModuleId.Value} not found.");
            }
        }

        if (!Enum.TryParse<CourseType>(command.CourseType, out var courseType))
        {
            return Result.Failure<int>($"Invalid course type: {command.CourseType}");
        }

        try
        {
            var courseId = CourseId.New();
            var course = Course.Create(
                courseId,
                command.SpecializationId,
                courseType,
                command.CourseName,
                command.InstitutionName,
                command.CompletionDate);

            if (!string.IsNullOrWhiteSpace(command.CourseNumber))
            {
                course.SetCourseNumber(command.CourseNumber);
            }

            if (!string.IsNullOrWhiteSpace(command.CertificateNumber))
            {
                course.SetCertificate(command.CertificateNumber);
            }

            if (command.ModuleId.HasValue)
            {
                course.AssignToModule(command.ModuleId.Value);
            }

            await _courseRepository.AddAsync(course);
            return Result.Success((int)course.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<int>($"Failed to create course: {ex.Message}");
        }
    }
}