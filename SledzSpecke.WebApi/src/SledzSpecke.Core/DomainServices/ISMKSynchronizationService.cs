using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

/// <summary>
/// Domain service for managing complex synchronization logic with the SMK system
/// </summary>
public interface ISMKSynchronizationService
{
    /// <summary>
    /// Validates if an internship and all its related data is ready for synchronization
    /// </summary>
    Task<Result<SynchronizationReadiness>> ValidateReadinessForSyncAsync(
        InternshipId internshipId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Prepares an internship and its related entities for synchronization
    /// </summary>
    Task<Result> PrepareForSynchronizationAsync(
        InternshipId internshipId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles the approval workflow which affects multiple related entities
    /// </summary>
    Task<Result> ProcessApprovalWorkflowAsync(
        InternshipId internshipId,
        string approverName,
        DateTime approvalDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Manages batch synchronization of multiple entities
    /// </summary>
    Task<Result<BatchSyncResult>> SynchronizeBatchAsync(
        IEnumerable<InternshipId> internshipIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves conflicts when local changes exist on synced data
    /// </summary>
    Task<Result<ConflictResolution>> ResolveConflictsAsync(
        InternshipId internshipId,
        ConflictResolutionStrategy strategy,
        CancellationToken cancellationToken = default);
}

public record SynchronizationReadiness
{
    public bool IsReady { get; init; }
    public List<string> MissingRequirements { get; init; } = new();
    public int ProcedureCount { get; init; }
    public int MedicalShiftCount { get; init; }
    public int CourseCount { get; init; }
}

public record BatchSyncResult
{
    public int TotalProcessed { get; init; }
    public int SuccessfulSyncs { get; init; }
    public int FailedSyncs { get; init; }
    public List<SyncError> Errors { get; init; } = new();
}

public record SyncError
{
    public InternshipId InternshipId { get; init; }
    public string ErrorMessage { get; init; }
    public string ErrorCode { get; init; }
}

public record ConflictResolution
{
    public InternshipId InternshipId { get; init; }
    public int ConflictsResolved { get; init; }
    public List<string> Actions { get; init; } = new();
}

public enum ConflictResolutionStrategy
{
    KeepLocal,
    KeepRemote,
    Merge,
    Manual
}