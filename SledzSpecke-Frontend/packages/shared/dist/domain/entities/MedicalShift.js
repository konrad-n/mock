import { SyncStatus } from '../value-objects';
export const canEditMedicalShift = (shift) => {
    return shift.syncStatus !== SyncStatus.Approved;
};
//# sourceMappingURL=MedicalShift.js.map