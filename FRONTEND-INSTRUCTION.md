# SledzSpecke Frontend Development Guide - React Web & React Native

## Development Progress Tracker

### Phase 1: React Web Implementation
- [x] **Project Setup** ✅ COMPLETED
  - [x] Create monorepo structure
  - [x] Set up shared package with domain entities
  - [x] Initialize React web app with Vite
  - [x] Configure TypeScript and ESLint
- [x] **Core Infrastructure** ✅ COMPLETED
  - [x] Set up routing with React Router
  - [x] Configure Material-UI theme
  - [x] Implement API client with Axios
  - [x] Set up React Query and Zustand
- [x] **Authentication Module** ✅ COMPLETED
  - [x] Create login page
  - [x] Create registration page
  - [x] Implement JWT token management
  - [x] Add protected routes
  - [x] Fix API integration (Username/Password fields)
- [x] **Dashboard Module** ✅ COMPLETED
  - [x] Create dashboard layout
  - [x] Implement module progress cards
  - [x] Add module switching (Basic/Specialist)
  - [x] Create statistics overview
- [x] **Medical Shifts Management** ✅ COMPLETED
  - [x] List medical shifts with table view
  - [x] Add/Edit shift form with date picker
  - [x] Delete functionality with confirmation
  - [x] Training year filtering (Rok 1-5)
  - [x] Progress tracking with visual bars
  - [x] Mock data fallback for development
- [x] **Procedures Management** ✅ COMPLETED
  - [x] List procedures with search functionality
  - [x] Add/Edit procedure form
  - [x] Operator level selection (Operator/Assistant)
  - [x] Patient information tracking
  - [x] ICD code support
  - [x] Statistics overview
- [x] **Internships Management** ✅ COMPLETED
  - [x] Card-based layout with progress
  - [x] Add/Edit internship form
  - [x] Module selection from specialization
  - [x] Progress tracking (days completed)
  - [x] Visual indicators for completion
- [x] **Courses & Training** ✅ COMPLETED
  - [x] Courses list with table/card views
  - [x] Add/Edit course form
  - [x] Course type categorization
  - [x] Credit hours tracking
  - [x] Certificate number support
- [x] **Self-Education Module** ✅ COMPLETED
  - [x] Activity tracking with categories
  - [x] Add/Edit activity form
  - [x] Credit hours progress tracking
  - [x] Activity type icons and filtering
- [x] **Export Functionality** ✅ COMPLETED
  - [x] General export (PDF/Excel/JSON)
  - [x] SMK export (Old/New versions)
  - [x] Date range filtering
  - [x] Selective data export
  - [x] Mock export implementation
- [ ] **Testing & Deployment** 🚧 PENDING
  - [ ] Unit tests for critical components
  - [ ] Integration tests for API calls
  - [ ] Build optimization
  - [ ] Deploy to VPS

### Phase 2: React Native Mobile
- [ ] **Project Setup**
- [ ] **Core Components Migration**
- [ ] **Platform-Specific Features**
- [ ] **Testing & Release**

---

## Executive Summary

SledzSpecke is a medical specialization tracking application for Polish doctors, designed to mirror the official SMK (System Monitorowania Kształcenia) system. This guide outlines a comprehensive approach to building both web (React) and mobile (React Native) frontends with a shared codebase where possible.

### Key Technology Decisions
- **Web First**: Deploy React web app on VPS for immediate access
- **Mobile Second**: React Native for iOS/Android using Expo
- **Code Sharing**: Maximum reuse of business logic, types, and utilities
- **State Management**: Zustand for simplicity + React Query for server state
- **UI Libraries**: Material-UI for web, React Native Paper for mobile
- **Form Handling**: React Hook Form with Zod validation
- **Architecture**: Clean Architecture with SOLID principles

