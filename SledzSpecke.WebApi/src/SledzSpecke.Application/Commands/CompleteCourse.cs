using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record CompleteCourse(
    int CourseId,
    DateTime CompletionDate,
    bool HasCertificate,
    string? CertificateNumber = null
) : ICommand;