import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { mauiTheme } from '@/theme/mauiTheme';
import { ModuleType } from '@shared/domain/value-objects';
import { useModule } from '@/contexts/ModuleContext';
import { apiClient } from '@/services/api';

interface ProcedureSummary {
  operatorLevel: 'A' | 'B';
  totalProcedures: number;
  completedProcedures: number;
}

interface Procedure {
  id: number;
  name: string;
  operatorCount: number;
  assistantCount: number;
  operatorLevel: 'A' | 'B';
  year: number;
  completedOperator: number;
  completedAssistant: number;
}

// Mock data
const mockProcedures: Procedure[] = [
  {
    id: 1,
    name: 'prowadzenie resuscytacji krążeniowo-oddechowej BLS / ALS',
    operatorCount: 3,
    assistantCount: 3,
    operatorLevel: 'A',
    year: 1,
    completedOperator: 1,
    completedAssistant: 0,
  },
  {
    id: 2,
    name: 'nakłucie jamy opłucnej w przypadku płynu',
    operatorCount: 10,
    assistantCount: 3,
    operatorLevel: 'A',
    year: 1,
    completedOperator: 0,
    completedAssistant: 0,
  },
  {
    id: 3,
    name: 'nakłucie jamy otrzewnej w przypadku wodobrzusza',
    operatorCount: 10,
    assistantCount: 3,
    operatorLevel: 'A',
    year: 1,
    completedOperator: 0,
    completedAssistant: 0,
  },
  {
    id: 4,
    name: 'nakłucie żył obwodowych – iniekcje dożylne, pobranie krwi obwodowej',
    operatorCount: 30,
    assistantCount: 5,
    operatorLevel: 'B',
    year: 1,
    completedOperator: 0,
    completedAssistant: 0,
  },
];

