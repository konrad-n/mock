using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record CreateInternship(
    int SpecializationId,
    string InstitutionName,
    string DepartmentName,
    DateTime StartDate,
    DateTime EndDate,
    string? SupervisorName = null,
    int? ModuleId = null
) : ICommand<int>;