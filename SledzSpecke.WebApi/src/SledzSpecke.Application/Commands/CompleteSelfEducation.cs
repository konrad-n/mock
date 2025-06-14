using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record CompleteSelfEducation(
    SelfEducationId SelfEducationId,
    DateTime? CompletedAt,
    string? CertificatePath) : ICommand;