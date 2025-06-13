using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record CreateSelfEducation(
    SpecializationId SpecializationId,
    UserId UserId,
    SelfEducationType Type,
    int Year,
    string Title,
    int CreditHours,
    string? Description = null,
    string? Provider = null) : ICommand;