import { useState, useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { DashboardOverview } from '@shared/types';
import { ModuleType } from '@shared/domain/value-objects';
import { useModule } from '@/contexts/ModuleContext';
import { apiClient } from '@/services/api';
import { mauiTheme } from '@/theme/mauiTheme';
import { useNavigate } from 'react-router-dom';

// Mock data for now
const mockDashboardData: DashboardOverview = {
  currentModule: 'Moduł podstawowy w zakresie chorób wewnętrznych',
  moduleType: 'Basic',
  overallProgress: 2,
  internships: {
    completed: 1,
    total: 12,
    percentage: 8
  },
  courses: {
    completed: 0,
    total: 10,
    percentage: 0
  },
  procedures: {
    completed: 0,
    total: 273,
    percentage: 0
  },
  medicalShifts: {
    completed: 0,
    total: 1048,
    hoursCompleted: 0,
    hoursRequired: 1048,
    percentage: 0
  },
  selfEducation: {
    completed: 0,
    creditHours: 0
  },
  publications: {
    total: 0,
    firstAuthor: 0,
    peerReviewed: 0
  }
};

export const DashboardPageMAUI = () => {
  const navigate = useNavigate();
  const { currentModule, modules = [], setCurrentModule, isLoading: modulesLoading } = useModule();
  const [selectedModuleType, setSelectedModuleType] = useState<ModuleType>(ModuleType.Basic);
  
  useEffect(() => {
    if (currentModule) {
      setSelectedModuleType(currentModule.type === 'Basic' ? ModuleType.Basic : ModuleType.Specialist);
    }
  }, [currentModule]);

  const { data: dashboardData, isLoading } = useQuery({
    queryKey: ['dashboard', currentModule?.id],
    queryFn: async () => {
      if (!currentModule) return mockDashboardData;
      
      try {
        const response = await apiClient.get<DashboardOverview>(
          `/api/dashboard/progress/${currentModule.specializationId}?moduleId=${currentModule.id}`
        );
        return response;
      } catch (error) {
        console.error('Failed to fetch dashboard data:', error);
        return mockDashboardData;
      }
    },
    enabled: !!currentModule
  });

  const handleModuleChange = (moduleType: ModuleType) => {
    setSelectedModuleType(moduleType);
    const module = modules.find(m => 
      (m.type === 'Basic' && moduleType === ModuleType.Basic) ||
      (m.type === 'Specialist' && moduleType === ModuleType.Specialist)
    );
    if (module) {
      setCurrentModule(module);
    }
  };

  if (isLoading || modulesLoading || !dashboardData) {
    return <div style={{ padding: '20px' }}>Ładowanie...</div>;
  }

  const styles = {
    container: {
      backgroundColor: mauiTheme.colors.background,
      minHeight: '100vh',
      paddingBottom: '80px',
    },
    content: {
      padding: '16px',
    },
    moduleSwitcher: {
      display: 'flex',
      justifyContent: 'center',
      marginBottom: '16px',
    },
    moduleCard: {
      backgroundColor: mauiTheme.colors.cardBackground,
      borderRadius: mauiTheme.borderRadius.md,
      padding: mauiTheme.spacing.lg,
      boxShadow: mauiTheme.shadows.card,
      marginBottom: '16px',
    },
    moduleTitle: {
      fontSize: '18px',
      fontWeight: '500',
      marginBottom: '8px',
      color: mauiTheme.colors.textPrimary,
    },
    moduleSubtitle: {
      fontSize: '14px',
      color: mauiTheme.colors.textSecondary,
      marginBottom: '16px',
    },
    progressBar: {
      width: '100%',
      height: '8px',
      backgroundColor: mauiTheme.colors.progressBackground,
      borderRadius: '4px',
      overflow: 'hidden',
      marginTop: '8px',
    },
    progressFill: {
      height: '100%',
      backgroundColor: mauiTheme.colors.progressFill,
      transition: 'width 0.3s ease',
    },
    progressText: {
      fontSize: '14px',
      color: mauiTheme.colors.textSecondary,
      marginTop: '8px',
      textAlign: 'center' as const,
    },
    statsGrid: {
      display: 'grid',
      gridTemplateColumns: 'repeat(2, 1fr)',
      gap: '16px',
      marginBottom: '16px',
    },
    statsCard: {
      backgroundColor: mauiTheme.colors.cardBackground,
      borderRadius: mauiTheme.borderRadius.md,
      padding: mauiTheme.spacing.lg,
      boxShadow: mauiTheme.shadows.card,
      textAlign: 'center' as const,
      minHeight: '140px',
      display: 'flex',
      flexDirection: 'column' as const,
      justifyContent: 'space-between',
    },
    statsTitle: {
      fontSize: '16px',
      color: mauiTheme.colors.textSecondary,
      marginBottom: '12px',
    },
    statsValue: {
      fontSize: '28px',
      fontWeight: '500',
      color: mauiTheme.colors.textPrimary,
      marginBottom: '16px',
    },
    statsButton: {
      width: '100%',
      height: '40px',
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
      border: 'none',
      borderRadius: mauiTheme.borderRadius.xl,
      fontSize: '16px',
      fontWeight: '500',
      cursor: 'pointer',
      transition: 'background-color 0.3s ease',
    },
    bottomActions: {
      position: 'fixed' as const,
      bottom: 0,
      left: 0,
      right: 0,
      backgroundColor: mauiTheme.colors.cardBackground,
      borderTop: `1px solid ${mauiTheme.colors.divider}`,
      padding: '12px 16px',
      display: 'flex',
      gap: '12px',
    },
    actionButton: {
      flex: 1,
      height: '48px',
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
      border: 'none',
      borderRadius: mauiTheme.borderRadius.xl,
      fontSize: '16px',
      fontWeight: '500',
      cursor: 'pointer',
      transition: 'background-color 0.3s ease',
    },
  };

  return (
    <div style={styles.container}>
      <div style={styles.content}>
        {/* Module Switcher */}
        <div style={styles.moduleSwitcher}>
          <div className="maui-module-switcher">
            <button
              className={`maui-module-tab ${selectedModuleType === ModuleType.Basic ? 'active' : ''}`}
              onClick={() => handleModuleChange(ModuleType.Basic)}
            >
              Podstawowy
            </button>
            <button
              className={`maui-module-tab ${selectedModuleType === ModuleType.Specialist ? 'active' : ''}`}
              onClick={() => handleModuleChange(ModuleType.Specialist)}
            >
              Specjalistyczny
            </button>
          </div>
        </div>

        {/* Module Card */}
        <div style={styles.moduleCard}>
          <h2 style={styles.moduleTitle}>{dashboardData.currentModule}</h2>
          <p style={styles.moduleSubtitle}>
            Kardiologia<br />
            11-05-2025 - 11-05-2030
          </p>
          <div style={styles.progressBar}>
            <div style={{ ...styles.progressFill, width: `${dashboardData.overallProgress}%` }} />
          </div>
          <p style={styles.progressText}>Ukończono {dashboardData.overallProgress}%</p>
        </div>

        {/* Stats Grid */}
        <div style={styles.statsGrid}>
          {/* Staże */}
          <div style={styles.statsCard}>
            <div>
              <div style={styles.statsTitle}>Staże</div>
              <div style={styles.statsValue}>
                {dashboardData.internships.completed}/{dashboardData.internships.total}
              </div>
            </div>
            <div className="maui-progress" style={{ marginBottom: '12px' }}>
              <div className="maui-progress-fill" style={{ width: `${dashboardData.internships.percentage}%` }} />
            </div>
            <button
              style={styles.statsButton}
              onClick={() => navigate('/internships')}
              onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
              onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
            >
              Przejdź
            </button>
          </div>

          {/* Dyżury */}
          <div style={styles.statsCard}>
            <div>
              <div style={styles.statsTitle}>Dyżury</div>
              <div style={styles.statsValue}>
                {dashboardData.medicalShifts.hoursCompleted}/{dashboardData.medicalShifts.hoursRequired}h
              </div>
            </div>
            <div className="maui-progress" style={{ marginBottom: '12px' }}>
              <div className="maui-progress-fill" style={{ width: `${dashboardData.medicalShifts.percentage}%` }} />
            </div>
            <button
              style={styles.statsButton}
              onClick={() => navigate('/medical-shifts')}
              onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
              onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
            >
              Przejdź
            </button>
          </div>

          {/* Procedury */}
          <div style={styles.statsCard}>
            <div>
              <div style={styles.statsTitle}>Procedury</div>
              <div style={styles.statsValue}>
                {dashboardData.procedures.completed}/{dashboardData.procedures.total}
              </div>
            </div>
            <div className="maui-progress" style={{ marginBottom: '12px' }}>
              <div className="maui-progress-fill" style={{ width: `${dashboardData.procedures.percentage}%` }} />
            </div>
            <button
              style={styles.statsButton}
              onClick={() => navigate('/procedures')}
              onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
              onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
            >
              Przejdź
            </button>
          </div>

          {/* Kursy */}
          <div style={styles.statsCard}>
            <div>
              <div style={styles.statsTitle}>Kursy</div>
              <div style={styles.statsValue}>
                {dashboardData.courses.completed}/{dashboardData.courses.total}
              </div>
            </div>
            <div className="maui-progress" style={{ marginBottom: '12px' }}>
              <div className="maui-progress-fill" style={{ width: `${dashboardData.courses.percentage}%` }} />
            </div>
            <button
              style={styles.statsButton}
              onClick={() => navigate('/courses')}
              onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
              onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
            >
              Przejdź
            </button>
          </div>

          {/* Samokształcenie */}
          <div style={styles.statsCard}>
            <div>
              <div style={styles.statsTitle}>Samokształcenie</div>
              <div style={styles.statsValue}>{dashboardData.selfEducation.completed}</div>
            </div>
            <button
              style={styles.statsButton}
              onClick={() => navigate('/self-education')}
              onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
              onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
            >
              Przejdź
            </button>
          </div>

          {/* Publikacje */}
          <div style={styles.statsCard}>
            <div>
              <div style={styles.statsTitle}>Publikacje</div>
              <div style={styles.statsValue}>{dashboardData.publications.total}</div>
            </div>
            <button
              style={styles.statsButton}
              onClick={() => navigate('/publications')}
              onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
              onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
            >
              Przejdź
            </button>
          </div>
        </div>

        {/* Bottom Actions */}
        <div style={styles.bottomActions}>
          <button
            style={styles.actionButton}
            onClick={() => navigate('/absences')}
            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
          >
            Nieobecności
          </button>
          <button
            style={styles.actionButton}
            onClick={() => navigate('/export')}
            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
          >
            Eksport
          </button>
        </div>
      </div>
    </div>
  );
};