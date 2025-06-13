using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record CreateCourse(
    int SpecializationId,
    string CourseType,
    string CourseName,
    string InstitutionName,
    DateTime CompletionDate,
    string? CourseNumber = null,
    string? CertificateNumber = null,
    int? ModuleId = null
) : ICommand<int>;