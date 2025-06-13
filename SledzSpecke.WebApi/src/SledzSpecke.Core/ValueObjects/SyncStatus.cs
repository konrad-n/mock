namespace SledzSpecke.Core.ValueObjects;

/// <summary>
/// Represents the synchronization status of an entity with the SMK (System Monitorowania Kształcenia) system.
/// This enum tracks the lifecycle of data synchronization and local modifications.
/// </summary>
public enum SyncStatus
{
    /// <summary>
    /// Entity has never been synchronized with SMK.
    /// This is the default state for newly created entities.
    /// </summary>
    NotSynced = 0,
    
    /// <summary>
    /// Entity has been successfully synchronized with SMK and has no local changes.
    /// In the current implementation, synced entities CAN be modified (will transition to Modified).
    /// </summary>
    Synced = 1,
    
    /// <summary>
    /// An error occurred during the last synchronization attempt.
    /// Entity may have partial or inconsistent data.
    /// </summary>
    SyncError = 2,
    
    /// <summary>
    /// Synchronization attempt failed completely.
    /// Entity remains in its previous state.
    /// </summary>
    SyncFailed = 3,
    
    /// <summary>
    /// Entity was previously synced but has been modified locally.
    /// This status is automatically set when a synced entity is updated.
    /// Indicates that the entity needs to be re-synchronized to push changes to SMK.
    /// </summary>
    Modified = 4
}