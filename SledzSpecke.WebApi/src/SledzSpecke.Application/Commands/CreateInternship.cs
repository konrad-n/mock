using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record CreateInternship(
    int SpecializationId,
    string Name,
    string InstitutionName,
    string DepartmentName,
    DateTime StartDate,
    DateTime EndDate,
    int PlannedWeeks,
    int PlannedDays,
    string? SupervisorName = null,
    string? SupervisorPwz = null,
    int? ModuleId = null
) : ICommand<int>;