---

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Technology Stack](#technology-stack)
3. [Project Structure](#project-structure)
4. [Shared Code Strategy](#shared-code-strategy)
5. [Phase 1: React Web Implementation](#phase-1-react-web-implementation)
6. [Phase 2: React Native Mobile](#phase-2-react-native-mobile)
7. [Design System](#design-system)
8. [State Management](#state-management)
9. [API Integration](#api-integration)
10. [Authentication Flow](#authentication-flow)
11. [Offline Support](#offline-support)
12. [Testing Strategy](#testing-strategy)
13. [Deployment](#deployment)
14. [Development Guidelines](#development-guidelines)

---

## 1. Architecture Overview

### Clean Architecture Layers
```
┌─────────────────────────────────────────────────┐
│                 Presentation                     │
│          (React Components / Screens)            │
├─────────────────────────────────────────────────┤
│                 Application                      │
│        (Use Cases, State Management)             │
├─────────────────────────────────────────────────┤
│                   Domain                         │
│      (Entities, Business Rules, Types)           │
├─────────────────────────────────────────────────┤
│                Infrastructure                    │
│        (API Client, Storage, External)           │
└─────────────────────────────────────────────────┘
```

### SOLID Principles Application
- **S**ingle Responsibility: Each component/module has one reason to change
- **O**pen/Closed: Use composition and props for extension
- **L**iskov Substitution: Interface consistency across platforms
- **I**nterface Segregation: Small, focused interfaces
- **D**ependency Inversion: Depend on abstractions, not implementations

---

## 2. Technology Stack

### Shared Dependencies
```json
{
  "dependencies": {
    "@tanstack/react-query": "^5.0.0",
    "zustand": "^4.4.0",
    "react-hook-form": "^7.47.0",
    "zod": "^3.22.0",
    "@hookform/resolvers": "^3.3.0",
    "date-fns": "^2.30.0",
    "axios": "^1.6.0"
  }
}
```

### React Web Specific
```json
{
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "react-router-dom": "^6.20.0",
    "@mui/material": "^5.14.0",
    "@mui/icons-material": "^5.14.0",
    "@mui/x-date-pickers": "^6.18.0",
    "recharts": "^2.9.0",
    "xlsx": "^0.18.5",
    "file-saver": "^2.0.5"
  },
  "devDependencies": {
    "vite": "^5.0.0",
    "@vitejs/plugin-react": "^4.2.0",
    "typescript": "^5.3.0"
  }
}
```

### React Native Specific
```json
{
  "dependencies": {
    "react-native": "0.73.0",
    "expo": "~50.0.0",
    "@react-navigation/native": "^6.1.0",
    "@react-navigation/drawer": "^6.6.0",
    "@react-navigation/bottom-tabs": "^6.5.0",
    "react-native-paper": "^5.11.0",
    "react-native-vector-icons": "^10.0.0",
    "@react-native-async-storage/async-storage": "~1.21.0",
    "expo-file-system": "~16.0.0",
    "expo-sharing": "~11.10.0"
  }
}
```

---

## 3. Project Structure

### Monorepo Structure
```
SledzSpecke/
├── packages/
│   ├── shared/                 # Shared code between web and mobile
│   │   ├── src/
│   │   │   ├── domain/        # Business entities and rules
│   │   │   │   ├── entities/
│   │   │   │   ├── value-objects/
│   │   │   │   └── services/
│   │   │   ├── application/   # Use cases and DTOs
│   │   │   │   ├── use-cases/
│   │   │   │   ├── dtos/
│   │   │   │   └── ports/
│   │   │   ├── infrastructure/# API clients and external services
│   │   │   │   ├── api/
│   │   │   │   └── storage/
│   │   │   └── utils/         # Shared utilities
│   │   └── package.json
│   │
│   ├── web/                   # React web application
│   │   ├── src/
│   │   │   ├── components/
│   │   │   ├── pages/
│   │   │   ├── hooks/
│   │   │   ├── styles/
│   │   │   └── App.tsx
│   │   ├── index.html
│   │   ├── vite.config.ts
│   │   └── package.json
│   │
│   └── mobile/                # React Native application
│       ├── src/
│       │   ├── components/
│       │   ├── screens/
│       │   ├── navigation/
│       │   ├── hooks/
│       │   └── App.tsx
│       ├── app.json
│       └── package.json
│
├── package.json              # Root package.json for workspaces
├── tsconfig.base.json        # Shared TypeScript config
└── .eslintrc.js             # Shared ESLint config
```

---

## 4. Shared Code Strategy

### Domain Layer (100% Shared)
```typescript
// packages/shared/src/domain/entities/MedicalShift.ts
export class MedicalShift {
  constructor(
    public readonly id: string,
    public readonly internshipId: string,
    public readonly date: Date,
    public readonly duration: Duration,
    public readonly location: string,
    public readonly year: number,
    public readonly syncStatus: SyncStatus
  ) {}

  isModified(): boolean {
    return this.syncStatus === SyncStatus.Modified;
  }

  canEdit(): boolean {
    return this.syncStatus !== SyncStatus.Approved;
  }
}

// packages/shared/src/domain/value-objects/Duration.ts
export class Duration {
  constructor(
    public readonly hours: number,
    public readonly minutes: number
  ) {
    if (hours < 0 || minutes < 0 || minutes >= 60) {
      throw new Error('Invalid duration');
    }
  }

  toTotalMinutes(): number {
    return this.hours * 60 + this.minutes;
  }

  toString(): string {
    return `${this.hours}h ${this.minutes}m`;
  }
}
```

### Application Layer (90% Shared)
```typescript
// packages/shared/src/application/use-cases/AddMedicalShift.ts
export interface AddMedicalShiftUseCase {
  execute(params: AddMedicalShiftParams): Promise<Result<MedicalShift>>;
}

export class AddMedicalShiftUseCaseImpl implements AddMedicalShiftUseCase {
  constructor(
    private medicalShiftRepository: MedicalShiftRepository,
    private internshipRepository: InternshipRepository
  ) {}

  async execute(params: AddMedicalShiftParams): Promise<Result<MedicalShift>> {
    // Validate internship exists
    const internship = await this.internshipRepository.findById(params.internshipId);
    if (!internship) {
      return Result.failure('Internship not found');
    }

    // Create medical shift
    const shift = new MedicalShift(
      generateId(),
      params.internshipId,
      params.date,
      new Duration(params.hours, params.minutes),
      params.location,
      params.year,
      SyncStatus.NotSynced
    );

    // Save to repository
    await this.medicalShiftRepository.save(shift);
    
    return Result.success(shift);
  }
}
```

### Infrastructure Layer (Platform Specific)
```typescript
// packages/shared/src/infrastructure/api/ApiClient.ts
export interface HttpClient {
  get<T>(url: string, config?: RequestConfig): Promise<T>;
  post<T>(url: string, data?: any, config?: RequestConfig): Promise<T>;
  put<T>(url: string, data?: any, config?: RequestConfig): Promise<T>;
  delete<T>(url: string, config?: RequestConfig): Promise<T>;
}

export class ApiClient {
  constructor(
    private httpClient: HttpClient,
    private baseURL: string,
    private tokenProvider: () => string | null
  ) {}

  private getHeaders(): Record<string, string> {
    const token = this.tokenProvider();
    return {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` })
    };
  }

  // API methods using the injected HTTP client
  async signIn(credentials: SignInDto): Promise<AuthResponse> {
    return this.httpClient.post(`${this.baseURL}/auth/sign-in`, credentials);
  }
}
```

---

## 5. Phase 1: React Web Implementation

### 5.1 Project Setup
```bash
# Create web project
cd packages/web
npm create vite@latest . -- --template react-ts

# Install dependencies
npm install @mui/material @emotion/react @emotion/styled
npm install react-router-dom
npm install @tanstack/react-query zustand
npm install react-hook-form @hookform/resolvers zod
npm install recharts xlsx file-saver
```

### 5.2 Core Components Structure

#### Layout Components
```typescript
// packages/web/src/components/layout/AppLayout.tsx
import { Outlet } from 'react-router-dom';
import { AppBar, Drawer, Box } from '@mui/material';
import { Navigation } from './Navigation';
import { useAuthStore } from '@/stores/authStore';

export const AppLayout: React.FC = () => {
  const { user } = useAuthStore();
  
  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed">
        {/* Header content */}
      </AppBar>
      <Drawer variant="permanent">
        <Navigation />
      </Drawer>
      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <Outlet />
      </Box>
    </Box>
  );
};
```

#### Dashboard Components
```typescript
// packages/web/src/pages/Dashboard/Dashboard.tsx
import { Grid, Card, CardContent, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { ProgressCard } from './components/ProgressCard';
import { ModuleTabs } from './components/ModuleTabs';
import { getDashboardOverview } from '@/api/dashboard';

export const Dashboard: React.FC = () => {
  const { data, isLoading } = useQuery({
    queryKey: ['dashboard'],
    queryFn: getDashboardOverview
  });

  if (isLoading) return <LoadingSpinner />;

  return (
    <Box>
      <ModuleTabs currentModule={data.currentModule} />
      <Grid container spacing={3}>
        <Grid item xs={12} md={4}>
          <ProgressCard
            title="Staże"
            completed={data.internships.completed}
            total={data.internships.total}
            icon={<LocalHospital />}
          />
        </Grid>
        {/* Other progress cards */}
      </Grid>
    </Box>
  );
};
```

### 5.3 Form Implementation
```typescript
// packages/web/src/components/forms/MedicalShiftForm.tsx
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { TextField, Button, Box } from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers';

const schema = z.object({
  date: z.date(),
  hours: z.number().min(0).max(24),
  minutes: z.number().min(0).max(59),
  location: z.string().min(1).max(500),
  year: z.number().min(1).max(6)
});

type FormData = z.infer<typeof schema>;

export const MedicalShiftForm: React.FC<{ onSubmit: (data: FormData) => void }> = ({ onSubmit }) => {
  const { register, handleSubmit, control, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(schema)
  });

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
      <Controller
        name="date"
        control={control}
        render={({ field }) => (
          <DatePicker
            label="Data dyżuru"
            value={field.value}
            onChange={field.onChange}
            slotProps={{
              textField: {
                error: !!errors.date,
                helperText: errors.date?.message
              }
            }}
          />
        )}
      />
      
      <Box sx={{ display: 'flex', gap: 2 }}>
        <TextField
          {...register('hours', { valueAsNumber: true })}
          label="Godziny"
          type="number"
          error={!!errors.hours}
          helperText={errors.hours?.message}
        />
        <TextField
          {...register('minutes', { valueAsNumber: true })}
          label="Minuty"
          type="number"
          error={!!errors.minutes}
          helperText={errors.minutes?.message}
        />
      </Box>
      
      <TextField
        {...register('location')}
        label="Miejsce"
        error={!!errors.location}
        helperText={errors.location?.message}
      />
      
      <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
        <Button variant="outlined">Anuluj</Button>
        <Button type="submit" variant="contained">Zapisz</Button>
      </Box>
    </Box>
  );
};
```

### 5.4 State Management with Zustand
```typescript
// packages/web/src/stores/authStore.ts
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
  user: User | null;
  token: string | null;
  setAuth: (user: User, token: string) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      token: null,
      setAuth: (user, token) => set({ user, token }),
      logout: () => set({ user: null, token: null })
    }),
    {
      name: 'auth-storage'
    }
  )
);
```

---

## 6. Phase 2: React Native Mobile

### 6.1 Project Setup
```bash
# Create React Native project with Expo
cd packages/mobile
npx create-expo-app . --template typescript

# Install navigation
npm install @react-navigation/native @react-navigation/drawer @react-navigation/bottom-tabs
npm install react-native-screens react-native-safe-area-context react-native-gesture-handler react-native-reanimated

# Install UI library
npm install react-native-paper react-native-vector-icons

# Install shared dependencies
npm install @tanstack/react-query zustand
npm install react-hook-form @hookform/resolvers zod
```

### 6.2 Navigation Structure
```typescript
// packages/mobile/src/navigation/AppNavigator.tsx
import { NavigationContainer } from '@react-navigation/native';
import { createDrawerNavigator } from '@react-navigation/drawer';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';

const Drawer = createDrawerNavigator();
const Tab = createBottomTabNavigator();

const TabNavigator = () => (
  <Tab.Navigator>
    <Tab.Screen name="Dashboard" component={DashboardScreen} />
    <Tab.Screen name="Procedures" component={ProceduresScreen} />
    <Tab.Screen name="Shifts" component={ShiftsScreen} />
  </Tab.Navigator>
);

export const AppNavigator = () => (
  <NavigationContainer>
    <Drawer.Navigator>
      <Drawer.Screen name="Home" component={TabNavigator} />
      <Drawer.Screen name="Internships" component={InternshipsScreen} />
      <Drawer.Screen name="Courses" component={CoursesScreen} />
      {/* Other screens */}
    </Drawer.Navigator>
  </NavigationContainer>
);
```

### 6.3 Shared Component Wrapper Pattern
```typescript
// packages/mobile/src/components/forms/MedicalShiftForm.tsx
import { View } from 'react-native';
import { TextInput, Button, HelperText } from 'react-native-paper';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { medicalShiftSchema } from '@shared/schemas'; // Shared validation

export const MedicalShiftForm: React.FC = () => {
  const { control, handleSubmit, formState: { errors } } = useForm({
    resolver: zodResolver(medicalShiftSchema)
  });

  return (
    <View style={styles.container}>
      <Controller
        control={control}
        name="location"
        render={({ field: { onChange, value } }) => (
          <TextInput
            label="Miejsce"
            value={value}
            onChangeText={onChange}
            mode="outlined"
            error={!!errors.location}
          />
        )}
      />
      {errors.location && (
        <HelperText type="error">{errors.location.message}</HelperText>
      )}
      
      <View style={styles.buttonContainer}>
        <Button mode="outlined" onPress={onCancel}>Anuluj</Button>
        <Button mode="contained" onPress={handleSubmit(onSubmit)}>Zapisz</Button>
      </View>
    </View>
  );
};
```

---

## 7. Design System

### Color Palette (Based on Screenshots)
```typescript
// packages/shared/src/design/colors.ts
export const colors = {
  primary: {
    main: '#00BCD4',      // Turquoise (from screenshots)
    light: '#4DD0E1',
    dark: '#0097A7',
    contrast: '#FFFFFF'
  },
  secondary: {
    main: '#FFC107',      // Amber for warnings/pending
    light: '#FFD54F',
    dark: '#FFA000',
    contrast: '#000000'
  },
  error: {
    main: '#F44336',
    light: '#EF5350',
    dark: '#D32F2F'
  },
  success: {
    main: '#4CAF50',
    light: '#66BB6A',
    dark: '#388E3C'
  },
  grey: {
    50: '#FAFAFA',
    100: '#F5F5F5',
    200: '#EEEEEE',
    300: '#E0E0E0',
    400: '#BDBDBD',
    500: '#9E9E9E',
    600: '#757575',
    700: '#616161',
    800: '#424242',
    900: '#212121'
  }
};
```

### Typography System
```typescript
// packages/shared/src/design/typography.ts
export const typography = {
  h1: {
    fontSize: 32,
    fontWeight: '700',
    lineHeight: 40
  },
  h2: {
    fontSize: 24,
    fontWeight: '600',
    lineHeight: 32
  },
  h3: {
    fontSize: 20,
    fontWeight: '600',
    lineHeight: 28
  },
  body1: {
    fontSize: 16,
    fontWeight: '400',
    lineHeight: 24
  },
  body2: {
    fontSize: 14,
    fontWeight: '400',
    lineHeight: 20
  },
  caption: {
    fontSize: 12,
    fontWeight: '400',
    lineHeight: 16
  },
  button: {
    fontSize: 16,
    fontWeight: '500',
    letterSpacing: 0.5,
    textTransform: 'uppercase' as const
  }
};
```

### Spacing System
```typescript
// packages/shared/src/design/spacing.ts
export const spacing = {
  xs: 4,
  sm: 8,
  md: 16,
  lg: 24,
  xl: 32,
  xxl: 48
};

// Usage in components
const styles = StyleSheet.create({
  container: {
    padding: spacing.md,
    marginBottom: spacing.lg
  }
});
```

---

## 8. State Management

### React Query Configuration
```typescript
// packages/shared/src/config/queryClient.ts
import { QueryClient } from '@tanstack/react-query';

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      gcTime: 10 * 60 * 1000,   // 10 minutes
      retry: 3,
      retryDelay: attemptIndex => Math.min(1000 * 2 ** attemptIndex, 30000)
    },
    mutations: {
      retry: 1
    }
  }
});
```

### Custom Hooks for Data Fetching
```typescript
// packages/shared/src/hooks/useMedicalShifts.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getMedicalShifts, createMedicalShift } from '@/api/medicalShifts';

