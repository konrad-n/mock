import { Grid, Box, Typography, Tabs, Tab, Button } from '@mui/material';
import { useState, useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { DashboardOverview } from '@shared/types';
import { ModuleType, getModuleLabel } from '@shared/domain/value-objects';
import { ProgressCard } from '@/components/dashboard/ProgressCard';
import { StatsCard } from '@/components/dashboard/StatsCard';
import { useModule } from '@/contexts/ModuleContext';
import { apiClient } from '@/services/api';
import {
  LocalHospital,
  Assignment,
  Description,
  School,
  Book,
  Article,
  EventBusy,
  GetApp
} from '@mui/icons-material';

// Mock data for now
const mockDashboardData: DashboardOverview = {
  currentModule: 'Moduł Podstawowy',
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

export const DashboardPage = () => {
  const { currentModule, modules, setCurrentModule, isLoading: modulesLoading } = useModule();
  const [selectedModuleType, setSelectedModuleType] = useState<ModuleType>(ModuleType.Basic);
  
  // Update selected module type when current module changes
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
        // Get module-specific progress
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

  const handleModuleChange = (_: React.SyntheticEvent, newValue: ModuleType) => {
    setSelectedModuleType(newValue);
    // Find and set the module of this type
    const module = modules.find(m => 
      (m.type === 'Basic' && newValue === ModuleType.Basic) ||
      (m.type === 'Specialist' && newValue === ModuleType.Specialist)
    );
    if (module) {
      setCurrentModule(module);
    }
  };

  if (isLoading || modulesLoading || !dashboardData) {
    return <Box>Ładowanie...</Box>;
  }

  return (
    <Box>
      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" gutterBottom>
          Dashboard
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Śledzenie postępów specjalizacji
        </Typography>
      </Box>

      {/* Module Tabs */}
      <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}>
        <Tabs value={selectedModuleType} onChange={handleModuleChange}>
          <Tab label={getModuleLabel(ModuleType.Basic)} value={ModuleType.Basic} />
          <Tab label={getModuleLabel(ModuleType.Specialist)} value={ModuleType.Specialist} />
        </Tabs>
      </Box>

      {/* Overall Progress */}
      <Box sx={{ mb: 4, p: 3, bgcolor: 'primary.main', color: 'white', borderRadius: 2 }}>
        <Typography variant="h6" gutterBottom>
          Postęp całkowity: {dashboardData.overallProgress}%
        </Typography>
        <Box sx={{ width: '100%', bgcolor: 'rgba(255,255,255,0.3)', borderRadius: 1, height: 8 }}>
          <Box
            sx={{
              width: `${dashboardData.overallProgress}%`,
              bgcolor: 'white',
              height: '100%',
              borderRadius: 1,
              transition: 'width 0.3s ease'
            }}
          />
        </Box>
      </Box>

      {/* Progress Cards */}
      <Grid container spacing={3}>
        <Grid item xs={12} md={6} lg={4}>
          <ProgressCard
            title="Staże"
            completed={dashboardData.internships.completed}
            total={dashboardData.internships.total}
            percentage={dashboardData.internships.percentage}
            icon={<LocalHospital />}
            color="primary"
            link="/internships"
          />
        </Grid>

        <Grid item xs={12} md={6} lg={4}>
          <ProgressCard
            title="Dyżury"
            completed={dashboardData.medicalShifts.hoursCompleted}
            total={dashboardData.medicalShifts.hoursRequired}
            percentage={dashboardData.medicalShifts.percentage}
            icon={<Assignment />}
            color="secondary"
            link="/medical-shifts"
            unit="h"
          />
        </Grid>

        <Grid item xs={12} md={6} lg={4}>
          <ProgressCard
            title="Procedury"
            completed={dashboardData.procedures.completed}
            total={dashboardData.procedures.total}
            percentage={dashboardData.procedures.percentage}
            icon={<Description />}
            color="success"
            link="/procedures"
          />
        </Grid>

        <Grid item xs={12} md={6} lg={4}>
          <ProgressCard
            title="Kursy"
            completed={dashboardData.courses.completed}
            total={dashboardData.courses.total}
            percentage={dashboardData.courses.percentage}
            icon={<School />}
            color="info"
            link="/courses"
          />
        </Grid>

        <Grid item xs={12} md={6} lg={4}>
          <StatsCard
            title="Samokształcenie"
            value={dashboardData.selfEducation.completed}
            subValue={`${dashboardData.selfEducation.creditHours} punktów`}
            icon={<Book />}
            color="warning"
            link="/self-education"
          />
        </Grid>

        <Grid item xs={12} md={6} lg={4}>
          <StatsCard
            title="Publikacje"
            value={dashboardData.publications.total}
            subValue={`${dashboardData.publications.firstAuthor} jako pierwszy autor`}
            icon={<Article />}
            color="error"
            link="/publications"
          />
        </Grid>
      </Grid>

      {/* Quick Actions */}
      <Grid container spacing={2} sx={{ mt: 4 }}>
        <Grid item xs={12} sm={6}>
          <Button
            fullWidth
            variant="outlined"
            startIcon={<EventBusy />}
            size="large"
            sx={{ py: 2 }}
          >
            Nieobecności
          </Button>
        </Grid>
        <Grid item xs={12} sm={6}>
          <Button
            fullWidth
            variant="outlined"
            startIcon={<GetApp />}
            size="large"
            sx={{ py: 2 }}
          >
            Eksport danych
          </Button>
        </Grid>
      </Grid>
    </Box>
  );
};