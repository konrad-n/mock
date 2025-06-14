export enum SyncStatus {
  NotSynced = 'NotSynced',
  Synced = 'Synced',
  Modified = 'Modified',
  Approved = 'Approved'
}

export const isSyncStatusEditable = (status: SyncStatus): boolean => {
  return status !== SyncStatus.Approved;
};