export const useMedicalShifts = (internshipId?: string) => {
  return useQuery({
    queryKey: ['medicalShifts', internshipId],
    queryFn: () => getMedicalShifts(internshipId),
    enabled: !!internshipId
  });
};

export const useCreateMedicalShift = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: createMedicalShift,
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ['medicalShifts'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    }
  });
};
```

### Global App State with Zustand
```typescript
// packages/shared/src/stores/appStore.ts
import { create } from 'zustand';
import { subscribeWithSelector } from 'zustand/middleware';

interface AppState {
  currentModule: ModuleType;
  selectedInternship: string | null;
  syncStatus: SyncStatus;
  setCurrentModule: (module: ModuleType) => void;
  setSelectedInternship: (id: string | null) => void;
  setSyncStatus: (status: SyncStatus) => void;
}

export const useAppStore = create<AppState>()(
  subscribeWithSelector((set) => ({
    currentModule: ModuleType.Basic,
    selectedInternship: null,
    syncStatus: SyncStatus.Synced,
    setCurrentModule: (module) => set({ currentModule: module }),
    setSelectedInternship: (id) => set({ selectedInternship: id }),
    setSyncStatus: (status) => set({ syncStatus: status })
  }))
);
```

---

## 9. API Integration

### API Client Factory
```typescript
// packages/shared/src/infrastructure/api/ApiClientFactory.ts
export class ApiClientFactory {
  static create(platform: 'web' | 'mobile'): ApiClient {
    const httpClient = platform === 'web' 
      ? new AxiosHttpClient() 
      : new FetchHttpClient();
    
    const storage = platform === 'web'
      ? new LocalStorageAdapter()
      : new AsyncStorageAdapter();
    
    const tokenProvider = () => storage.getItem('authToken');
    
    return new ApiClient(
      httpClient,
      process.env.API_BASE_URL || 'http://localhost:5000/api',
      tokenProvider
    );
  }
}
```

### Type-Safe API Methods
```typescript
// packages/shared/src/api/medicalShifts.ts
import { apiClient } from './client';

