import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { mauiTheme } from '@/theme/mauiTheme';
import { ModuleType } from '@shared/domain/value-objects';
import { useModule } from '@/contexts/ModuleContext';
import { apiClient } from '@/services/api';
import { format } from 'date-fns';
import { pl } from 'date-fns/locale';
// import { useNavigate } from 'react-router-dom';

interface Internship {
  id: number;
  moduleId: number;
  name: string;
  requiredDays: number;
  completedDays: number;
  recognizedDays: number;
  selfEducationDays: number;
  status: 'Pending' | 'InProgress' | 'Completed';
  startDate?: string;
  endDate?: string;
  location?: string;
}

// Mock data
const mockInternships: Record<ModuleType, Internship[]> = {
  [ModuleType.Basic]: [
    {
      id: 1,
      moduleId: 1,
      name: 'Staż podstawowy w zakresie chorób wewnętrznych',
      requiredDays: 350,
      completedDays: 350,
      recognizedDays: 0,
      selfEducationDays: 0,
      status: 'Completed',
      startDate: '2025-05-11',
      endDate: '2026-04-25',
      location: 'Katedra',
    },
    {
      id: 2,
      moduleId: 1,
      name: 'Staż kierunkowy w zakresie intensywnej opieki medycznej',
      requiredDays: 20,
      completedDays: 0,
      recognizedDays: 0,
      selfEducationDays: 0,
      status: 'Pending',
    },
    {
      id: 3,
      moduleId: 1,
      name: 'Staż kierunkowy w zakresie kardiologii',
      requiredDays: 80,
      completedDays: 0,
      recognizedDays: 0,
      selfEducationDays: 0,
      status: 'Pending',
    },
    {
      id: 4,
      moduleId: 1,
      name: 'Staż kierunkowy w zakresie chorób płuc',
      requiredDays: 30,
      completedDays: 0,
      recognizedDays: 0,
      selfEducationDays: 0,
      status: 'Pending',
    },
  ],
  [ModuleType.Specialist]: [],
};

