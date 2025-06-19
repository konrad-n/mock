import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { mauiTheme } from '@/theme/mauiTheme';
// import { ModuleType } from '@shared/domain/value-objects';
import { useModule } from '@/contexts/ModuleContext';
import { apiClient } from '@/services/api';
import { format } from 'date-fns';
import { pl } from 'date-fns/locale';

interface MedicalShift {
  id: number;
  internshipId: number;
  date: string;
  startTime: string;
  endTime: string;
  hours: number;
  minutes: number;
  department: string;
  year: number;
}

// Mock data
const mockShifts: MedicalShift[] = [
  {
    id: 1,
    internshipId: 1,
    date: '2025-06-14',
    startTime: '2025-06-14T08:00:00',
    endTime: '2025-06-14T20:00:00',
    hours: 24,
    minutes: 0,
    department: 'Oddział Kardiologii',
    year: 2,
  },
];

export const MedicalShiftsPageMAUI = () => {
  const { currentModule } = useModule();
  const [selectedYear, setSelectedYear] = useState<number>(1);
  // const [showAddForm, setShowAddForm] = useState(false);
  const [expandedCard, setExpandedCard] = useState<number | null>(null);

  const { data: shifts = mockShifts, isLoading } = useQuery({
    queryKey: ['medical-shifts', currentModule?.id, selectedYear],
    queryFn: async () => {
      if (!currentModule) return mockShifts;
      try {
        const response = await apiClient.get<MedicalShift[]>(
          `/api/medical-shifts?moduleId=${currentModule.id}&year=${selectedYear}`
        );
        return response;
      } catch (error) {
        console.error('Failed to fetch medical shifts:', error);
        return mockShifts;
      }
    },
    enabled: !!currentModule,
  });

  const totalHours = shifts.reduce((sum, shift) => sum + shift.hours, 0);
  const totalMinutes = shifts.reduce((sum, shift) => sum + shift.minutes, 0);
  const totalTime = `${totalHours} godz. ${totalMinutes} min`;

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
    yearSelector: {
      backgroundColor: mauiTheme.colors.cardBackground,
      borderRadius: mauiTheme.borderRadius.md,
      padding: '16px',
      boxShadow: mauiTheme.shadows.card,
      marginBottom: '16px',
    },
    yearTitle: {
      fontSize: '16px',
      color: mauiTheme.colors.primary,
      marginBottom: '12px',
    },
    yearButtons: {
      display: 'flex',
      flexWrap: 'wrap' as const,
      gap: '8px',
      justifyContent: 'center',
    },
    yearButton: {
      padding: '12px 20px',
      borderRadius: mauiTheme.borderRadius.md,
      border: 'none',
      fontSize: '16px',
      fontWeight: '500',
      cursor: 'pointer',
      transition: 'all 0.3s ease',
      backgroundColor: mauiTheme.colors.cardBackground,
      color: mauiTheme.colors.textSecondary,
      boxShadow: '0 1px 3px rgba(0,0,0,0.1)',
    },
    yearButtonActive: {
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
    },
    summaryCard: {
      backgroundColor: mauiTheme.colors.cardBackground,
      borderRadius: mauiTheme.borderRadius.md,
      padding: '16px',
      boxShadow: mauiTheme.shadows.card,
      marginBottom: '16px',
    },
    summaryTitle: {
      fontSize: '18px',
      color: mauiTheme.colors.primary,
      marginBottom: '12px',
    },
    summaryRow: {
      display: 'flex',
      justifyContent: 'space-between',
      marginBottom: '8px',
      fontSize: '16px',
    },
    summaryLabel: {
      color: mauiTheme.colors.textSecondary,
    },
    summaryValue: {
      color: mauiTheme.colors.textPrimary,
      fontWeight: '500',
    },
    shiftsList: {
      marginBottom: '16px',
    },
    sectionTitle: {
      fontSize: '18px',
      color: mauiTheme.colors.primary,
      marginBottom: '12px',
    },
    hint: {
      fontSize: '14px',
      color: mauiTheme.colors.textSecondary,
      marginBottom: '16px',
      textAlign: 'center' as const,
    },
    shiftCard: {
      backgroundColor: mauiTheme.colors.cardBackground,
      borderRadius: mauiTheme.borderRadius.md,
      padding: '16px',
      boxShadow: mauiTheme.shadows.card,
      marginBottom: '12px',
      cursor: 'pointer',
      transition: 'all 0.3s ease',
    },
    shiftCardExpanded: {
      boxShadow: mauiTheme.shadows.elevated,
    },
    shiftHeader: {
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
    },
    shiftInfo: {
      flex: 1,
    },
    shiftYear: {
      fontSize: '14px',
      color: mauiTheme.colors.textSecondary,
      marginBottom: '4px',
    },
    shiftDuration: {
      fontSize: '18px',
      fontWeight: '500',
      color: mauiTheme.colors.textPrimary,
      marginBottom: '4px',
    },
    shiftDate: {
      fontSize: '16px',
      color: mauiTheme.colors.textPrimary,
      marginBottom: '4px',
    },
    shiftDepartment: {
      fontSize: '14px',
      color: mauiTheme.colors.textSecondary,
    },
    editButton: {
      padding: '8px 16px',
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
      border: 'none',
      borderRadius: mauiTheme.borderRadius.md,
      fontSize: '14px',
      fontWeight: '500',
      cursor: 'pointer',
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
    emptyState: {
      textAlign: 'center' as const,
      padding: '40px 20px',
      color: mauiTheme.colors.textSecondary,
    },
  };

  const formatShiftDate = (dateString: string) => {
    return format(new Date(dateString), 'dd.MM.yyyy', { locale: pl });
  };

  // const formatShiftTime = (startTime: string, endTime: string) => {
  //   const start = format(new Date(startTime), 'HH:mm');
  //   const end = format(new Date(endTime), 'HH:mm');
  //   return `${start} - ${end}`;
  // };

  if (isLoading) {
    return <div style={{ padding: '20px' }}>Ładowanie...</div>;
  }

  return (
    <div style={styles.container}>
      <div style={styles.content}>
        {/* Header */}
        <div style={styles.header}>
          <h1 style={styles.pageTitle}>Dyżury medyczne (Stary SMK)</h1>
          <p style={styles.subtitle}>Moduł podstawowy w zakresie chorób wewnętrznych</p>
        </div>

        {/* Year Selector */}
        <div style={styles.yearSelector}>
          <h3 style={styles.yearTitle}>
            Wybierz rok specjalizacji, dla którego chcesz zobaczyć dyżury:
          </h3>
          <div style={{ textAlign: 'center', marginBottom: '12px' }}>
            <p style={{ fontSize: '14px', color: mauiTheme.colors.textSecondary }}>
              Wybierz rok:
            </p>
          </div>
          <div style={styles.yearButtons}>
            {[1, 2, 3, 4, 5].map((year) => (
              <button
                key={year}
                style={{
                  ...styles.yearButton,
                  ...(selectedYear === year ? styles.yearButtonActive : {}),
                }}
                onClick={() => setSelectedYear(year)}
              >
                Rok {year}
              </button>
            ))}
          </div>
        </div>

        {/* Summary Card */}
        <div style={styles.summaryCard}>
          <h3 style={styles.summaryTitle}>Dyżury medyczne - podsumowanie</h3>
          <div style={styles.summaryRow}>
            <span style={styles.summaryLabel}>Liczba godzin</span>
            <span style={styles.summaryLabel}>Liczba minut</span>
          </div>
          <div style={styles.summaryRow}>
            <span style={styles.summaryValue}>Dyżury zrealizowane</span>
            <span style={styles.summaryValue}>{totalTime}</span>
          </div>
        </div>

        {/* Shifts List */}
        <div style={styles.shiftsList}>
          <h3 style={styles.sectionTitle}>Lista dyżurów medycznych</h3>
          <p style={styles.hint}>Swipe w lewo aby usunąć dyżur</p>

          {shifts.length === 0 ? (
            <div style={styles.emptyState}>
              <p>Brak dyżurów dla wybranego roku</p>
            </div>
          ) : (
            shifts.map((shift) => (
              <div
                key={shift.id}
                style={{
                  ...styles.shiftCard,
                  ...(expandedCard === shift.id ? styles.shiftCardExpanded : {}),
                }}
                onClick={() => setExpandedCard(expandedCard === shift.id ? null : shift.id)}
              >
                <div style={styles.shiftHeader}>
                  <div style={styles.shiftInfo}>
                    <div style={styles.shiftYear}>Rok szkolenia: Rok {shift.year}</div>
                    <div style={styles.shiftDuration}>
                      Liczba godzin: {shift.hours}
                      {shift.minutes > 0 && ` godz. ${shift.minutes} min`}
                    </div>
                    <div style={styles.shiftDate}>
                      Data rozpoczęcia: {formatShiftDate(shift.date)}
                    </div>
                    <div style={styles.shiftDepartment}>
                      Nazwa komórki organizacyjnej: {shift.department}
                    </div>
                  </div>
                  {expandedCard === shift.id && (
                    <button style={styles.editButton}>EDYTUJ</button>
                  )}
                </div>
              </div>
            ))
          )}
        </div>

        {/* Add Button */}
        <button
          style={styles.addButton}
          onClick={() => console.log('Add shift')}
          onMouseEnter={(e) => {
            e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark;
          }}
          onMouseLeave={(e) => {
            e.currentTarget.style.backgroundColor = mauiTheme.colors.primary;
          }}
        >
          +
        </button>
      </div>
    </div>
  );
};