import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { mauiTheme } from '@/theme/mauiTheme';
import { ModuleType } from '@shared/domain/value-objects';
import { useModule } from '@/contexts/ModuleContext';
import { apiClient } from '@/services/api';
// import { format } from 'date-fns';
// import { pl } from 'date-fns/locale';

interface Course {
  id: number;
  moduleId: number;
  name: string;
  type: 'Required' | 'Optional';
  requiredHours: number;
  completedHours: number;
  startDate?: string;
  endDate?: string;
  location?: string;
  status: 'NotStarted' | 'InProgress' | 'Completed';
}

// Mock data
const mockCourses: Course[] = [
  {
    id: 1,
    moduleId: 1,
    name: 'Kurs specjalizacyjny z zakresu ratownictwa medycznego',
    type: 'Required',
    requiredHours: 80,
    completedHours: 0,
    status: 'NotStarted',
  },
  {
    id: 2,
    moduleId: 1,
    name: 'Kurs doskonalący z zakresu intensywnej terapii',
    type: 'Required',
    requiredHours: 40,
    completedHours: 0,
    status: 'NotStarted',
  },
  {
    id: 3,
    moduleId: 1,
    name: 'Kurs z zakresu badań obrazowych w kardiologii',
    type: 'Optional',
    requiredHours: 24,
    completedHours: 0,
    status: 'NotStarted',
  },
];

