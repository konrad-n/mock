# Sync Status Management Documentation

## Overview

The Sync Status Management system tracks the synchronization state of entities between the local application and the SMK (System Monitorowania Kształcenia) system. This document explains the implementation details and design decisions.

## Key Design Decision

**Important Change**: Unlike the original MAUI implementation where synced items were read-only, the Web API implementation allows synced items to be modified. When a synced item is updated, it automatically transitions to `Modified` status, maintaining an audit trail while allowing data corrections.

## Sync Status States

The `SyncStatus` enum has five possible values:

1. **NotSynced (0)**: Entity has never been synchronized with SMK. Default state for new entities.
2. **Synced (1)**: Entity is synchronized with SMK with no local changes. Can be modified (will auto-transition to Modified).
3. **SyncError (2)**: Error occurred during last sync attempt. Entity may have partial data.
4. **SyncFailed (3)**: Sync attempt failed completely. Entity remains in previous state.
5. **Modified (4)**: Previously synced entity has been modified locally. Needs re-synchronization.

## Implementation Details

### Affected Entities

The following entities implement sync status management:
- `MedicalShift`
- `ProcedureBase` (and derived classes)
- `Internship`
- `Course`

### Automatic Status Transitions

When any update method is called on a synced entity, it automatically transitions to `Modified` status:

```csharp
// Example from MedicalShift.UpdateShiftDetails
if (SyncStatus == SyncStatus.Synced)
{
    SyncStatus = SyncStatus.Modified;
}
```

### Modification Rules

- **Synced items**: CAN be modified (auto-transition to Modified)
- **Approved items**: CANNOT be modified (throw exception)
- **Other statuses**: Can be modified normally

### Code Example

```csharp
// Before update
var shift = await _repository.GetByIdAsync(shiftId);
// shift.SyncStatus == SyncStatus.Synced

// Update the shift
shift.UpdateShiftDetails(8, 30, "New Hospital");
// shift.SyncStatus == SyncStatus.Modified (automatic transition)

await _repository.UpdateAsync(shift);
```

## Migration from MAUI

### Key Differences

1. **MAUI**: Synced items were completely read-only
2. **Web API**: Synced items can be modified (with automatic status transition)

### Rationale

This change was made to:
- Allow users to correct data errors in synced items
- Maintain audit trail of changes
- Support more flexible workflows
- Reduce user frustration with locked data

## Testing

The test script (`test_api.py`) includes comprehensive testing for sync status transitions. Key test scenarios:

1. Creating new entities (NotSynced status)
2. Updating synced entities (transition to Modified)
3. Attempting to update approved entities (should fail)

## Troubleshooting

### Common Issues

1. **"Cannot modify synced item" errors**
   - Check if you're using old code that prevents synced modifications
   - Ensure entity's `EnsureCanModify()` method is updated

2. **Status not transitioning to Modified**
   - Verify update methods include the transition logic
   - Check that the entity was actually synced before update

3. **Database sync status mismatch**
   - Run migrations to ensure database schema is current
   - Check Entity Framework configurations

### Database Queries

Check entity sync status:
```sql
-- PostgreSQL
SELECT "Id", "SyncStatus", "IsApproved" FROM "MedicalShifts" WHERE "Id" = 1;
SELECT "Id", "SyncStatus", "Status" FROM "Procedures" WHERE "Id" = 1;
```

## Future Considerations

1. **Sync Queue**: Implement a queue for Modified items awaiting sync
2. **Conflict Resolution**: Handle conflicts when SMK data changes during local modifications
3. **Batch Sync**: Support bulk synchronization of Modified items
4. **Sync History**: Track sync attempts and results for auditing

## Related Files

- `/src/SledzSpecke.Core/ValueObjects/SyncStatus.cs` - Enum definition
- `/src/SledzSpecke.Core/Entities/*.cs` - Entity implementations
- `/src/SledzSpecke.Application/*/Handlers/Update*.cs` - Update handlers
- `/test_api.py` - Integration tests