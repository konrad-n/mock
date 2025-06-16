using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Abstractions;

public interface IProjectionService
{
    // Student Progress Projections
    Task UpdateStudentProgressProjectionAsync(
        InternshipId internshipId,
        CancellationToken cancellationToken = default);

    Task<double> GetModuleCompletionPercentageAsync(
        InternshipId internshipId,
        ModuleId moduleId,
        CancellationToken cancellationToken = default);

    // Supervisor Projections
    Task UpdateSupervisorWorkloadProjectionAsync(
        string supervisorId,
        CancellationToken cancellationToken = default);

    // Department Analytics Projections
    Task UpdateDepartmentAnalyticsProjectionAsync(
        string department,
        CancellationToken cancellationToken = default);
}