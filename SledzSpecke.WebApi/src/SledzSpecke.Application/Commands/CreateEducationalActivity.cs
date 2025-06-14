using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public sealed record CreateEducationalActivity(
    int SpecializationId,
    int? ModuleId,
    string Type,
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime EndDate) : ICommand<int>;