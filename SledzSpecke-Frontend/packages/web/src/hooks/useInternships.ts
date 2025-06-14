import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { internshipsService } from '@/services/internships';
import { CreateInternshipRequest, UpdateInternshipRequest } from '@shared/types';

export const useInternships = (specializationId?: number) => {
  return useQuery({
    queryKey: ['internships', specializationId],
    queryFn: () => internshipsService.getInternships(specializationId),
  });
};

export const useInternshipModules = (specializationId: number) => {
  return useQuery({
    queryKey: ['internship-modules', specializationId],
    queryFn: () => internshipsService.getInternshipModules(specializationId),
    enabled: !!specializationId,
  });
};

export const useCreateInternship = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateInternshipRequest) => 
      internshipsService.createInternship(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['internships'] });
    },
  });
};

export const useUpdateInternship = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateInternshipRequest }) =>
      internshipsService.updateInternship(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['internships'] });
    },
  });
};

export const useDeleteInternship = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => internshipsService.deleteInternship(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['internships'] });
    },
  });
};