using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record UpdateInternship(
    int InternshipId,
    string? InstitutionName = null,
    string? DepartmentName = null,
    string? SupervisorName = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int? ModuleId = null
) : ICommand;