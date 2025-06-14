import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { coursesService } from '@/services/courses';
import { 
  CreateCourseRequest, 
  UpdateCourseRequest,
  CreateSelfEducationRequest,
  UpdateSelfEducationRequest 
} from '@shared/types';

// Courses hooks
export const useCourses = (specializationId?: number) => {
  return useQuery({
    queryKey: ['courses', specializationId],
    queryFn: () => coursesService.getCourses(specializationId),
  });
};

export const useCreateCourse = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateCourseRequest) => 
      coursesService.createCourse(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['courses'] });
    },
  });
};

export const useUpdateCourse = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateCourseRequest }) =>
      coursesService.updateCourse(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['courses'] });
    },
  });
};

export const useDeleteCourse = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => coursesService.deleteCourse(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['courses'] });
    },
  });
};

// Self-Education hooks
export const useSelfEducation = (specializationId?: number) => {
  return useQuery({
    queryKey: ['self-education', specializationId],
    queryFn: () => coursesService.getSelfEducation(specializationId),
  });
};

export const useCreateSelfEducation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateSelfEducationRequest) => 
      coursesService.createSelfEducation(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['self-education'] });
    },
  });
};

export const useUpdateSelfEducation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateSelfEducationRequest }) =>
      coursesService.updateSelfEducation(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['self-education'] });
    },
  });
};

export const useDeleteSelfEducation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => coursesService.deleteSelfEducation(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['self-education'] });
    },
  });
};