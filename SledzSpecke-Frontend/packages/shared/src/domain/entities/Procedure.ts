import { SyncStatus, OperatorLevel } from '../value-objects';

export interface Procedure {
  id: number;
  internshipId: number;
  procedureCode: string;
  procedureName: string;
  performanceDate: string;
  operatorLevel: OperatorLevel;
  location: string;
  patientAge: number;
  patientSex: 'M' | 'K';
  additionalInfo?: string;
  icdCode?: string;
  syncStatus: SyncStatus;
  createdAt: string;
  updatedAt: string;
}