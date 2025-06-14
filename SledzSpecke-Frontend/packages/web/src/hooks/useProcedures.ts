import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { proceduresApi } from '@/services/procedures';
import { CreateProcedureRequest, UpdateProcedureRequest } from '@shared/types';

// Query key factory
const procedureKeys = {
  all: ['procedures'] as const,
  lists: () => [...procedureKeys.all, 'list'] as const,
  list: (internshipId?: number) => 
    internshipId 
      ? [...procedureKeys.lists(), { internshipId }] as const
      : procedureKeys.lists(),
  details: () => [...procedureKeys.all, 'detail'] as const,
  detail: (id: number) => [...procedureKeys.details(), id] as const,
  statistics: () => [...procedureKeys.all, 'statistics'] as const,
};

// Get procedures
export const useProcedures = (internshipId?: number) => {
  return useQuery({
    queryKey: procedureKeys.list(internshipId),
    queryFn: () => proceduresApi.getProcedures(internshipId),
  });
};

// Get single procedure
export const useProcedure = (id: number) => {
  return useQuery({
    queryKey: procedureKeys.detail(id),
    queryFn: () => proceduresApi.getProcedure(id),
    enabled: !!id,
  });
};

// Create procedure
export const useCreateProcedure = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (data: CreateProcedureRequest) => 
      proceduresApi.createProcedure(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: procedureKeys.all });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
};

// Update procedure
export const useUpdateProcedure = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateProcedureRequest }) => 
      proceduresApi.updateProcedure(id, data),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: procedureKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: procedureKeys.lists() });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
};

// Delete procedure
export const useDeleteProcedure = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: number) => proceduresApi.deleteProcedure(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: procedureKeys.all });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
};

// Get statistics
export const useProcedureStatistics = () => {
  return useQuery({
    queryKey: procedureKeys.statistics(),
    queryFn: () => proceduresApi.getStatistics(),
  });
};