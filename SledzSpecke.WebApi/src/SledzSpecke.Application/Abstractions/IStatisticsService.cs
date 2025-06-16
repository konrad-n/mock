using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Abstractions;

public interface IStatisticsService
{
    // Medical Shift Statistics
    Task UpdateMonthlyShiftStatisticsAsync(
        InternshipId internshipId,
        DateTime shiftDate,
        CancellationToken cancellationToken = default);

    Task UpdateApprovedHoursStatisticsAsync(
        InternshipId internshipId,
        DateTime shiftDate,
        CancellationToken cancellationToken = default);

    // Procedure Statistics
    Task TrackDuplicateProcedureAsync(
        InternshipId internshipId,
        string procedureCode,
        DateTime date,
        CancellationToken cancellationToken = default);

    Task UpdateDailyProcedureCountAsync(
        InternshipId internshipId,
        DateTime date,
        CancellationToken cancellationToken = default);

    Task TrackNewProcedureTypeAsync(
        InternshipId internshipId,
        string procedureCode,
        CancellationToken cancellationToken = default);

    Task UpdateLocationStatisticsAsync(
        string location,
        string procedureCode,
        CancellationToken cancellationToken = default);

    Task UpdateProcedurePatternsAsync(
        InternshipId internshipId,
        object patterns,
        CancellationToken cancellationToken = default);

    Task UpdateProcedureStatisticsAsync(
        string department,
        string procedureCode,
        int count,
        CancellationToken cancellationToken = default);

    // Module and Progress Statistics
    Task UpdateModuleProgressAsync(
        InternshipId internshipId,
        object moduleProgress,
        CancellationToken cancellationToken = default);

    Task UpdateYearProgressAsync(
        InternshipId internshipId,
        object yearProgress,
        CancellationToken cancellationToken = default);
}