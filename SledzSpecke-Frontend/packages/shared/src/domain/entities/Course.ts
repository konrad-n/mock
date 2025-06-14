import { SyncStatus } from '../value-objects';

export interface Course {
  id: number;
  userId: number;
  specializationId: number;
  courseName: string;
  courseType: CourseType;
  startDate: string;
  endDate: string;
  organizer: string;
  location: string;
  certificateNumber?: string;
  creditHours?: number;
  notes?: string;
  syncStatus: SyncStatus;
  createdAt: string;
  updatedAt: string;
}

export enum CourseType {
  Conference = 'Conference',
  Workshop = 'Workshop',
  Seminar = 'Seminar',
  OnlineCourse = 'OnlineCourse',
  Other = 'Other'
}

export interface SelfEducation {
  id: number;
  userId: number;
  specializationId: number;
  activityType: SelfEducationType;
  title: string;
  description: string;
  date: string;
  creditHours: number;
  notes?: string;
  syncStatus: SyncStatus;
  createdAt: string;
  updatedAt: string;
}

export enum SelfEducationType {
  BookReading = 'BookReading',
  ArticleReview = 'ArticleReview',
  OnlineLecture = 'OnlineLecture',
  CaseStudy = 'CaseStudy',
  Research = 'Research',
  Other = 'Other'
}