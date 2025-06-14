import { apiClient } from './api';
import { Internship, InternshipModule } from '@shared/domain/entities';
import { CreateInternshipRequest, UpdateInternshipRequest } from '@shared/types';
import { SyncStatus } from '@shared/domain/value-objects';

// Mock data for development
const mockModules: InternshipModule[] = [
  { id: 1, name: 'Oddział Kardiologii Inwazyjnej', requiredDays: 180 },
  { id: 2, name: 'Oddział Kardiologii Zachowawczej', requiredDays: 180 },
  { id: 3, name: 'Oddział Intensywnej Opieki Kardiologicznej', requiredDays: 90 },
  { id: 4, name: 'Poradnia Kardiologiczna', requiredDays: 30 },
  { id: 5, name: 'Pracownia Echokardiografii', requiredDays: 60 },
];

const mockInternships: Internship[] = [
  {
    id: 1,
    userId: 1,
    specializationId: 1,
    moduleId: 1,
    institutionName: 'Szpital Uniwersytecki',
    departmentName: 'Oddział Kardiologii Inwazyjnej',
    supervisorName: 'dr hab. n. med. Jan Kowalski',
    startDate: '2024-01-01',
    endDate: '2024-06-30',
    requiredDays: 180,
    completedDays: 165,
    syncStatus: SyncStatus.Synced,
    createdAt: '2024-01-01T08:00:00Z',
    updatedAt: '2024-06-15T10:30:00Z',
  },
  {
    id: 2,
    userId: 1,
    specializationId: 1,
    moduleId: 2,
    institutionName: 'Centrum Kardiologii',
    departmentName: 'Oddział Kardiologii Zachowawczej',
    supervisorName: 'prof. dr hab. Anna Nowak',
    startDate: '2024-07-01',
    endDate: '2024-12-31',
    requiredDays: 180,
    completedDays: 0,
    syncStatus: SyncStatus.NotSynced,
    createdAt: '2024-06-20T09:00:00Z',
    updatedAt: '2024-06-20T09:00:00Z',
  },
];

class InternshipsService {
  async getInternships(specializationId?: number) {
    try {
      const params = specializationId ? { specializationId } : {};
      return await apiClient.get<Internship[]>('/Internships', { params });
    } catch (error) {
      console.warn('Using mock data for internships:', error);
      return specializationId 
        ? mockInternships.filter(i => i.specializationId === specializationId)
        : mockInternships;
    }
  }

  async getInternshipModules(specializationId: number) {
    try {
      return await apiClient.get<InternshipModule[]>(`/Specializations/${specializationId}/modules`);
    } catch (error) {
      console.warn('Using mock data for modules:', error);
      return mockModules;
    }
  }

  async createInternship(data: CreateInternshipRequest) {
    try {
      return await apiClient.post<Internship>('/Internships', data);
    } catch (error) {
      // Mock create
      const newInternship: Internship = {
        id: Date.now(),
        userId: 1,
        specializationId: data.specializationId,
        moduleId: data.moduleId,
        institutionName: data.institutionName,
        departmentName: data.departmentName,
        supervisorName: data.supervisorName,
        startDate: data.startDate,
        endDate: data.endDate,
        requiredDays: mockModules.find(m => m.id === data.moduleId)?.requiredDays || 180,
        completedDays: 0,
        syncStatus: SyncStatus.NotSynced,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };
      mockInternships.push(newInternship);
      return newInternship;
    }
  }

  async updateInternship(id: number, data: UpdateInternshipRequest) {
    try {
      return await apiClient.put<Internship>(`/Internships/${id}`, data);
    } catch (error) {
      // Mock update
      const index = mockInternships.findIndex(i => i.id === id);
      if (index !== -1) {
        mockInternships[index] = {
          ...mockInternships[index],
          ...data,
          syncStatus: SyncStatus.Modified,
          updatedAt: new Date().toISOString(),
        };
        return mockInternships[index];
      }
      throw new Error('Internship not found');
    }
  }

  async deleteInternship(id: number) {
    try {
      return await apiClient.delete(`/Internships/${id}`);
    } catch (error) {
      // Mock delete
      const index = mockInternships.findIndex(i => i.id === id);
      if (index !== -1) {
        mockInternships.splice(index, 1);
        return;
      }
      throw new Error('Internship not found');
    }
  }
}

export const internshipsService = new InternshipsService();