export const CoursesPageMAUI = () => {
  const { currentModule } = useModule();
  const [selectedModuleType, setSelectedModuleType] = useState<ModuleType>(ModuleType.Basic);
  const [expandedCourse, setExpandedCourse] = useState<number | null>(null);

  const { data: courses = mockCourses, isLoading } = useQuery({
    queryKey: ['courses', currentModule?.id, selectedModuleType],
    queryFn: async () => {
      if (!currentModule) return mockCourses;
      try {
        const response = await apiClient.get<Course[]>(
          `/api/courses?moduleId=${currentModule.id}`
        );
        return response;
      } catch (error) {
        console.error('Failed to fetch courses:', error);
        return mockCourses;
      }
    },
    enabled: !!currentModule,
  });

  const completedCourses = courses.filter(c => c.status === 'Completed').length;
  const totalRequiredCourses = courses.filter(c => c.type === 'Required').length;

  const styles = {
    container: {
      backgroundColor: mauiTheme.colors.background,
      minHeight: '100vh',
      paddingBottom: '80px',
    },
    content: {
      padding: '16px',
    },
    header: {
      marginBottom: '24px',
    },
    pageTitle: {
      fontSize: '24px',
      fontWeight: '500',
      color: mauiTheme.colors.textPrimary,
      marginBottom: '8px',
    },
    subtitle: {
      fontSize: '16px',
      color: mauiTheme.colors.textSecondary,
    },
    moduleSwitcher: {
      display: 'flex',
      justifyContent: 'center',
      marginBottom: '24px',
    },
    summaryCard: {
      backgroundColor: mauiTheme.colors.cardBackground,
      borderRadius: mauiTheme.borderRadius.md,
      padding: '20px',
      boxShadow: mauiTheme.shadows.card,
      marginBottom: '24px',
      textAlign: 'center' as const,
    },
    summaryTitle: {
      fontSize: '18px',
      color: mauiTheme.colors.primary,
      marginBottom: '12px',
    },
    summaryStats: {
      fontSize: '24px',
      fontWeight: '500',
      color: mauiTheme.colors.textPrimary,
    },
    sectionTitle: {
      fontSize: '18px',
      color: mauiTheme.colors.primary,
      marginBottom: '16px',
      marginTop: '24px',
    },
    courseCard: {
      backgroundColor: mauiTheme.colors.cardBackground,
      borderRadius: mauiTheme.borderRadius.md,
      padding: '16px',
      boxShadow: mauiTheme.shadows.card,
      marginBottom: '12px',
      cursor: 'pointer',
      transition: 'all 0.3s ease',
    },
    courseCardCompleted: {
      opacity: 0.8,
    },
    courseHeader: {
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'flex-start',
    },
    courseInfo: {
      flex: 1,
      marginRight: '16px',
    },
    courseName: {
      fontSize: '16px',
      fontWeight: '500',
      color: mauiTheme.colors.textPrimary,
      marginBottom: '8px',
    },
    courseType: {
      display: 'inline-block',
      padding: '4px 12px',
      borderRadius: '12px',
      fontSize: '12px',
      fontWeight: '500',
      marginBottom: '8px',
    },
    courseTypeRequired: {
      backgroundColor: '#FFE0E0',
      color: '#D32F2F',
    },
    courseTypeOptional: {
      backgroundColor: '#E3F2FD',
      color: '#1976D2',
    },
    courseHours: {
      fontSize: '14px',
      color: mauiTheme.colors.textSecondary,
    },
    checkmark: {
      width: '32px',
      height: '32px',
      borderRadius: '50%',
      backgroundColor: mauiTheme.colors.success,
      color: mauiTheme.colors.textOnPrimary,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      fontSize: '18px',
      flexShrink: 0,
    },
    detailsButton: {
      width: '100%',
      padding: '12px',
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
      border: 'none',
      borderRadius: mauiTheme.borderRadius.xl,
      fontSize: '16px',
      fontWeight: '500',
      cursor: 'pointer',
      marginTop: '12px',
    },
    expandedContent: {
      marginTop: '16px',
      paddingTop: '16px',
      borderTop: `1px solid ${mauiTheme.colors.divider}`,
    },
    detailRow: {
      display: 'flex',
      justifyContent: 'space-between',
      marginBottom: '8px',
      fontSize: '14px',
    },
    detailLabel: {
      color: mauiTheme.colors.textSecondary,
    },
    detailValue: {
      color: mauiTheme.colors.textPrimary,
    },
    addRealizationButton: {
      width: '100%',
      padding: '12px',
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
      border: 'none',
      borderRadius: mauiTheme.borderRadius.xl,
      fontSize: '16px',
      fontWeight: '500',
      cursor: 'pointer',
      marginTop: '16px',
    },
    addButton: {
      position: 'fixed' as const,
      bottom: '20px',
      right: '20px',
      width: '56px',
      height: '56px',
      borderRadius: '50%',
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
      border: 'none',
      fontSize: '24px',
      cursor: 'pointer',
      boxShadow: mauiTheme.shadows.elevated,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
    },
  };

  if (isLoading) {
    return <div style={{ padding: '20px' }}>Ładowanie...</div>;
  }

  const handleToggleCourse = (courseId: number) => {
    setExpandedCourse(expandedCourse === courseId ? null : courseId);
  };

  return (
    <div style={styles.container}>
      <div style={styles.content}>
        {/* Header */}
        <div style={styles.header}>
          <h1 style={styles.pageTitle}>Kursy (Stary SMK)</h1>
          <p style={styles.subtitle}>Moduł podstawowy w zakresie chorób wewnętrznych</p>
        </div>

        {/* Module Switcher */}
        <div style={styles.moduleSwitcher}>
          <div className="maui-module-switcher">
            <button
              className={`maui-module-tab ${selectedModuleType === ModuleType.Basic ? 'active' : ''}`}
              onClick={() => setSelectedModuleType(ModuleType.Basic)}
            >
              Podstawowy
            </button>
            <button
              className={`maui-module-tab ${selectedModuleType === ModuleType.Specialist ? 'active' : ''}`}
              onClick={() => setSelectedModuleType(ModuleType.Specialist)}
            >
              Specjalistyczny
            </button>
          </div>
        </div>

        {/* Summary Card */}
        <div style={styles.summaryCard}>
          <h2 style={styles.summaryTitle}>Kursy zrealizowane</h2>
          <div style={styles.summaryStats}>
            {completedCourses} / {totalRequiredCourses}
          </div>
        </div>

        {/* Courses List */}
        <h2 style={styles.sectionTitle}>Lista kursów</h2>
        
        {courses.map((course) => (
          <div
            key={course.id}
            style={{
              ...styles.courseCard,
              ...(course.status === 'Completed' ? styles.courseCardCompleted : {}),
            }}
            onClick={() => handleToggleCourse(course.id)}
          >
            <div style={styles.courseHeader}>
              <div style={styles.courseInfo}>
                <div style={styles.courseName}>{course.name}</div>
                <span
                  style={{
                    ...styles.courseType,
                    ...(course.type === 'Required' ? styles.courseTypeRequired : styles.courseTypeOptional),
                  }}
                >
                  {course.type === 'Required' ? 'Obowiązkowy' : 'Fakultatywny'}
                </span>
                <div style={styles.courseHours}>
                  Wymiar godzinowy: {course.requiredHours} godz.
                  {course.completedHours > 0 && ` (ukończono: ${course.completedHours} godz.)`}
                </div>
              </div>
              {course.status === 'Completed' && (
                <div style={styles.checkmark}>✓</div>
              )}
            </div>

            <button style={styles.detailsButton}>
              SZCZEGÓŁY
            </button>

            {expandedCourse === course.id && (
              <div style={styles.expandedContent}>
                <div style={styles.detailRow}>
                  <span style={styles.detailLabel}>Typ kursu</span>
                  <span style={styles.detailValue}>
                    {course.type === 'Required' ? 'Obowiązkowy' : 'Fakultatywny'}
                  </span>
                </div>
                <div style={styles.detailRow}>
                  <span style={styles.detailLabel}>Wymiar godzinowy</span>
                  <span style={styles.detailValue}>{course.requiredHours} godz.</span>
                </div>
                <div style={styles.detailRow}>
                  <span style={styles.detailLabel}>Godziny zrealizowane</span>
                  <span style={styles.detailValue}>{course.completedHours} godz.</span>
                </div>
                <div style={styles.detailRow}>
                  <span style={styles.detailLabel}>Status</span>
                  <span style={styles.detailValue}>
                    {course.status === 'Completed' ? 'Ukończony' : 
                     course.status === 'InProgress' ? 'W trakcie' : 'Nierozpoczęty'}
                  </span>
                </div>

                {course.status !== 'Completed' && (
                  <button
                    style={styles.addRealizationButton}
                    onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
                    onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
                  >
                    Dodaj realizację kursu
                  </button>
                )}
              </div>
            )}
          </div>
        ))}

        {/* Add Button */}
        <button
          style={styles.addButton}
          onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
          onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
        >
          +
        </button>
      </div>
    </div>
  );
};