import { apiClient } from './api';
import { Course, CourseType, SelfEducation, SelfEducationType } from '@shared/domain/entities';
import { 
  CreateCourseRequest, 
  UpdateCourseRequest,
  CreateSelfEducationRequest,
  UpdateSelfEducationRequest 
} from '@shared/types';
import { SyncStatus } from '@shared/domain/value-objects';

// Mock data for development
const mockCourses: Course[] = [
  {
    id: 1,
    userId: 1,
    specializationId: 1,
    courseName: 'Konferencja Kardiologiczna ESC 2024',
    courseType: CourseType.Conference,
    startDate: '2024-09-15',
    endDate: '2024-09-18',
    organizer: 'European Society of Cardiology',
    location: 'Warszawa, Polska',
    certificateNumber: 'ESC/2024/12345',
    creditHours: 20,
    notes: 'Główne tematy: niewydolność serca, arytmie',
    syncStatus: SyncStatus.Synced,
    createdAt: '2024-09-20T10:00:00Z',
    updatedAt: '2024-09-20T10:00:00Z',
  },
  {
    id: 2,
    userId: 1,
    specializationId: 1,
    courseName: 'Warsztaty Echokardiografii Zaawansowanej',
    courseType: CourseType.Workshop,
    startDate: '2024-06-10',
    endDate: '2024-06-12',
    organizer: 'Polskie Towarzystwo Kardiologiczne',
    location: 'Kraków, Polska',
    creditHours: 15,
    syncStatus: SyncStatus.NotSynced,
    createdAt: '2024-06-15T09:00:00Z',
    updatedAt: '2024-06-15T09:00:00Z',
  },
];

const mockSelfEducation: SelfEducation[] = [
  {
    id: 1,
    userId: 1,
    specializationId: 1,
    activityType: SelfEducationType.ArticleReview,
    title: 'Przegląd najnowszych wytycznych ESC dotyczących niewydolności serca',
    description: 'Analiza i podsumowanie kluczowych zmian w wytycznych ESC 2023',
    date: '2024-03-15',
    creditHours: 4,
    notes: 'Szczególny nacisk na nowe leki SGLT2i',
    syncStatus: SyncStatus.Synced,
    createdAt: '2024-03-15T20:00:00Z',
    updatedAt: '2024-03-15T20:00:00Z',
  },
  {
    id: 2,
    userId: 1,
    specializationId: 1,
    activityType: SelfEducationType.OnlineLecture,
    title: 'Webinarium: Postępy w leczeniu migotania przedsionków',
    description: 'Wykład online organizowany przez ACC',
    date: '2024-05-20',
    creditHours: 2,
    syncStatus: SyncStatus.Modified,
    createdAt: '2024-05-20T18:00:00Z',
    updatedAt: '2024-05-22T10:00:00Z',
  },
];

class CoursesService {
  // Courses
  async getCourses(specializationId?: number) {
    try {
      const params = specializationId ? { specializationId } : {};
      return await apiClient.get<Course[]>('/Courses', { params });
    } catch (error) {
      console.warn('Using mock data for courses:', error);
      return specializationId 
        ? mockCourses.filter(c => c.specializationId === specializationId)
        : mockCourses;
    }
  }

  async createCourse(data: CreateCourseRequest) {
    try {
      return await apiClient.post<Course>('/Courses', data);
    } catch (error) {
      // Mock create
      const newCourse: Course = {
        id: Date.now(),
        userId: 1,
        specializationId: data.specializationId,
        courseName: data.courseName,
        courseType: data.courseType as CourseType,
        startDate: data.startDate,
        endDate: data.endDate,
        organizer: data.organizer,
        location: data.location,
        certificateNumber: data.certificateNumber,
        creditHours: data.creditHours,
        notes: data.notes,
        syncStatus: SyncStatus.NotSynced,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };
      mockCourses.push(newCourse);
      return newCourse;
    }
  }

  async updateCourse(id: number, data: UpdateCourseRequest) {
    try {
      return await apiClient.put<Course>(`/Courses/${id}`, data);
    } catch (error) {
      // Mock update
      const index = mockCourses.findIndex(c => c.id === id);
      if (index !== -1) {
        mockCourses[index] = {
          ...mockCourses[index],
          ...data,
          courseType: data.courseType as CourseType || mockCourses[index].courseType,
          syncStatus: SyncStatus.Modified,
          updatedAt: new Date().toISOString(),
        };
        return mockCourses[index];
      }
      throw new Error('Course not found');
    }
  }

  async deleteCourse(id: number) {
    try {
      return await apiClient.delete(`/Courses/${id}`);
    } catch (error) {
      // Mock delete
      const index = mockCourses.findIndex(c => c.id === id);
      if (index !== -1) {
        mockCourses.splice(index, 1);
        return;
      }
      throw new Error('Course not found');
    }
  }

  // Self-Education
  async getSelfEducation(specializationId?: number) {
    try {
      const params = specializationId ? { specializationId } : {};
      return await apiClient.get<SelfEducation[]>('/SelfEducation', { params });
    } catch (error) {
      console.warn('Using mock data for self-education:', error);
      return specializationId 
        ? mockSelfEducation.filter(s => s.specializationId === specializationId)
        : mockSelfEducation;
    }
  }

  async createSelfEducation(data: CreateSelfEducationRequest) {
    try {
      return await apiClient.post<SelfEducation>('/SelfEducation', data);
    } catch (error) {
      // Mock create
      const newActivity: SelfEducation = {
        id: Date.now(),
        userId: 1,
        specializationId: data.specializationId,
        activityType: data.activityType as SelfEducationType,
        title: data.title,
        description: data.description,
        date: data.date,
        creditHours: data.creditHours,
        notes: data.notes,
        syncStatus: SyncStatus.NotSynced,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      };
      mockSelfEducation.push(newActivity);
      return newActivity;
    }
  }

  async updateSelfEducation(id: number, data: UpdateSelfEducationRequest) {
    try {
      return await apiClient.put<SelfEducation>(`/SelfEducation/${id}`, data);
    } catch (error) {
      // Mock update
      const index = mockSelfEducation.findIndex(s => s.id === id);
      if (index !== -1) {
        mockSelfEducation[index] = {
          ...mockSelfEducation[index],
          ...data,
          activityType: data.activityType as SelfEducationType || mockSelfEducation[index].activityType,
          syncStatus: SyncStatus.Modified,
          updatedAt: new Date().toISOString(),
        };
        return mockSelfEducation[index];
      }
      throw new Error('Self-education activity not found');
    }
  }

  async deleteSelfEducation(id: number) {
    try {
      return await apiClient.delete(`/SelfEducation/${id}`);
    } catch (error) {
      // Mock delete
      const index = mockSelfEducation.findIndex(s => s.id === id);
      if (index !== -1) {
        mockSelfEducation.splice(index, 1);
        return;
      }
      throw new Error('Self-education activity not found');
    }
  }
}

export const coursesService = new CoursesService();