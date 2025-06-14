import { SyncStatus } from '../value-objects';

export interface Internship {
  id: number;
  userId: number;
  specializationId: number;
  moduleId: number;
  institutionName: string;
  departmentName: string;
  supervisorName: string;
  startDate: string;
  endDate: string;
  requiredDays: number;
  completedDays: number;
  syncStatus: SyncStatus;
  createdAt: string;
  updatedAt: string;
}

export interface InternshipModule {
  id: number;
  name: string;
  requiredDays: number;
  description?: string;
}