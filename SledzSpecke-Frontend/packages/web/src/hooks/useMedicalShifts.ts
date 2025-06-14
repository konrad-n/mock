import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { medicalShiftsApi } from '@/services/medicalShifts';
import { CreateMedicalShiftRequest, UpdateMedicalShiftRequest } from '@shared/types';

// Query key factory
const medicalShiftKeys = {
  all: ['medicalShifts'] as const,
  lists: () => [...medicalShiftKeys.all, 'list'] as const,
  list: (internshipId?: number) => 
    internshipId 
      ? [...medicalShiftKeys.lists(), { internshipId }] as const
      : medicalShiftKeys.lists(),
  details: () => [...medicalShiftKeys.all, 'detail'] as const,
  detail: (id: number) => [...medicalShiftKeys.details(), id] as const,
  statistics: () => [...medicalShiftKeys.all, 'statistics'] as const,
};

// Get medical shifts
export const useMedicalShifts = (internshipId?: number) => {
  return useQuery({
    queryKey: medicalShiftKeys.list(internshipId),
    queryFn: () => medicalShiftsApi.getMedicalShifts(internshipId),
  });
};

// Get single medical shift
export const useMedicalShift = (id: number) => {
  return useQuery({
    queryKey: medicalShiftKeys.detail(id),
    queryFn: () => medicalShiftsApi.getMedicalShift(id),
    enabled: !!id,
  });
};

// Create medical shift
export const useCreateMedicalShift = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: CreateMedicalShiftRequest) => 
      medicalShiftsApi.createMedicalShift(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: medicalShiftKeys.all });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
};

// Update medical shift
export const useUpdateMedicalShift = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateMedicalShiftRequest }) => 
      medicalShiftsApi.updateMedicalShift(id, data),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: medicalShiftKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: medicalShiftKeys.lists() });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
};

// Delete medical shift
export const useDeleteMedicalShift = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => medicalShiftsApi.deleteMedicalShift(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: medicalShiftKeys.all });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
};

// Get statistics
export const useMedicalShiftStatistics = () => {
  return useQuery({
    queryKey: medicalShiftKeys.statistics(),
    queryFn: () => medicalShiftsApi.getStatistics(),
  });
};