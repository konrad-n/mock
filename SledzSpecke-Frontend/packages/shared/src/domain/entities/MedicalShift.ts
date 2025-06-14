import { Duration, SyncStatus } from '../value-objects';

export interface MedicalShift {
  id: number;
  internshipId: number;
  date: string;
  duration: Duration;
  location: string;
  year: number;
  syncStatus: SyncStatus;
  createdAt: string;
  updatedAt: string;
}

export const canEditMedicalShift = (shift: MedicalShift): boolean => {
  return shift.syncStatus !== SyncStatus.Approved;
};