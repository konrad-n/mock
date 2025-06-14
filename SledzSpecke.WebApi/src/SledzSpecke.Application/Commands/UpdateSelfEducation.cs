using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record UpdateSelfEducation(
    SelfEducationId SelfEducationId,
    string Title,
    string? Description,
    string? Provider,
    int CreditHours,
    DateTime? StartDate,
    DateTime? EndDate,
    int? DurationHours) : ICommand;