export interface CreateMedicalShiftDto {
  internshipId: number;
  date: string;
  hours: number;
  minutes: number;
  location: string;
  year: number;
}

export interface MedicalShiftDto {
  id: number;
  internshipId: number;
  date: string;
  duration: { hours: number; minutes: number };
  location: string;
  year: number;
  syncStatus: SyncStatus;
}

export const medicalShiftsApi = {
  getAll: async (internshipId?: number): Promise<MedicalShiftDto[]> => {
    const params = internshipId ? { internshipId } : {};
    return apiClient.get('/medical-shifts', { params });
  },
  
  getById: async (id: number): Promise<MedicalShiftDto> => {
    return apiClient.get(`/medical-shifts/${id}`);
  },
  
  create: async (data: CreateMedicalShiftDto): Promise<MedicalShiftDto> => {
    return apiClient.post('/medical-shifts', data);
  },
  
  update: async (id: number, data: Partial<CreateMedicalShiftDto>): Promise<MedicalShiftDto> => {
    return apiClient.put(`/medical-shifts/${id}`, data);
  },
  
  delete: async (id: number): Promise<void> => {
    return apiClient.delete(`/medical-shifts/${id}`);
  }
};
```

---

## 10. Authentication Flow

### Authentication Hook
```typescript
// packages/shared/src/hooks/useAuth.ts
import { useMutation } from '@tanstack/react-query';
import { useAuthStore } from '@/stores/authStore';
import { authApi } from '@/api/auth';
import { useNavigate } from 'react-router-dom'; // or navigation for RN

export const useAuth = () => {
  const { setAuth, logout } = useAuthStore();
  const navigate = useNavigate();
  
  const signIn = useMutation({
    mutationFn: authApi.signIn,
    onSuccess: (data) => {
      setAuth(data.user, data.accessToken);
      navigate('/dashboard');
    }
  });
  
  const signUp = useMutation({
    mutationFn: authApi.signUp,
    onSuccess: (data) => {
      setAuth(data.user, data.accessToken);
      navigate('/onboarding');
    }
  });
  
  const signOut = () => {
    logout();
    navigate('/login');
  };
  
  return { signIn, signUp, signOut };
};
```

### Protected Route Component
```typescript
// packages/web/src/components/auth/ProtectedRoute.tsx
import { Navigate, Outlet } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';

export const ProtectedRoute: React.FC = () => {
  const { token } = useAuthStore();
  
  if (!token) {
    return <Navigate to="/login" replace />;
  }
  
  return <Outlet />;
};
```

---

## 11. Offline Support

### Offline Queue Manager
```typescript
// packages/shared/src/services/OfflineQueueManager.ts
interface QueuedRequest {
  id: string;
  type: 'create' | 'update' | 'delete';
  entity: EntityType;
  data: any;
  timestamp: Date;
}

export class OfflineQueueManager {
  private queue: QueuedRequest[] = [];
  private storage: StorageAdapter;
  
  constructor(storage: StorageAdapter) {
    this.storage = storage;
    this.loadQueue();
  }
  
  async addToQueue(request: Omit<QueuedRequest, 'id' | 'timestamp'>) {
    const queuedRequest: QueuedRequest = {
      ...request,
      id: generateId(),
      timestamp: new Date()
    };
    
    this.queue.push(queuedRequest);
    await this.saveQueue();
  }
  
  async processQueue(apiClient: ApiClient) {
    const failedRequests: QueuedRequest[] = [];
    
    for (const request of this.queue) {
      try {
        await this.processRequest(request, apiClient);
      } catch (error) {
        failedRequests.push(request);
      }
    }
    
    this.queue = failedRequests;
    await this.saveQueue();
  }
  
  private async processRequest(request: QueuedRequest, apiClient: ApiClient) {
    switch (request.entity) {
      case EntityType.MedicalShift:
        if (request.type === 'create') {
          await apiClient.medicalShifts.create(request.data);
        }
        // Handle other operations
        break;
      // Handle other entities
    }
  }
}
```

### Network Status Hook
```typescript
// packages/shared/src/hooks/useNetworkStatus.ts
import { useEffect, useState } from 'react';
import NetInfo from '@react-native-community/netinfo'; // For React Native

