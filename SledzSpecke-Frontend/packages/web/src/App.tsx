import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider } from '@mui/material/styles';
import { CssBaseline } from '@mui/material';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { pl } from 'date-fns/locale';

import { theme } from './theme/theme';
import { useAuthStore } from './stores/authStore';
import { ModuleProvider } from './contexts/ModuleContext';

// Layout components
import { AppLayout } from './components/layout/AppLayout';
import { AuthLayout } from './components/layout/AuthLayout';

// Page components
import { LoginPage } from './pages/auth/LoginPage';
import { RegisterPage } from './pages/auth/RegisterPage';
import { DashboardPage } from './pages/dashboard/DashboardPage';
import { MedicalShiftsPage } from './pages/medical-shifts/MedicalShiftsPage';
import { ProceduresPage } from './pages/procedures/ProceduresPage';
import { InternshipsPage } from './pages/internships/InternshipsPage';
import { CoursesPage } from './pages/courses/CoursesPage';
import { SelfEducationPage } from './pages/self-education/SelfEducationPage';
import { ExportPage } from './pages/export/ExportPage';
import { PublicationsPage } from './pages/publications/PublicationsPage';
import { AbsencesPage } from './pages/absences/AbsencesPage';
import { SettingsPage } from './pages/settings/SettingsPage';
import { ProfilePage } from './pages/profile/ProfilePage';

// Create a client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      gcTime: 10 * 60 * 1000,   // 10 minutes
      retry: 3,
      retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 30000)
    }
  }
});

// Protected Route component
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  return <>{children}</>;
};

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeProvider theme={theme}>
        <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={pl}>
          <CssBaseline />
          <BrowserRouter>
            <Routes>
              {/* Auth routes */}
              <Route element={<AuthLayout />}>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
              </Route>
              
              {/* Protected routes */}
              <Route
                element={
                  <ProtectedRoute>
                    <ModuleProvider>
                      <AppLayout />
                    </ModuleProvider>
                  </ProtectedRoute>
                }
              >
                <Route path="/" element={<Navigate to="/dashboard" replace />} />
                <Route path="/dashboard" element={<DashboardPage />} />
                <Route path="/medical-shifts" element={<MedicalShiftsPage />} />
                <Route path="/procedures" element={<ProceduresPage />} />
                <Route path="/internships" element={<InternshipsPage />} />
                <Route path="/courses" element={<CoursesPage />} />
                <Route path="/self-education" element={<SelfEducationPage />} />
                <Route path="/publications" element={<PublicationsPage />} />
                <Route path="/absences" element={<AbsencesPage />} />
                <Route path="/export" element={<ExportPage />} />
                <Route path="/settings" element={<SettingsPage />} />
                <Route path="/profile" element={<ProfilePage />} />
              </Route>
            </Routes>
          </BrowserRouter>
        </LocalizationProvider>
      </ThemeProvider>
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}

export default App