export const ProceduresPageMAUI = () => {
  const { currentModule } = useModule();
  const [selectedModuleType, setSelectedModuleType] = useState<ModuleType>(ModuleType.Basic);
  const [expandedProcedure, setExpandedProcedure] = useState<number | null>(null);

  const { data: procedures = mockProcedures, isLoading } = useQuery({
    queryKey: ['procedures', currentModule?.id, selectedModuleType],
    queryFn: async () => {
      if (!currentModule) return mockProcedures;
      try {
        const response = await apiClient.get<Procedure[]>(
          `/api/procedures?moduleId=${currentModule.id}`
        );
        return response;
      } catch (error) {
        console.error('Failed to fetch procedures:', error);
        return mockProcedures;
      }
    },
    enabled: !!currentModule,
  });

  // Calculate summaries
  const summaryA: ProcedureSummary = {
    operatorLevel: 'A',
    totalProcedures: procedures.filter(p => p.operatorLevel === 'A').reduce((sum, p) => sum + p.operatorCount, 0),
    completedProcedures: procedures.filter(p => p.operatorLevel === 'A').reduce((sum, p) => sum + p.completedOperator, 0),
  };

  const summaryB: ProcedureSummary = {
    operatorLevel: 'B',
    totalProcedures: procedures.filter(p => p.operatorLevel === 'B').reduce((sum, p) => sum + p.operatorCount, 0),
    completedProcedures: procedures.filter(p => p.operatorLevel === 'B').reduce((sum, p) => sum + p.completedOperator, 0),
  };

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
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
    },
    addButton: {
      width: '40px',
      height: '40px',
      borderRadius: '8px',
      backgroundColor: mauiTheme.colors.textPrimary,
      color: mauiTheme.colors.textOnPrimary,
      border: 'none',
      fontSize: '24px',
      cursor: 'pointer',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
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
    },
    summaryTitle: {
      fontSize: '18px',
      color: mauiTheme.colors.primary,
      marginBottom: '16px',
    },
    summaryRow: {
      marginBottom: '16px',
    },
    summaryLabel: {
      fontSize: '16px',
      fontWeight: '500',
      color: mauiTheme.colors.textPrimary,
      marginBottom: '4px',
    },
    summaryStats: {
      fontSize: '14px',
      color: mauiTheme.colors.textSecondary,
    },
    sectionTitle: {
      fontSize: '18px',
      color: mauiTheme.colors.primary,
      marginBottom: '16px',
      marginTop: '24px',
    },
    procedureCard: {
      backgroundColor: mauiTheme.colors.cardBackground,
      borderRadius: mauiTheme.borderRadius.md,
      padding: '16px',
      boxShadow: mauiTheme.shadows.card,
      marginBottom: '12px',
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
      cursor: 'pointer',
      transition: 'all 0.3s ease',
    },
    procedureInfo: {
      flex: 1,
      marginRight: '16px',
    },
    procedureName: {
      fontSize: '16px',
      fontWeight: '500',
      color: mauiTheme.colors.textPrimary,
      marginBottom: '4px',
    },
    procedureStats: {
      fontSize: '14px',
      color: mauiTheme.colors.textSecondary,
    },
    expandButton: {
      width: '48px',
      height: '48px',
      borderRadius: '50%',
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
      border: 'none',
      fontSize: '24px',
      cursor: 'pointer',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      transition: 'transform 0.3s ease',
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
    actionButtons: {
      display: 'flex',
      gap: '8px',
      marginTop: '16px',
    },
    actionButton: {
      flex: 1,
      padding: '12px',
      borderRadius: mauiTheme.borderRadius.md,
      border: 'none',
      fontSize: '14px',
      fontWeight: '500',
      cursor: 'pointer',
    },
    addOperatorButton: {
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
    },
    addAssistantButton: {
      backgroundColor: '#9E9E9E',
      color: mauiTheme.colors.textOnPrimary,
    },
  };

  if (isLoading) {
    return <div style={{ padding: '20px' }}>Ładowanie...</div>;
  }

  const handleToggleProcedure = (procedureId: number) => {
    setExpandedProcedure(expandedProcedure === procedureId ? null : procedureId);
  };

  return (
    <div style={styles.container}>
      <div style={styles.content}>
        {/* Header */}
        <div style={styles.header}>
          <h1 style={styles.pageTitle}>
            Procedury (Stary SMK)
            <button style={styles.addButton}>+</button>
          </h1>
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
          <h2 style={styles.summaryTitle}>Podsumowanie procedur</h2>
          
          <div style={styles.summaryRow}>
            <div style={styles.summaryLabel}>A - operator</div>
            <div style={styles.summaryStats}>
              Liczba procedur: {summaryA.totalProcedures}<br />
              Liczba wykonanych procedur: {summaryA.completedProcedures}
            </div>
          </div>

          <div style={styles.summaryRow}>
            <div style={styles.summaryLabel}>B - asysta</div>
            <div style={styles.summaryStats}>
              Liczba procedur: {summaryB.totalProcedures}<br />
              Liczba wykonanych procedur: {summaryB.completedProcedures}
            </div>
          </div>
        </div>

        {/* Procedures List */}
        <h2 style={styles.sectionTitle}>Lista procedur</h2>
        
        {procedures.map((procedure) => (
          <div key={procedure.id}>
            <div
              style={styles.procedureCard}
              onClick={() => handleToggleProcedure(procedure.id)}
            >
              <div style={styles.procedureInfo}>
                <div style={styles.procedureName}>{procedure.name}</div>
                <div style={styles.procedureStats}>
                  {procedure.completedOperator}/{procedure.operatorCount} (A), {procedure.completedAssistant}/{procedure.assistantCount} (B)
                </div>
              </div>
              <button
                style={{
                  ...styles.expandButton,
                  transform: expandedProcedure === procedure.id ? 'rotate(180deg)' : 'rotate(0deg)',
                }}
              >
                ⌄
              </button>
            </div>

            {expandedProcedure === procedure.id && (
              <div style={styles.procedureCard}>
                <div style={{ width: '100%' }}>
                  <div style={styles.expandedContent}>
                    <div style={styles.detailRow}>
                      <span style={styles.detailLabel}>Poziom operatora</span>
                      <span style={styles.detailValue}>{procedure.operatorLevel}</span>
                    </div>
                    <div style={styles.detailRow}>
                      <span style={styles.detailLabel}>Wymagane jako operator</span>
                      <span style={styles.detailValue}>{procedure.operatorCount}</span>
                    </div>
                    <div style={styles.detailRow}>
                      <span style={styles.detailLabel}>Wykonane jako operator</span>
                      <span style={styles.detailValue}>{procedure.completedOperator}</span>
                    </div>
                    <div style={styles.detailRow}>
                      <span style={styles.detailLabel}>Wymagane jako asysta</span>
                      <span style={styles.detailValue}>{procedure.assistantCount}</span>
                    </div>
                    <div style={styles.detailRow}>
                      <span style={styles.detailLabel}>Wykonane jako asysta</span>
                      <span style={styles.detailValue}>{procedure.completedAssistant}</span>
                    </div>

                    <div style={styles.actionButtons}>
                      <button
                        style={{ ...styles.actionButton, ...styles.addOperatorButton }}
                        onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
                        onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
                      >
                        Dodaj jako operator
                      </button>
                      <button
                        style={{ ...styles.actionButton, ...styles.addAssistantButton }}
                        onMouseEnter={(e) => e.currentTarget.style.backgroundColor = '#757575'}
                        onMouseLeave={(e) => e.currentTarget.style.backgroundColor = '#9E9E9E'}
                      >
                        Dodaj jako asysta
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
};