export const useNetworkStatus = () => {
  const [isOnline, setIsOnline] = useState(true);
  
  useEffect(() => {
    // Web implementation
    if (typeof window !== 'undefined' && 'navigator' in window) {
      setIsOnline(navigator.onLine);
      
      const handleOnline = () => setIsOnline(true);
      const handleOffline = () => setIsOnline(false);
      
      window.addEventListener('online', handleOnline);
      window.addEventListener('offline', handleOffline);
      
      return () => {
        window.removeEventListener('online', handleOnline);
        window.removeEventListener('offline', handleOffline);
      };
    }
    
    // React Native implementation
    const unsubscribe = NetInfo.addEventListener(state => {
      setIsOnline(state.isConnected ?? false);
    });
    
    return unsubscribe;
  }, []);
  
  return { isOnline };
};
```

---

## 12. Testing Strategy

### Unit Tests for Business Logic
```typescript
// packages/shared/src/domain/entities/__tests__/MedicalShift.test.ts
import { MedicalShift } from '../MedicalShift';
import { Duration } from '../../value-objects/Duration';
import { SyncStatus } from '../../value-objects/SyncStatus';

describe('MedicalShift', () => {
  it('should create a valid medical shift', () => {
    const shift = new MedicalShift(
      '1',
      '1',
      new Date('2024-01-15'),
      new Duration(8, 30),
      'Emergency Department',
      1,
      SyncStatus.NotSynced
    );
    
    expect(shift.isModified()).toBe(false);
    expect(shift.canEdit()).toBe(true);
  });
  
  it('should not allow editing approved shifts', () => {
    const shift = new MedicalShift(
      '1',
      '1',
      new Date('2024-01-15'),
      new Duration(8, 30),
      'Emergency Department',
      1,
      SyncStatus.Approved
    );
    
    expect(shift.canEdit()).toBe(false);
  });
});
```

### Integration Tests for API
```typescript
// packages/web/src/__tests__/integration/medicalShifts.test.tsx
import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useMedicalShifts } from '@/hooks/useMedicalShifts';

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } }
  });
  
  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  );
};

describe('Medical Shifts Integration', () => {
  it('should fetch medical shifts', async () => {
    const { result } = renderHook(
      () => useMedicalShifts('1'),
      { wrapper: createWrapper() }
    );
    
    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });
    
    expect(result.current.data).toHaveLength(3);
  });
});
```

### E2E Tests
```typescript
// packages/web/cypress/e2e/medical-shifts.cy.ts
describe('Medical Shifts Management', () => {
  beforeEach(() => {
    cy.login('test@example.com', 'password123');
    cy.visit('/medical-shifts');
  });
  
  it('should add a new medical shift', () => {
    cy.get('[data-testid="add-shift-button"]').click();
    
    cy.get('[name="date"]').type('2024-01-15');
    cy.get('[name="hours"]').type('8');
    cy.get('[name="minutes"]').type('30');
    cy.get('[name="location"]').type('Emergency Department');
    
    cy.get('[data-testid="submit-button"]').click();
    
    cy.contains('Dyżur został dodany pomyślnie').should('be.visible');
  });
});
```

---

## 13. Deployment

### Web Deployment (VPS)

#### Build Process
```bash
# Build the web app
cd packages/web
npm run build

