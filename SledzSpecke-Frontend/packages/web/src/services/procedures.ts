import { CreateProcedureRequest, UpdateProcedureRequest } from '@shared/types';
import { Procedure } from '@shared/domain/entities';
import { SyncStatus, OperatorLevel } from '@shared/domain/value-objects';
import { apiClient } from './api';

// Mock data for development
const mockProcedures: Procedure[] = [
  {
    id: 1,
    internshipId: 1,
    procedureCode: '89.52',
    procedureName: 'Koronarografia',
    performanceDate: '2024-06-01',
    operatorLevel: OperatorLevel.Operator,
    location: 'Pracownia Hemodynamiki',
    patientAge: 65,
    patientSex: 'M',
    additionalInfo: 'Pacjent z podejrzeniem choroby wieńcowej',
    icdCode: 'I25.1',
    syncStatus: SyncStatus.Synced,
    createdAt: '2024-06-01T10:00:00Z',
    updatedAt: '2024-06-01T10:00:00Z',
  },
  {
    id: 2,
    internshipId: 1,
    procedureCode: '36.01',
    procedureName: 'PTCA (angioplastyka wieńcowa)',
    performanceDate: '2024-06-05',
    operatorLevel: OperatorLevel.FirstAssistant,
    location: 'Pracownia Hemodynamiki',
    patientAge: 58,
    patientSex: 'K',
    additionalInfo: 'Zwężenie LAD 90%, implantacja stentu DES',
    icdCode: 'I25.1',
    syncStatus: SyncStatus.Modified,
    createdAt: '2024-06-05T14:00:00Z',
    updatedAt: '2024-06-06T09:00:00Z',
  },
  {
    id: 3,
    internshipId: 1,
    procedureCode: '37.22',
    procedureName: 'Cewnikowanie serca',
    performanceDate: '2024-06-10',
    operatorLevel: OperatorLevel.SecondAssistant,
    location: 'Oddział Kardiologii',
    patientAge: 72,
    patientSex: 'M',
    additionalInfo: 'Diagnostyka niewydolności serca',
    icdCode: 'I50.0',
    syncStatus: SyncStatus.NotSynced,
    createdAt: '2024-06-10T11:00:00Z',
    updatedAt: '2024-06-10T11:00:00Z',
  },
];

let nextId = 4;

export const proceduresApi = {
  // Get all procedures for the current user
  async getProcedures(internshipId?: number) {
    try {
      const params = internshipId ? { internshipId } : {};
      return await apiClient.get<Procedure[]>('/Procedures', { params });
    } catch (error) {
      console.warn('Using mock data for procedures:', error);
      // Return mock data in development
      return internshipId 
        ? mockProcedures.filter(proc => proc.internshipId === internshipId)
        : mockProcedures;
    }
  },

  // Get a single procedure by ID
  async getProcedure(id: number) {
    try {
      return await apiClient.get<Procedure>(`/Procedures/${id}`);
    } catch (error) {
      console.warn('Using mock data for procedure:', error);
      const procedure = mockProcedures.find(p => p.id === id);
      if (!procedure) throw new Error('Procedure not found');
      return procedure;
    }
  },

  // Create a new procedure
  async createProcedure(data: CreateProcedureRequest) {
    try {
      return await apiClient.post<Procedure>('/Procedures', data);
    } catch (error) {
      console.warn('Using mock data for create procedure:', error);
      // Simulate creation in mock data
      const newProcedure: Procedure = {
        id: nextId++,
        internshipId: data.internshipId,
        procedureCode: data.procedureCode,
        procedureName: data.procedureName,
        performanceDate: data.performanceDate,
        operatorLevel: data.operatorLevel as OperatorLevel,
        location: data.location,
        patientAge: data.patientAge,
        patientSex: data.patientSex,
        additionalInfo: data.additionalInfo || '',
        icdCode: data.icdCode || '',
        syncStatus: SyncStatus.NotSynced,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };
      mockProcedures.push(newProcedure);
      return newProcedure;
    }
  },

  // Update an existing procedure
  async updateProcedure(id: number, data: UpdateProcedureRequest) {
    try {
      return await apiClient.put<Procedure>(`/Procedures/${id}`, data);
    } catch (error) {
      console.warn('Using mock data for update procedure:', error);
      // Simulate update in mock data
      const index = mockProcedures.findIndex(p => p.id === id);
      if (index === -1) throw new Error('Procedure not found');
      
      const updated: Procedure = {
        ...mockProcedures[index],
        ...(data.procedureCode && { procedureCode: data.procedureCode }),
        ...(data.procedureName && { procedureName: data.procedureName }),
        ...(data.performanceDate && { performanceDate: data.performanceDate }),
        ...(data.operatorLevel && { operatorLevel: data.operatorLevel as OperatorLevel }),
        ...(data.location && { location: data.location }),
        ...(data.patientAge !== undefined && { patientAge: data.patientAge }),
        ...(data.patientSex && { patientSex: data.patientSex }),
        ...(data.additionalInfo !== undefined && { additionalInfo: data.additionalInfo }),
        ...(data.icdCode !== undefined && { icdCode: data.icdCode }),
        syncStatus: mockProcedures[index].syncStatus === SyncStatus.Synced 
          ? SyncStatus.Modified 
          : mockProcedures[index].syncStatus,
        updatedAt: new Date().toISOString(),
      };
      
      mockProcedures[index] = updated;
      return updated;
    }
  },

  // Delete a procedure
  async deleteProcedure(id: number) {
    try {
      await apiClient.delete(`/Procedures/${id}`);
    } catch (error) {
      console.warn('Using mock data for delete procedure:', error);
      // Simulate deletion in mock data
      const index = mockProcedures.findIndex(p => p.id === id);
      if (index === -1) throw new Error('Procedure not found');
      mockProcedures.splice(index, 1);
    }
  },

  // Get procedure statistics
  async getStatistics() {
    try {
      return await apiClient.get<any>('/Procedures/statistics');
    } catch (error) {
      console.warn('Using mock data for statistics:', error);
      // Return mock statistics
      const operatorCount = mockProcedures.filter(p => p.operatorLevel === OperatorLevel.Operator).length;
      const assistantCount = mockProcedures.filter(p => p.operatorLevel !== OperatorLevel.Operator).length;
      
      return {
        totalProcedures: mockProcedures.length,
        asOperator: operatorCount,
        asAssistant: assistantCount,
        byMonth: {
          '2024-06': mockProcedures.filter(p => p.performanceDate.startsWith('2024-06')).length,
        },
        commonProcedures: [
          { code: '89.52', name: 'Koronarografia', count: 1 },
          { code: '36.01', name: 'PTCA', count: 1 },
          { code: '37.22', name: 'Cewnikowanie serca', count: 1 },
        ]
      };
    }
  }
};