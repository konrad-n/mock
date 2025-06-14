using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record UpdateCourse(
    int CourseId,
    string? CourseName = null,
    string? CourseNumber = null,
    string? InstitutionName = null,
    DateTime? CompletionDate = null,
    bool? HasCertificate = null,
    string? CertificateNumber = null,
    int? ModuleId = null
) : ICommand;