export const InternshipsPageMAUI = () => {
  // const navigate = useNavigate();
  const { currentModule } = useModule();
  const [selectedModuleType, setSelectedModuleType] = useState<ModuleType>(ModuleType.Basic);
  const [expandedInternship, setExpandedInternship] = useState<number | null>(null);

  const { data: internships = [], isLoading } = useQuery({
    queryKey: ['internships', currentModule?.id, selectedModuleType],
    queryFn: async () => {
      if (!currentModule) return mockInternships[selectedModuleType];
      try {
        const response = await apiClient.get<Internship[]>(
          `/api/internships?moduleId=${currentModule.id}`
        );
        return response;
      } catch (error) {
        console.error('Failed to fetch internships:', error);
        return mockInternships[selectedModuleType];
      }
    },
    enabled: !!currentModule,
  });

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
    sectionTitle: {
      fontSize: '18px',
      color: mauiTheme.colors.primary,
      marginBottom: '16px',
    },
    internshipCard: {
      backgroundColor: mauiTheme.colors.cardBackground,
      borderRadius: mauiTheme.borderRadius.md,
      padding: '16px',
      boxShadow: mauiTheme.shadows.card,
      marginBottom: '12px',
      transition: 'all 0.3s ease',
    },
    internshipCardExpanded: {
      boxShadow: mauiTheme.shadows.elevated,
    },
    internshipTitle: {
      fontSize: '16px',
      fontWeight: '500',
      color: mauiTheme.colors.textPrimary,
      marginBottom: '8px',
    },
    internshipStatus: {
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
      marginBottom: '8px',
    },
    daysInfo: {
      fontSize: '14px',
      color: mauiTheme.colors.textSecondary,
    },
    checkmark: {
      width: '24px',
      height: '24px',
      borderRadius: '50%',
      backgroundColor: mauiTheme.colors.success,
      color: mauiTheme.colors.textOnPrimary,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      fontSize: '16px',
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
    realizationCard: {
      backgroundColor: '#F5F5F5',
      borderRadius: mauiTheme.borderRadius.sm,
      padding: '12px',
      marginTop: '12px',
    },
    realizationInfo: {
      fontSize: '14px',
      color: mauiTheme.colors.textPrimary,
      marginBottom: '4px',
    },
    actionButtons: {
      display: 'flex',
      gap: '8px',
      marginTop: '12px',
    },
    editButton: {
      padding: '8px 16px',
      backgroundColor: '#FFC107',
      color: mauiTheme.colors.textPrimary,
      border: 'none',
      borderRadius: mauiTheme.borderRadius.md,
      fontSize: '14px',
      cursor: 'pointer',
    },
    deleteButton: {
      padding: '8px 16px',
      backgroundColor: '#FF6B6B',
      color: mauiTheme.colors.textOnPrimary,
      border: 'none',
      borderRadius: mauiTheme.borderRadius.md,
      fontSize: '14px',
      cursor: 'pointer',
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
  };

  if (isLoading) {
    return <div style={{ padding: '20px' }}>Ładowanie...</div>;
  }

  const handleToggleDetails = (internshipId: number) => {
    setExpandedInternship(expandedInternship === internshipId ? null : internshipId);
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return '-';
    return format(new Date(dateString), 'dd.MM.yyyy', { locale: pl });
  };

  return (
    <div style={styles.container}>
      <div style={styles.content}>
        {/* Header */}
        <div style={styles.header}>
          <h1 style={styles.pageTitle}>Staże (Stary SMK)</h1>
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

        {/* Internships List */}
        <div>
          <h2 style={styles.sectionTitle}>Staże kierunkowe</h2>
          
          {internships.map((internship) => (
            <div
              key={internship.id}
              style={{
                ...styles.internshipCard,
                ...(expandedInternship === internship.id ? styles.internshipCardExpanded : {}),
              }}
            >
              <h3 style={styles.internshipTitle}>{internship.name}</h3>
              
              <div style={styles.internshipStatus}>
                <span style={styles.daysInfo}>
                  Zrealizowano {internship.completedDays} z {internship.requiredDays} dni
                </span>
                {internship.status === 'Completed' && (
                  <div style={styles.checkmark}>✓</div>
                )}
              </div>

              <button
                style={styles.detailsButton}
                onClick={() => handleToggleDetails(internship.id)}
              >
                SZCZEGÓŁY
              </button>

              {expandedInternship === internship.id && (
                <div style={styles.expandedContent}>
                  <div style={styles.detailRow}>
                    <span style={styles.detailLabel}>Dni wymagane</span>
                    <span style={styles.detailValue}>{internship.requiredDays}</span>
                  </div>
                  <div style={styles.detailRow}>
                    <span style={styles.detailLabel}>Dni wprowadzone</span>
                    <span style={styles.detailValue}>{internship.completedDays}</span>
                  </div>
                  <div style={styles.detailRow}>
                    <span style={styles.detailLabel}>Dni uznane</span>
                    <span style={styles.detailValue}>{internship.recognizedDays}</span>
                  </div>
                  <div style={styles.detailRow}>
                    <span style={styles.detailLabel}>Dni samokształcenia</span>
                    <span style={styles.detailValue}>{internship.selfEducationDays}</span>
                  </div>
                  <div style={styles.detailRow}>
                    <span style={styles.detailLabel}>Pozostało do zrealizowania</span>
                    <span style={styles.detailValue}>
                      {Math.max(0, internship.requiredDays - internship.completedDays - internship.recognizedDays - internship.selfEducationDays)}
                    </span>
                  </div>

                  {internship.status === 'Completed' && internship.location && (
                    <>
                      <h4 style={{ ...styles.sectionTitle, fontSize: '16px', marginTop: '16px' }}>
                        Realizacje stażu
                      </h4>
                      <div style={styles.realizationCard}>
                        <div style={styles.realizationInfo}>
                          {internship.location}
                        </div>
                        <div style={styles.realizationInfo}>
                          {formatDate(internship.startDate)} - {formatDate(internship.endDate)}
                        </div>
                        <div style={styles.realizationInfo}>
                          {internship.completedDays} dni
                        </div>
                        <div style={styles.actionButtons}>
                          <button style={styles.editButton}>✏️</button>
                          <button style={styles.deleteButton}>✕</button>
                        </div>
                      </div>
                    </>
                  )}

                  <button style={styles.addRealizationButton}>
                    Dodaj realizację
                  </button>
                </div>
              )}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};