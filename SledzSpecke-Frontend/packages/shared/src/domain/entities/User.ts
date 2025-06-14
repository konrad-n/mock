import { SmkVersion } from '../value-objects';

export interface User {
  id: number;
  email: string;
  username: string;
  fullName: string;
  smkVersion: SmkVersion;
  specializationId?: number;
  phoneNumber?: string;
  dateOfBirth?: string;
  bio?: string;
  createdAt: string;
  updatedAt: string;
}