# Output will be in dist/ folder
```

#### Nginx Configuration
```nginx
server {
    listen 80;
    listen [::]:80;
    server_name sledzspecke.pl www.sledzspecke.pl;
    
    # Redirect to HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name sledzspecke.pl www.sledzspecke.pl;
    
    ssl_certificate /etc/letsencrypt/live/sledzspecke.pl/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/sledzspecke.pl/privkey.pem;
    
    root /var/www/sledzspecke-web/dist;
    index index.html;
    
    # Gzip compression
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
    
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    # API proxy
    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
    
    # Cache static assets
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

#### Deployment Script
```bash
#!/bin/bash
# deploy-web.sh

# Build the application
cd packages/web
npm run build

# Copy to server
rsync -avz --delete dist/ user@server:/var/www/sledzspecke-web/dist/

# Reload nginx
ssh user@server "sudo nginx -t && sudo systemctl reload nginx"
```

### Mobile Deployment

#### Android Build (React Native)
```bash
# EAS Build for production
cd packages/mobile
eas build --platform android --profile production

# Local build
npx react-native bundle --platform android --dev false --entry-file index.js --bundle-output android/app/src/main/assets/index.android.bundle
cd android
./gradlew assembleRelease
```

#### iOS Build
```bash
# EAS Build (requires Apple Developer account)
eas build --platform ios --profile production

# Local build (Mac only)
cd ios
pod install
xcodebuild -workspace SledzSpecke.xcworkspace -scheme SledzSpecke -configuration Release
```

---

## 14. Development Guidelines

### Code Style and Standards

#### TypeScript Configuration
```json
{
  "compilerOptions": {
    "target": "ES2020",
    "module": "ESNext",
    "lib": ["ES2020", "DOM"],
    "jsx": "react-jsx",
    "strict": true,
    "esModuleInterop": true,
    "skipLibCheck": true,
    "forceConsistentCasingInFileNames": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "paths": {
      "@shared/*": ["../shared/src/*"],
      "@/*": ["./src/*"]
    }
  }
}
```

#### ESLint Configuration
```javascript
module.exports = {
  extends: [
    'eslint:recommended',
    'plugin:@typescript-eslint/recommended',
    'plugin:react/recommended',
    'plugin:react-hooks/recommended',
    'prettier'
  ],
  parser: '@typescript-eslint/parser',
  plugins: ['@typescript-eslint', 'react', 'react-hooks'],
  rules: {
    'react/prop-types': 'off',
    'react/react-in-jsx-scope': 'off',
    '@typescript-eslint/explicit-module-boundary-types': 'off',
    '@typescript-eslint/no-unused-vars': ['error', { argsIgnorePattern: '^_' }],
    'no-console': ['warn', { allow: ['warn', 'error'] }]
  }
};
```

### Component Guidelines

#### 1. Single Responsibility Principle
```typescript
// ❌ Bad: Component doing too much
const MedicalShiftCard = ({ shift, onEdit, onDelete, onSync }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState(shift);
  
  // Form handling logic
  // Sync logic
  // Delete confirmation logic
  
  return (/* Complex JSX */);
};

// ✅ Good: Separated concerns
const MedicalShiftCard = ({ shift, onAction }) => (
  <Card>
    <MedicalShiftInfo shift={shift} />
    <MedicalShiftActions shift={shift} onAction={onAction} />
  </Card>
);

const MedicalShiftInfo = ({ shift }) => (/* Display logic */);
const MedicalShiftActions = ({ shift, onAction }) => (/* Action buttons */);
```

#### 2. Composition over Inheritance
```typescript
// ✅ Use composition for shared behavior
const withLoadingState = <P extends object>(
  Component: React.ComponentType<P>
): React.FC<P & { isLoading: boolean }> => {
  return ({ isLoading, ...props }) => {
    if (isLoading) return <LoadingSpinner />;
    return <Component {...(props as P)} />;
  };
};

const MedicalShiftList = withLoadingState(({ shifts }) => (
  <List>
    {shifts.map(shift => <MedicalShiftCard key={shift.id} shift={shift} />)}
  </List>
));
```

#### 3. Custom Hooks for Logic Reuse
```typescript
// ✅ Extract complex logic into custom hooks
const useFormWithValidation = <T extends Record<string, any>>(
  initialValues: T,
  validationSchema: z.ZodSchema<T>
) => {
  const [values, setValues] = useState(initialValues);
  const [errors, setErrors] = useState<Partial<Record<keyof T, string>>>({});
  
  const validate = () => {
    try {
      validationSchema.parse(values);
      setErrors({});
      return true;
    } catch (error) {
      if (error instanceof z.ZodError) {
        const fieldErrors = error.flatten().fieldErrors;
        setErrors(fieldErrors as any);
      }
      return false;
    }
  };
  
  const handleChange = (field: keyof T) => (value: any) => {
    setValues(prev => ({ ...prev, [field]: value }));
  };
  
  return { values, errors, handleChange, validate };
};
```

### Performance Optimization

#### 1. Memoization
```typescript
// ✅ Memoize expensive computations
const DashboardStats = ({ data }) => {
  const statistics = useMemo(() => 
    calculateComplexStatistics(data),
    [data]
  );
  
  return <StatsDisplay stats={statistics} />;
};

// ✅ Memoize components to prevent unnecessary re-renders
const MedicalShiftCard = memo(({ shift, onEdit }) => {
  return (/* Component JSX */);
}, (prevProps, nextProps) => {
  return prevProps.shift.id === nextProps.shift.id &&
         prevProps.shift.syncStatus === nextProps.shift.syncStatus;
});
```

#### 2. Code Splitting
```typescript
// ✅ Lazy load heavy components
const Statistics = lazy(() => import('./pages/Statistics'));
const ExportModal = lazy(() => import('./components/ExportModal'));

// Usage with Suspense
<Suspense fallback={<LoadingSpinner />}>
  <Statistics />
</Suspense>
```

#### 3. Virtual Lists for Large Data
```typescript
// ✅ Use virtualization for long lists
import { FixedSizeList } from 'react-window';

const ProcedureList = ({ procedures }) => (
  <FixedSizeList
    height={600}
    itemCount={procedures.length}
    itemSize={80}
    width="100%"
  >
    {({ index, style }) => (
      <div style={style}>
        <ProcedureCard procedure={procedures[index]} />
      </div>
    )}
  </FixedSizeList>
);
```

### Error Handling

#### Global Error Boundary
```typescript
// packages/web/src/components/ErrorBoundary.tsx
class ErrorBoundary extends Component<Props, State> {
  state = { hasError: false, error: null };
  
  static getDerivedStateFromError(error: Error) {
    return { hasError: true, error };
  }
  
  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error('Error caught by boundary:', error, errorInfo);
    // Send to error reporting service
  }
  
  render() {
    if (this.state.hasError) {
      return <ErrorFallback error={this.state.error} />;
    }
    
    return this.props.children;
  }
}
```

#### API Error Handling
```typescript
// ✅ Centralized error handling
class ApiError extends Error {
  constructor(
    public statusCode: number,
    public message: string,
    public details?: any
  ) {
    super(message);
  }
}

const handleApiError = (error: unknown): never => {
  if (axios.isAxiosError(error)) {
    throw new ApiError(
      error.response?.status || 500,
      error.response?.data?.message || 'Network error',
      error.response?.data
    );
  }
  throw error;
};
```

### Accessibility

#### ARIA Labels and Roles
```typescript
// ✅ Proper accessibility attributes
const MedicalShiftCard = ({ shift }) => (
  <article
    role="article"
    aria-label={`Medical shift on ${format(shift.date, 'PP')}`}
  >
    <h3 id={`shift-${shift.id}-title`}>
      {format(shift.date, 'EEEE, MMMM d, yyyy')}
    </h3>
    <p aria-describedby={`shift-${shift.id}-title`}>
      Duration: {shift.duration.toString()}
    </p>
    <button
      aria-label={`Edit medical shift from ${format(shift.date, 'PP')}`}
      onClick={handleEdit}
    >
      Edit
    </button>
  </article>
);
```

### Security Best Practices

#### 1. Input Sanitization
```typescript
// ✅ Sanitize user input
import DOMPurify from 'dompurify';

const SafeHTML = ({ html }: { html: string }) => (
  <div
    dangerouslySetInnerHTML={{
      __html: DOMPurify.sanitize(html)
    }}
  />
);
```

#### 2. Secure Storage
```typescript
// ✅ Encrypt sensitive data
import CryptoJS from 'crypto-js';

class SecureStorage {
  private key: string;
  
  constructor(key: string) {
    this.key = key;
  }
  
  setItem(key: string, value: any): void {
    const encrypted = CryptoJS.AES.encrypt(
      JSON.stringify(value),
      this.key
    ).toString();
    localStorage.setItem(key, encrypted);
  }
  
  getItem(key: string): any {
    const encrypted = localStorage.getItem(key);
    if (!encrypted) return null;
    
    const decrypted = CryptoJS.AES.decrypt(encrypted, this.key);
    return JSON.parse(decrypted.toString(CryptoJS.enc.Utf8));
  }
}
```

### Documentation

#### Component Documentation
```typescript
/**
 * Medical shift management card component
 * 
 * @component
 * @example
 * ```tsx
 * <MedicalShiftCard
 *   shift={shift}
 *   onEdit={(shift) => console.log('Edit', shift)}
 *   onDelete={(id) => console.log('Delete', id)}
 * />
 * ```
 */
interface MedicalShiftCardProps {
  /** The medical shift data to display */
  shift: MedicalShift;
  /** Callback when edit button is clicked */
  onEdit: (shift: MedicalShift) => void;
  /** Callback when delete button is clicked */
  onDelete: (id: string) => void;
  /** Whether the card is in loading state */
  isLoading?: boolean;
}
```

---

## Summary

This comprehensive guide provides a production-ready approach to building SledzSpecke with:

1. **Shared Architecture**: Maximum code reuse between web and mobile
2. **Type Safety**: Full TypeScript coverage with proper typing
3. **Performance**: Optimized rendering and data fetching
4. **User Experience**: Consistent design system based on existing UI
5. **Maintainability**: Clean architecture with SOLID principles
6. **Scalability**: Modular structure ready for growth
7. **Security**: Best practices for authentication and data protection
8. **Testing**: Comprehensive testing strategy
9. **Deployment**: Ready for production deployment on VPS

The approach prioritizes delivering a working web application first, then extending to mobile platforms while maintaining code quality and user experience throughout.

---

## Current Implementation Status (December 2024)

### Completed Features

#### 1. **Authentication System**
- Login and registration pages with Material-UI components
- JWT token management with Zustand persistence
- Protected routes using React Router
- API integration fixed to match backend expectations (Username/Password fields)
- Auth state persistence across page refreshes

#### 2. **Dashboard**
- Overview cards showing progress for all modules
- Module switching between Basic and Specialist tracks
- Visual progress indicators
- Quick action buttons for common tasks
- Responsive grid layout

#### 3. **Medical Shifts (Dyżury)**
- Full CRUD operations with confirmation dialogs
- Training year filtering (Rok 1-5) with chip selection
- Progress tracking showing hours/minutes completed vs required (1048h)
- Visual progress bar with color coding
- Table view with sync status indicators
- Form validation with Polish locale date picker

#### 4. **Procedures (Procedury)**
- Search functionality across procedure name, code, and location
- Operator level tracking (Operator, First Assistant, Second Assistant)
- Patient information (age, sex) and ICD codes
- Statistics overview showing totals by role
- Common procedure templates for quick selection
- Sync status management preventing edits of approved items

#### 5. **Internships (Staże)**
- Card-based layout showing institution details
- Progress tracking with days completed/required
- Visual indicators for completion status
- Warning icons for internships ending soon
- Module selection from specialization requirements
- Supervisor and department information

#### 6. **Courses (Kursy)**
- Dual view modes (table and card layouts)
- Course type categorization with color coding
- Credit hours tracking with statistics
- Certificate number management
- Date range support for multi-day events
- Notes field for additional information

#### 7. **Self-Education (Samokształcenie)**
- Activity type categorization with icons
- Credit hours progress tracking (100 points required)
- Visual progress bar with percentage
- Activity suggestions based on type
- Table view with search capabilities

#### 8. **Export Functionality**
- General export supporting PDF, Excel, and JSON formats
- SMK-specific export for Old and New system versions
- Date range filtering for exports
- Selective data export with checkboxes
- Mock implementation using file-saver library
- Export history placeholder

### Technical Implementation Details

#### Architecture Decisions
1. **Monorepo Structure**: Packages for web, shared domain logic
2. **State Management**: Zustand for client state, React Query for server state
3. **Form Handling**: React Hook Form with Zod validation
4. **UI Framework**: Material-UI v5 with custom theme
5. **Date Handling**: date-fns with Polish locale
6. **Build Tool**: Vite for fast development
7. **API Client**: Axios with interceptors for auth

#### Mock Data Strategy
- All services include fallback to mock data when API fails
- Enables frontend development independent of backend
- Mock data follows same structure as API responses
- Simulates sync status and approval workflows

#### Polish Localization
- All UI text in Polish
- Polish date formats (dd.MM.yyyy)
- Polish locale for date pickers
- Medical terminology appropriate for Polish healthcare

### Key Files and Locations

```
packages/
├── shared/
│   └── src/
│       ├── domain/
│       │   ├── entities/      # User, MedicalShift, Procedure, etc.
│       │   └── value-objects/ # Duration, SyncStatus, SmkVersion
│       ├── types/            # TypeScript interfaces
│       └── utils/            # Shared utilities
└── web/
    └── src/
        ├── components/
        │   ├── forms/        # All form components
        │   ├── layout/       # AppLayout, Navigation
        │   └── dashboard/    # Dashboard-specific components
        ├── pages/           # Page components by feature
        ├── services/        # API services with mock data
        ├── hooks/           # React Query hooks
        └── stores/          # Zustand stores
```

### Pending Work

1. **API Integration Testing**: Need to test all endpoints with live backend
2. **Error Handling**: Add proper error boundaries and user feedback
3. **Loading States**: Improve skeleton screens and loading indicators
4. **Validation**: Add more comprehensive form validation rules
5. **Performance**: Implement lazy loading and code splitting
6. **Tests**: Add unit and integration tests
7. **Deployment**: Configure nginx and deploy to VPS

### Known Issues

1. **API Endpoints**: Some endpoints return 404 or dependency injection errors
2. **Authentication**: Backend expects Username field, not email
3. **Date Validation**: Need to add business rules for date ranges
4. **File Size**: Bundle size is large (>900KB), needs optimization

### Recommendations for Next Phase

1. **Test with Real API**: Ensure all CRUD operations work with backend
2. **Add Publications Module**: Complete the remaining feature
3. **Implement Absences**: Add absence tracking functionality
4. **Optimize Bundle**: Use dynamic imports and tree shaking
5. **Add PWA Support**: Enable offline functionality
6. **Implement Real Export**: Replace mock export with actual file generation
7. **Add Data Validation**: Implement business rules from SMK requirements
8. **Performance Monitoring**: Add analytics and error tracking

---

## Production Readiness Plan - Frontend Focus (December 2024)

### 🚀 Current Status: 95% Feature Complete, 70% Production Ready

### 🔴 CRITICAL - Must Fix Before Production (Week 1)

#### 1. API Integration Issues
```typescript
// Current (broken)
apiClient.post('/auth/login', {...})  // ❌ 404 Error

// Should be
apiClient.post('/auth/sign-in', {...}) // ✅ Correct endpoint
```

**Action Items:**
- [ ] Audit all API endpoints against backend Swagger
- [ ] Update service methods to match actual endpoints
- [ ] Test every CRUD operation with live API
- [ ] Add proper error handling for network failures

#### 2. Bundle Size Optimization (Currently 942KB)
```javascript
// vite.config.ts updates needed
export default defineConfig({
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          'vendor': ['react', 'react-dom', 'react-router-dom'],
          'mui': ['@mui/material', '@mui/icons-material'],
          'forms': ['react-hook-form', '@hookform/resolvers', 'zod'],
          'data': ['@tanstack/react-query', 'axios', 'zustand']
        }
      }
    }
  }
});
```

**Target: < 500KB initial bundle**
- [ ] Implement code splitting
- [ ] Lazy load routes
- [ ] Remove unused dependencies
- [ ] Enable gzip compression

#### 3. Environment Configuration
```typescript
// .env.production
VITE_API_URL=https://api.sledzspecke.pl
VITE_APP_VERSION=1.0.0
VITE_SENTRY_DSN=your-sentry-dsn
VITE_GA_TRACKING_ID=your-ga-id
```

### 🟡 IMPORTANT - Should Fix Before Production (Week 2)

#### 1. Error Handling & User Feedback
```typescript
// Global error boundary
class ErrorBoundary extends Component<Props, State> {
  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error('Error caught:', error);
    // Send to Sentry
    Sentry.captureException(error, { contexts: { react: errorInfo } });
  }
  
  render() {
    if (this.state.hasError) {
      return (
        <ErrorFallback 
          error={this.state.error}
          resetError={() => this.setState({ hasError: false })}
        />
      );
    }
    return this.props.children;
  }
}
```

#### 2. Loading States & Skeletons
```typescript
// Implement skeleton screens
const MedicalShiftSkeleton = () => (
  <TableRow>
    <TableCell><Skeleton variant="text" /></TableCell>
    <TableCell><Skeleton variant="text" /></TableCell>
    <TableCell><Skeleton variant="text" /></TableCell>
    <TableCell><Skeleton variant="circular" width={40} height={40} /></TableCell>
  </TableRow>
);
```

#### 3. Form Validation Enhancements
```typescript
// Add business rules
const medicalShiftSchema = z.object({
  date: z.date()
    .max(new Date(), 'Data nie może być w przyszłości')
    .refine(date => {
      const daysDiff = differenceInDays(new Date(), date);
      return daysDiff <= 30;
    }, 'Dyżur nie może być starszy niż 30 dni'),
  // ... other validations
});
```

### 🟢 NICE TO HAVE - Post-Launch Improvements

#### 1. Progressive Web App
```javascript
// vite-plugin-pwa configuration
PWA({
  registerType: 'autoUpdate',
  includeAssets: ['favicon.ico', 'apple-touch-icon.png', 'mask-icon.svg'],
  manifest: {
    name: 'SledzSpecke',
    short_name: 'SledzSpecke',
    description: 'System śledzenia specjalizacji medycznej',
    theme_color: '#00BCD4',
    icons: [...]
  }
})
```

#### 2. Performance Monitoring
```typescript
// Web Vitals tracking
import { getCLS, getFID, getFCP, getLCP, getTTFB } from 'web-vitals';

function sendToAnalytics(metric: Metric) {
  // Send to Google Analytics
  gtag('event', metric.name, {
    value: Math.round(metric.name === 'CLS' ? metric.value * 1000 : metric.value),
    event_label: metric.id,
    non_interaction: true,
  });
}

getCLS(sendToAnalytics);
getFID(sendToAnalytics);
getFCP(sendToAnalytics);
getLCP(sendToAnalytics);
getTTFB(sendToAnalytics);
```

### 📋 Pre-Production Checklist

#### Code Quality
- [ ] Run `npm run lint` - fix all warnings
- [ ] Run `npm run type-check` - no TypeScript errors
- [ ] Remove all `console.log` statements
- [ ] Add proper TypeScript types (no `any`)
- [ ] Review and remove unused code

#### Testing (Minimum Viable)
- [ ] Test all forms with validation
- [ ] Test error states (network failure, 404, 500)
- [ ] Test on different browsers (Chrome, Firefox, Safari)
- [ ] Test on mobile devices
- [ ] Test with slow network (3G throttling)

#### Security
- [ ] Ensure all API calls use HTTPS
- [ ] Add Content Security Policy headers
- [ ] Sanitize user inputs (already using Zod)
- [ ] Review authentication flow
- [ ] Check for exposed secrets

#### Performance
- [ ] Lighthouse score > 90
- [ ] First Contentful Paint < 1.5s
- [ ] Time to Interactive < 3s
- [ ] Bundle size < 500KB (gzipped)

#### Deployment
- [ ] Build production bundle
- [ ] Test production build locally
- [ ] Configure nginx for SPA
- [ ] Set up SSL certificates
- [ ] Configure caching headers

### 🚢 Deployment Configuration

#### Nginx Configuration
```nginx
server {
    listen 443 ssl http2;
    server_name sledzspecke.pl www.sledzspecke.pl;
    
    ssl_certificate /etc/letsencrypt/live/sledzspecke.pl/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/sledzspecke.pl/privkey.pem;
    
    root /var/www/sledzspecke/dist;
    index index.html;
    
    # Gzip
    gzip on;
    gzip_vary on;
    gzip_types text/css application/javascript application/json;
    
    # SPA routing
    location / {
        try_files $uri $uri/ /index.html;
    }
    
    # API proxy
    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
    
    # Cache static assets
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
    
    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Referrer-Policy "strict-origin-when-cross-origin" always;
    add_header Content-Security-Policy "default-src 'self'; script-src 'self' 'unsafe-inline' https://www.google-analytics.com; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; connect-src 'self' https://api.sledzspecke.pl" always;
}
```

#### GitHub Actions Deployment
```yaml
name: Deploy Frontend

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: '20'
          
      - name: Install dependencies
        run: npm ci
        
      - name: Build
        run: npm run build
        env:
          VITE_API_URL: https://api.sledzspecke.pl
          
      - name: Deploy to VPS
        uses: appleboy/ssh-action@v0.1.5
        with:
          host: ${{ secrets.VPS_HOST }}
          username: ${{ secrets.VPS_USER }}
          key: ${{ secrets.VPS_SSH_KEY }}
          script: |
            cd /var/www/sledzspecke
            git pull
            npm ci
            npm run build
            sudo nginx -s reload
```

### 📊 Success Metrics

**Technical Metrics:**
- Page Load Time: < 3s on 3G
- Error Rate: < 0.1%
- Uptime: > 99.9%
- Bundle Size: < 500KB

**User Metrics:**
- Daily Active Users
- Feature Adoption Rate
- User Retention (30-day)
- Support Tickets

**Business Metrics:**
- Successful SMK Exports
- Data Accuracy
- Time Saved vs Manual Entry

### 🎯 Go-Live Strategy

**Week 1: Critical Fixes**
- Fix all API integration issues
- Optimize bundle size
- Set up production environment

**Week 2: Testing & Polish**
- Comprehensive testing
- Performance optimization
- Security hardening

**Week 3: Soft Launch**
- Deploy to production
- Monitor closely
- Gather feedback from beta users

**Week 4: Full Launch**
- Public announcement
- Marketing campaign
- Support team ready

### 🆘 Rollback Plan

1. **Quick Rollback** (< 5 minutes)
   ```bash
   cd /var/www/sledzspecke
   git checkout previous-release-tag
   npm ci && npm run build
   sudo nginx -s reload
   ```

2. **Database Rollback** (if needed)
   ```bash
   pg_restore -d sledzspecke backup_before_deploy.sql
   ```

3. **Communication**
   - Status page update
   - Email to affected users
   - Support team notification

### 💡 Final Recommendations

1. **Start with MVP**: Launch with core features, add others based on feedback
2. **Monitor Everything**: Set up comprehensive monitoring before launch
3. **Have a War Room**: First 48 hours after launch, have team ready
4. **Collect Feedback**: In-app feedback widget for quick user input
5. **Iterate Quickly**: Plan weekly releases for fixes and improvements

The frontend is **feature-complete** but needs **production hardening**. With 2-3 weeks of focused effort, it will be ready for a successful launch.