import { CreateMedicalShiftRequest, UpdateMedicalShiftRequest } from '@shared/types';
import { MedicalShift } from '@shared/domain/entities';
import { Duration, SyncStatus } from '@shared/domain/value-objects';
import { apiClient } from './api';

// Mock data for development
const mockShifts: MedicalShift[] = [
  {
    id: 1,
    internshipId: 1,
    date: '2024-06-01',
    duration: Duration.fromHoursAndMinutes(12, 30),
    location: 'Szpital Uniwersytecki, Oddział Intensywnej Terapii',
    year: 1,
    syncStatus: SyncStatus.Synced,
    createdAt: '2024-06-01T08:00:00Z',
    updatedAt: '2024-06-01T08:00:00Z',
  },
  {
    id: 2,
    internshipId: 1,
    date: '2024-06-05',
    duration: Duration.fromHoursAndMinutes(8, 0),
    location: 'Szpital Miejski, Oddział Kardiologii',
    year: 1,
    syncStatus: SyncStatus.Modified,
    createdAt: '2024-06-05T08:00:00Z',
    updatedAt: '2024-06-06T10:00:00Z',
  },
  {
    id: 3,
    internshipId: 1,
    date: '2024-06-10',
    duration: Duration.fromHoursAndMinutes(24, 0),
    location: 'Szpital Wojewódzki, Oddział Chirurgii',
    year: 1,
    syncStatus: SyncStatus.NotSynced,
    createdAt: '2024-06-10T08:00:00Z',
    updatedAt: '2024-06-10T08:00:00Z',
  },
];

let nextId = 4;

export const medicalShiftsApi = {
  // Get all medical shifts for the current user
  async getMedicalShifts(internshipId?: number) {
    try {
      const params = internshipId ? { internshipId } : {};
      return await apiClient.get<MedicalShift[]>('/MedicalShifts', { params });
    } catch (error) {
      console.warn('Using mock data for medical shifts:', error);
      // Return mock data in development
      return internshipId 
        ? mockShifts.filter(shift => shift.internshipId === internshipId)
        : mockShifts;
    }
  },

  // Get a single medical shift by ID
  async getMedicalShift(id: number) {
    try {
      return await apiClient.get<MedicalShift>(`/MedicalShifts/${id}`);
    } catch (error) {
      console.warn('Using mock data for medical shift:', error);
      const shift = mockShifts.find(s => s.id === id);
      if (!shift) throw new Error('Medical shift not found');
      return shift;
    }
  },

  // Create a new medical shift
  async createMedicalShift(data: CreateMedicalShiftRequest) {
    try {
      return await apiClient.post<MedicalShift>('/MedicalShifts', data);
    } catch (error) {
      console.warn('Using mock data for create medical shift:', error);
      // Simulate creation in mock data
      const newShift: MedicalShift = {
        id: nextId++,
        internshipId: data.internshipId,
        date: data.date,
        duration: Duration.fromHoursAndMinutes(data.hours, data.minutes),
        location: data.location,
        year: data.year,
        syncStatus: SyncStatus.NotSynced,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };
      mockShifts.push(newShift);
      return newShift;
    }
  },

  // Update an existing medical shift
  async updateMedicalShift(id: number, data: UpdateMedicalShiftRequest) {
    try {
      return await apiClient.put<MedicalShift>(`/MedicalShifts/${id}`, data);
    } catch (error) {
      console.warn('Using mock data for update medical shift:', error);
      // Simulate update in mock data
      const index = mockShifts.findIndex(s => s.id === id);
      if (index === -1) throw new Error('Medical shift not found');
      
      const updated: MedicalShift = {
        ...mockShifts[index],
        ...(data.date && { date: data.date }),
        ...(data.hours !== undefined && data.minutes !== undefined && {
          duration: Duration.fromHoursAndMinutes(data.hours, data.minutes)
        }),
        ...(data.location && { location: data.location }),
        ...(data.year !== undefined && { year: data.year }),
        syncStatus: mockShifts[index].syncStatus === SyncStatus.Synced 
          ? SyncStatus.Modified 
          : mockShifts[index].syncStatus,
        updatedAt: new Date().toISOString(),
      };
      
      mockShifts[index] = updated;
      return updated;
    }
  },

  // Delete a medical shift
  async deleteMedicalShift(id: number) {
    try {
      await apiClient.delete(`/MedicalShifts/${id}`);
    } catch (error) {
      console.warn('Using mock data for delete medical shift:', error);
      // Simulate deletion in mock data
      const index = mockShifts.findIndex(s => s.id === id);
      if (index === -1) throw new Error('Medical shift not found');
      mockShifts.splice(index, 1);
    }
  },

  // Get medical shift statistics
  async getStatistics() {
    try {
      return await apiClient.get<any>('/MedicalShifts/statistics');
    } catch (error) {
      console.warn('Using mock data for statistics:', error);
      // Return mock statistics
      const totalHours = mockShifts.reduce((sum, shift) => 
        sum + shift.duration.toTotalMinutes() / 60, 0
      );
      return {
        totalShifts: mockShifts.length,
        totalHours: totalHours,
        averageHours: mockShifts.length > 0 ? totalHours / mockShifts.length : 0,
        byYear: {
          1: mockShifts.filter(s => s.year === 1).length,
          2: mockShifts.filter(s => s.year === 2).length,
          3: mockShifts.filter(s => s.year === 3).length,
          4: mockShifts.filter(s => s.year === 4).length,
          5: mockShifts.filter(s => s.year === 5).length,
        }
      };
    }
  }
};