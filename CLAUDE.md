# SledzSpecke React Native - Complete Development Roadmap for Claude Code

## Project Overview

**SledzSpecke** is a medical specialization tracking app for doctors in Poland. It mirrors the official SMK (System Monitorowania Kształcenia) system structure, allowing doctors to track their specialization progress on mobile devices and export data in SMK-compatible format.

### Key Requirements
- Support for both "old" and "new" SMK versions
- Module-based specializations (Basic Module + Specialist Module)
- Offline-first with sync capabilities
- Export to Excel/PDF in SMK format
- Polish language UI with English codebase

### Technology Stack
- React Native + Expo
- TypeScript
- React Navigation (Drawer + Tabs)
- React Native Paper (Material Design)
- React Query + Zustand
- React Hook Form + Zod validation

---

## Phase 1: Project Setup and Core Infrastructure (Days 1-3)

### Step 1.1: Initialize Project
```bash
npx create-expo-app SledzSpecke --template typescript
cd SledzSpecke
```

### Step 1.2: Install Dependencies
```bash
# Navigation
npm install @react-navigation/native @react-navigation/native-stack @react-navigation/bottom-tabs @react-navigation/drawer
npm install react-native-screens react-native-safe-area-context react-native-gesture-handler react-native-reanimated

# UI Components
npm install react-native-paper react-native-vector-icons
npm install react-native-svg

# State Management & Data Fetching
npm install @tanstack/react-query zustand
npm install @react-native-async-storage/async-storage

# Forms & Validation
npm install react-hook-form @hookform/resolvers zod

# Date & Time
npm install date-fns react-native-date-picker

# Export functionality
npm install react-native-pdf xlsx
npm install expo-file-system expo-sharing

# Utilities
npm install react-native-toast-message
npm install axios

# Dev Dependencies
npm install -D @types/react @types/react-native
npm install -D eslint @typescript-eslint/eslint-plugin @typescript-eslint/parser
npm install -D prettier eslint-config-prettier eslint-plugin-prettier
```

### Step 1.3: Create Folder Structure
```
SledzSpecke/
├── App.tsx
├── app.json
├── tsconfig.json
├── babel.config.js
├── .env
├── .env.example
├── .eslintrc.js
├── .prettierrc
├── .gitignore
│
└── src/
    ├── app/
    │   ├── App.tsx
    │   ├── AppProviders.tsx
    │   └── RootNavigator.tsx
    ├── features/
    ├── shared/
    └── assets/
```

### Step 1.4: Configure TypeScript Path Aliases

**tsconfig.json:**
```json
{
  "extends": "expo/tsconfig.base",
  "compilerOptions": {
    "strict": true,
    "baseUrl": ".",
    "paths": {
      "@/*": ["src/*"],
      "@features/*": ["src/features/*"],
      "@shared/*": ["src/shared/*"],
      "@assets/*": ["src/assets/*"]
    }
  }
}
```

**babel.config.js:**
```javascript
module.exports = function(api) {
  api.cache(true);
  return {
    presets: ['babel-preset-expo'],
    plugins: [
      [
        'module-resolver',
        {
          root: ['./src'],
          alias: {
            '@': './src',
            '@features': './src/features',
            '@shared': './src/shared',
            '@assets': './src/assets'
          }
        }
      ],
      'react-native-reanimated/plugin'
    ]
  };
};
```

### Step 1.5: Setup Linting and Formatting

**.eslintrc.js:**
```javascript
module.exports = {
  root: true,
  extends: [
    '@react-native-community',
    'plugin:@typescript-eslint/recommended',
    'prettier'
  ],
  parser: '@typescript-eslint/parser',
  plugins: ['@typescript-eslint', 'prettier'],
  rules: {
    'prettier/prettier': 'error',
    '@typescript-eslint/no-unused-vars': 'error',
    '@typescript-eslint/explicit-function-return-type': 'off',
    'react-native/no-inline-styles': 'off'
  }
};
```

**.prettierrc:**
```json
{
  "singleQuote": true,
  "trailingComma": "es5",
  "tabWidth": 2,
  "semi": true,
  "bracketSpacing": true,
  "arrowParens": "always",
  "endOfLine": "lf"
}
```

### Step 1.6: Environment Configuration

**.env.example:**
```
API_BASE_URL=http://localhost:5000/api
```

---

## Phase 2: Theme and Shared Components (Days 4-5)

### Step 2.1: Create Theme System

**src/shared/theme/colors.ts:**
```typescript
export const colors = {
  // Primary colors (from screenshots)
  primary: '#22A6B3',      // Turquoise
  primaryLight: '#7FCDCD',
  primaryDark: '#1A8A96',
  
  // Secondary colors
  secondary: '#FFB800',    // Yellow/Orange for accents
  
  // Status colors
  success: '#51CF66',
  error: '#FF6B6B',
  warning: '#FFB800',
  info: '#3498DB',
  
  // Neutral colors
  background: '#F5F7FA',
  surface: '#FFFFFF',
  text: {
    primary: '#2C3E50',
    secondary: '#7F8C8D',
    disabled: '#BDBDBD',
    inverse: '#FFFFFF',
  },
  
  // UI elements
  border: '#E0E0E0',
  divider: '#ECEFF1',
  overlay: 'rgba(0, 0, 0, 0.5)',
  
  // Progress indicators
  progressBackground: '#E0E0E0',
  progressFill: '#22A6B3',
};
```

**src/shared/theme/spacing.ts:**
```typescript
export const spacing = {
  xs: 4,
  sm: 8,
  md: 16,
  lg: 24,
  xl: 32,
  xxl: 48,
};
```

**src/shared/theme/typography.ts:**
```typescript
export const typography = {
  h1: {
    fontSize: 28,
    fontWeight: '700' as const,
    lineHeight: 34,
  },
  h2: {
    fontSize: 24,
    fontWeight: '600' as const,
    lineHeight: 30,
  },
  h3: {
    fontSize: 20,
    fontWeight: '600' as const,
    lineHeight: 26,
  },
  body1: {
    fontSize: 16,
    fontWeight: '400' as const,
    lineHeight: 22,
  },
  body2: {
    fontSize: 14,
    fontWeight: '400' as const,
    lineHeight: 20,
  },
  caption: {
    fontSize: 12,
    fontWeight: '400' as const,
    lineHeight: 16,
  },
  button: {
    fontSize: 16,
    fontWeight: '500' as const,
    lineHeight: 20,
  },
};
```

**src/shared/theme/theme.ts:**
```typescript
import { MD3LightTheme as DefaultTheme } from 'react-native-paper';
import { colors } from './colors';

export const theme = {
  ...DefaultTheme,
  colors: {
    ...DefaultTheme.colors,
    primary: colors.primary,
    primaryContainer: colors.primaryLight,
    secondary: colors.secondary,
    tertiary: colors.info,
    surface: colors.surface,
    background: colors.background,
    error: colors.error,
    onPrimary: colors.text.inverse,
    onSurface: colors.text.primary,
    onSurfaceVariant: colors.text.secondary,
    outline: colors.border,
  },
  roundness: 8,
};
```

### Step 2.2: Create Core UI Components

**src/shared/components/ui/Button.tsx:**
```typescript
import React, { FC } from 'react';
import { Button as PaperButton, ButtonProps as PaperButtonProps } from 'react-native-paper';
import { StyleSheet } from 'react-native';
import { colors } from '@shared/theme';

interface ButtonProps extends Omit<PaperButtonProps, 'children'> {
  title: string;
  variant?: 'primary' | 'secondary' | 'outline';
}

export const Button: FC<ButtonProps> = ({ 
  title, 
  variant = 'primary',
  style,
  ...props 
}) => {
  const getMode = () => {
    switch (variant) {
      case 'outline':
        return 'outlined';
      case 'secondary':
        return 'contained-tonal';
      default:
        return 'contained';
    }
  };

  return (
    <PaperButton
      mode={getMode()}
      style={[styles.button, style]}
      contentStyle={styles.content}
      labelStyle={styles.label}
      {...props}
    >
      {title}
    </PaperButton>
  );
};

const styles = StyleSheet.create({
  button: {
    borderRadius: 8,
  },
  content: {
    paddingVertical: 8,
  },
  label: {
    fontSize: 16,
    fontWeight: '500',
  },
});
```

**src/shared/components/ui/Card.tsx:**
```typescript
import React, { FC, ReactNode } from 'react';
import { StyleSheet, TouchableOpacity, View } from 'react-native';
import { Surface, Text } from 'react-native-paper';
import { colors, spacing } from '@shared/theme';

interface CardProps {
  title?: string;
  subtitle?: string;
  children?: ReactNode;
  onPress?: () => void;
  rightContent?: ReactNode;
}

export const Card: FC<CardProps> = ({
  title,
  subtitle,
  children,
  onPress,
  rightContent,
}) => {
  const content = (
    <Surface style={styles.surface} elevation={2}>
      {(title || subtitle) && (
        <View style={styles.header}>
          <View style={styles.titleContainer}>
            {title && <Text style={styles.title}>{title}</Text>}
            {subtitle && <Text style={styles.subtitle}>{subtitle}</Text>}
          </View>
          {rightContent}
        </View>
      )}
      {children && <View style={styles.content}>{children}</View>}
    </Surface>
  );

  if (onPress) {
    return (
      <TouchableOpacity onPress={onPress} activeOpacity={0.8}>
        {content}
      </TouchableOpacity>
    );
  }

  return content;
};

const styles = StyleSheet.create({
  surface: {
    borderRadius: 8,
    marginVertical: spacing.xs,
    backgroundColor: colors.surface,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: spacing.md,
  },
  titleContainer: {
    flex: 1,
  },
  title: {
    fontSize: 16,
    fontWeight: '600',
    color: colors.text.primary,
  },
  subtitle: {
    fontSize: 14,
    color: colors.text.secondary,
    marginTop: 2,
  },
  content: {
    paddingHorizontal: spacing.md,
    paddingBottom: spacing.md,
  },
});
```

**src/shared/components/ui/ProgressBar.tsx:**
```typescript
import React, { FC } from 'react';
import { View, StyleSheet } from 'react-native';
import { Text } from 'react-native-paper';
import { colors, spacing } from '@shared/theme';

interface ProgressBarProps {
  progress: number; // 0-1
  label?: string;
  showPercentage?: boolean;
  height?: number;
  color?: string;
}

export const ProgressBar: FC<ProgressBarProps> = ({
  progress,
  label,
  showPercentage = true,
  height = 8,
  color = colors.primary,
}) => {
  const percentage = Math.round(progress * 100);

  return (
    <View style={styles.container}>
      {(label || showPercentage) && (
        <View style={styles.labelContainer}>
          {label && <Text style={styles.label}>{label}</Text>}
          {showPercentage && (
            <Text style={styles.percentage}>{percentage}%</Text>
          )}
        </View>
      )}
      <View style={[styles.progressBackground, { height }]}>
        <View
          style={[
            styles.progressFill,
            {
              width: `${percentage}%`,
              backgroundColor: color,
              height,
            },
          ]}
        />
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    width: '100%',
  },
  labelContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: spacing.xs,
  },
  label: {
    fontSize: 14,
    color: colors.text.secondary,
  },
  percentage: {
    fontSize: 14,
    fontWeight: '600',
    color: colors.text.primary,
  },
  progressBackground: {
    backgroundColor: colors.progressBackground,
    borderRadius: 4,
    overflow: 'hidden',
  },
  progressFill: {
    borderRadius: 4,
  },
});
```

---

## Phase 3: Navigation and App Structure (Days 6-7)

### Step 3.1: Create Navigation Types

**src/shared/types/navigation.types.ts:**
```typescript
import { NavigatorScreenParams } from '@react-navigation/native';

export type RootStackParamList = {
  Auth: NavigatorScreenParams<AuthStackParamList>;
  Main: NavigatorScreenParams<MainDrawerParamList>;
};

export type AuthStackParamList = {
  Login: undefined;
  Register: undefined;
};

export type MainDrawerParamList = {
  DashboardTab: NavigatorScreenParams<DashboardTabParamList>;
  Export: undefined;
  Settings: undefined;
};

export type DashboardTabParamList = {
  Dashboard: undefined;
  Procedures: NavigatorScreenParams<ProcedureStackParamList>;
  Shifts: NavigatorScreenParams<ShiftStackParamList>;
  Internships: NavigatorScreenParams<InternshipStackParamList>;
  Courses: undefined;
  SelfEducation: undefined;
  Publications: undefined;
  Absences: undefined;
};

export type ProcedureStackParamList = {
  ProcedureList: undefined;
  ProcedureDetail: { id: string };
  AddProcedure: { internshipId?: string };
};

export type ShiftStackParamList = {
  ShiftList: { year?: number };
  AddShift: { internshipId?: string };
};

export type InternshipStackParamList = {
  InternshipList: undefined;
  InternshipDetail: { id: string };
  AddInternship: undefined;
  EditInternship: { id: string };
};
```

### Step 3.2: Create Navigation Structure

**src/app/AppProviders.tsx:**
```typescript
import React, { FC, ReactNode } from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Provider as PaperProvider } from 'react-native-paper';
import Toast from 'react-native-toast-message';
import { theme } from '@shared/theme/theme';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 2,
      staleTime: 5 * 60 * 1000, // 5 minutes
      cacheTime: 10 * 60 * 1000, // 10 minutes
    },
  },
});

interface AppProvidersProps {
  children: ReactNode;
}

export const AppProviders: FC<AppProvidersProps> = ({ children }) => {
  return (
    <QueryClientProvider client={queryClient}>
      <PaperProvider theme={theme}>
        <NavigationContainer>
          {children}
          <Toast />
        </NavigationContainer>
      </PaperProvider>
    </QueryClientProvider>
  );
};
```

**src/features/navigation/navigators/DrawerNavigator.tsx:**
```typescript
import React from 'react';
import {
  createDrawerNavigator,
  DrawerContentComponentProps,
} from '@react-navigation/drawer';
import { DrawerContent } from '../components/DrawerContent';
import { TabNavigator } from './TabNavigator';
import { ExportScreen } from '@features/export/screens/ExportScreen';
import { SettingsScreen } from '@features/settings/screens/SettingsScreen';
import { MainDrawerParamList } from '@shared/types/navigation.types';

const Drawer = createDrawerNavigator<MainDrawerParamList>();

export const DrawerNavigator = () => {
  return (
    <Drawer.Navigator
      drawerContent={(props: DrawerContentComponentProps) => (
        <DrawerContent {...props} />
      )}
      screenOptions={{
        headerShown: false,
        drawerType: 'slide',
      }}
    >
      <Drawer.Screen
        name="DashboardTab"
        component={TabNavigator}
        options={{ title: 'Dashboard' }}
      />
      <Drawer.Screen
        name="Export"
        component={ExportScreen}
        options={{ title: 'Eksport' }}
      />
      <Drawer.Screen
        name="Settings"
        component={SettingsScreen}
        options={{ title: 'Ustawienia' }}
      />
    </Drawer.Navigator>
  );
};
```

---

## Phase 4: Authentication Feature (Days 8-10)

### Step 4.1: Create Auth Types

**src/features/auth/types/auth.types.ts:**
```typescript
export interface User {
  id: number;
  email: string;
  username: string;
  fullName: string;
  specializationId: number;
  specializationName: string;
  smkVersion: 'old' | 'new';
  currentModuleId: number;
}

export interface SignInDto {
  email: string;
  password: string;
}

export interface SignUpDto {
  email: string;
  username: string;
  password: string;
  fullName: string;
  smkVersion: 'old' | 'new';
  specializationId: number;
}

export interface AuthResponse {
  accessToken: string;
  userId: number;
  email: string;
  fullName: string;
  specializationId: number;
  smkVersion: 'old' | 'new';
}

export interface Specialization {
  id: number;
  name: string;
  code: string;
  durationYears: number;
  hasModules: boolean;
}
```

### Step 4.2: Create Auth Store

**src/features/auth/store/authStore.ts:**
```typescript
import { create } from 'zustand';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { api } from '@shared/services/api/apiClient';
import { User, SignInDto, SignUpDto } from '../types/auth.types';

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  
  signIn: (data: SignInDto) => Promise<void>;
  signUp: (data: SignUpDto) => Promise<void>;
  signOut: () => Promise<void>;
  loadUser: () => Promise<void>;
  updateUser: (user: Partial<User>) => void;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  user: null,
  isAuthenticated: false,
  isLoading: true,

  signIn: async (data) => {
    try {
      const response = await api.auth.signIn(data);
      const { accessToken, ...userData } = response.data;
      
      await AsyncStorage.setItem('accessToken', accessToken);
      await AsyncStorage.setItem('user', JSON.stringify(userData));
      
      set({ 
        user: userData as User, 
        isAuthenticated: true 
      });
    } catch (error) {
      throw error;
    }
  },

  signUp: async (data) => {
    try {
      const response = await api.auth.signUp(data);
      const { accessToken, ...userData } = response.data;
      
      await AsyncStorage.setItem('accessToken', accessToken);
      await AsyncStorage.setItem('user', JSON.stringify(userData));
      
      set({ 
        user: userData as User, 
        isAuthenticated: true 
      });
    } catch (error) {
      throw error;
    }
  },

  signOut: async () => {
    await AsyncStorage.multiRemove(['accessToken', 'user']);
    set({ user: null, isAuthenticated: false });
  },

  loadUser: async () => {
    try {
      const [token, userStr] = await AsyncStorage.multiGet([
        'accessToken',
        'user',
      ]);
      
      if (token[1] && userStr[1]) {
        const user = JSON.parse(userStr[1]);
        set({ user, isAuthenticated: true });
      }
    } catch (error) {
      console.error('Failed to load user:', error);
    } finally {
      set({ isLoading: false });
    }
  },

  updateUser: (userData) => {
    const currentUser = get().user;
    if (currentUser) {
      const updatedUser = { ...currentUser, ...userData };
      set({ user: updatedUser });
      AsyncStorage.setItem('user', JSON.stringify(updatedUser));
    }
  },
}));
```

### Step 4.3: Create Login Screen

**src/features/auth/screens/LoginScreen.tsx:**
```typescript
import React, { useState } from 'react';
import {
  View,
  StyleSheet,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
} from 'react-native';
import { Text, TextInput, HelperText } from 'react-native-paper';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button } from '@shared/components/ui/Button';
import { colors, spacing } from '@shared/theme';
import { useAuthStore } from '../store/authStore';
import { handleError } from '@shared/utils/errorHandler';
import { useNavigation } from '@react-navigation/native';

const loginSchema = z.object({
  email: z.string().email('Nieprawidłowy adres email'),
  password: z.string().min(6, 'Hasło musi mieć minimum 6 znaków'),
});

type LoginFormData = z.infer<typeof loginSchema>;

export const LoginScreen: React.FC = () => {
  const navigation = useNavigation();
  const [isLoading, setIsLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const signIn = useAuthStore((state) => state.signIn);

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: '',
      password: '',
    },
  });

  const onSubmit = async (data: LoginFormData) => {
    try {
      setIsLoading(true);
      await signIn(data);
      // Navigation is handled by auth state change
    } catch (error) {
      handleError(error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <KeyboardAvoidingView
      style={styles.container}
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
    >
      <ScrollView
        contentContainerStyle={styles.scrollContent}
        keyboardShouldPersistTaps="handled"
      >
        <View style={styles.headerContainer}>
          <Text style={styles.title}>SledzSpecke</Text>
          <Text style={styles.subtitle}>
            Śledź postęp swojej specjalizacji
          </Text>
        </View>

        <View style={styles.formContainer}>
          <Controller
            control={control}
            name="email"
            render={({ field: { onChange, onBlur, value } }) => (
              <View style={styles.inputContainer}>
                <TextInput
                  label="Email"
                  value={value}
                  onChangeText={onChange}
                  onBlur={onBlur}
                  keyboardType="email-address"
                  autoCapitalize="none"
                  error={!!errors.email}
                  disabled={isLoading}
                />
                <HelperText type="error" visible={!!errors.email}>
                  {errors.email?.message}
                </HelperText>
              </View>
            )}
          />

          <Controller
            control={control}
            name="password"
            render={({ field: { onChange, onBlur, value } }) => (
              <View style={styles.inputContainer}>
                <TextInput
                  label="Hasło"
                  value={value}
                  onChangeText={onChange}
                  onBlur={onBlur}
                  secureTextEntry={!showPassword}
                  error={!!errors.password}
                  disabled={isLoading}
                  right={
                    <TextInput.Icon
                      icon={showPassword ? 'eye-off' : 'eye'}
                      onPress={() => setShowPassword(!showPassword)}
                    />
                  }
                />
                <HelperText type="error" visible={!!errors.password}>
                  {errors.password?.message}
                </HelperText>
              </View>
            )}
          />

          <Button
            title="Zaloguj się"
            onPress={handleSubmit(onSubmit)}
            loading={isLoading}
            disabled={isLoading}
            style={styles.loginButton}
          />

          <Button
            title="Nie masz konta? Zarejestruj się"
            variant="outline"
            onPress={() => navigation.navigate('Register')}
            disabled={isLoading}
            style={styles.registerButton}
          />
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.background,
  },
  scrollContent: {
    flexGrow: 1,
    justifyContent: 'center',
    padding: spacing.lg,
  },
  headerContainer: {
    alignItems: 'center',
    marginBottom: spacing.xxl,
  },
  title: {
    fontSize: 32,
    fontWeight: '700',
    color: colors.primary,
    marginBottom: spacing.xs,
  },
  subtitle: {
    fontSize: 16,
    color: colors.text.secondary,
  },
  formContainer: {
    width: '100%',
    maxWidth: 400,
    alignSelf: 'center',
  },
  inputContainer: {
    marginBottom: spacing.sm,
  },
  loginButton: {
    marginTop: spacing.md,
  },
  registerButton: {
    marginTop: spacing.sm,
  },
});
```

---

## Phase 5: Dashboard Feature (Days 11-13)

### Step 5.1: Dashboard Types and Data

**src/features/dashboard/types/dashboard.types.ts:**
```typescript
export interface DashboardData {
  specialization: {
    id: number;
    name: string;
    startDate: string;
    plannedEndDate: string;
    smkVersion: 'old' | 'new';
  };
  currentModule: {
    id: number;
    name: string;
    type: 'basic' | 'specialist';
    startDate: string;
    endDate: string;
  };
  overallProgress: number;
  statistics: {
    internships: {
      completed: number;
      total: number;
      completedDays: number;
      requiredDays: number;
    };
    shifts: {
      completedHours: number;
      requiredHours: number;
    };
    procedures: {
      completedTypeA: number;
      requiredTypeA: number;
      completedTypeB: number;
      requiredTypeB: number;
    };
    courses: {
      completed: number;
      total: number;
    };
    selfEducation: {
      count: number;
    };
    publications: {
      count: number;
    };
  };
}

export interface ModuleProgress {
  moduleId: number;
  moduleName: string;
  progress: number;
  components: {
    internships: number;
    procedures: number;
    shifts: number;
    courses: number;
  };
}
```

### Step 5.2: Dashboard Screen Implementation

**src/features/dashboard/screens/DashboardScreen.tsx:**
```typescript
import React from 'react';
import { ScrollView, StyleSheet, View, RefreshControl } from 'react-native';
import { Text } from 'react-native-paper';
import { useNavigation } from '@react-navigation/native';
import { ScreenWrapper } from '@shared/components/layout/ScreenWrapper';
import { ModuleSwitch } from '../components/ModuleSwitch';
import { ModuleHeader } from '../components/ModuleHeader';
import { StatisticsGrid } from '../components/StatisticsGrid';
import { ActionButtons } from '../components/ActionButtons';
import { useDashboardData } from '../hooks/useDashboardData';
import { useAuthStore } from '@features/auth/store/authStore';
import { colors, spacing } from '@shared/theme';

export const DashboardScreen: React.FC = () => {
  const navigation = useNavigation();
  const user = useAuthStore((state) => state.user);
  const { data, isLoading, refetch, currentModule, switchModule } = 
    useDashboardData();

  const handleNavigate = (screen: string) => {
    navigation.navigate(screen as any);
  };

  return (
    <ScreenWrapper>
      <ScrollView
        showsVerticalScrollIndicator={false}
        refreshControl={
          <RefreshControl
            refreshing={isLoading}
            onRefresh={refetch}
            colors={[colors.primary]}
          />
        }
      >
        {/* Module Switch */}
        <ModuleSwitch
          currentModule={currentModule}
          onSwitch={switchModule}
          smkVersion={user?.smkVersion || 'new'}
        />

        {/* Module Header with Progress */}
        <ModuleHeader
          moduleName={data?.currentModule.name || ''}
          moduleType={data?.currentModule.type || 'basic'}
          specializationName={data?.specialization.name || ''}
          dateRange={`${data?.currentModule.startDate} - ${data?.currentModule.endDate}`}
          overallProgress={data?.overallProgress || 0}
        />

        {/* Statistics Grid */}
        <StatisticsGrid
          statistics={data?.statistics}
          onCardPress={handleNavigate}
        />

        {/* Action Buttons */}
        <ActionButtons
          onAbsencesPress={() => handleNavigate('Absences')}
          onExportPress={() => handleNavigate('Export')}
        />
      </ScrollView>
    </ScreenWrapper>
  );
};
```

### Step 5.3: Dashboard Components

**src/features/dashboard/components/StatisticsGrid.tsx:**
```typescript
import React from 'react';
import { View, StyleSheet } from 'react-native';
import { StatCard } from './StatCard';
import { spacing } from '@shared/theme';

interface StatisticsGridProps {
  statistics?: any; // Use proper type from dashboard.types.ts
  onCardPress: (screen: string) => void;
}

export const StatisticsGrid: React.FC<StatisticsGridProps> = ({
  statistics,
  onCardPress,
}) => {
  if (!statistics) return null;

  return (
    <View style={styles.container}>
      <View style={styles.row}>
        <StatCard
          title="Staże"
          value={`${statistics.internships.completed}/${statistics.internships.total}`}
          progress={statistics.internships.completed / statistics.internships.total}
          onPress={() => onCardPress('Internships')}
          style={styles.card}
        />
        <StatCard
          title="Dyżury"
          value={`${statistics.shifts.completedHours}/${statistics.shifts.requiredHours}h`}
          progress={statistics.shifts.completedHours / statistics.shifts.requiredHours}
          onPress={() => onCardPress('Shifts')}
          style={styles.card}
        />
      </View>

      <View style={styles.row}>
        <StatCard
          title="Procedury"
          value={`${statistics.procedures.completedTypeA + statistics.procedures.completedTypeB}/${
            statistics.procedures.requiredTypeA + statistics.procedures.requiredTypeB
          }`}
          progress={
            (statistics.procedures.completedTypeA + statistics.procedures.completedTypeB) /
            (statistics.procedures.requiredTypeA + statistics.procedures.requiredTypeB)
          }
          onPress={() => onCardPress('Procedures')}
          style={styles.card}
        />
        <StatCard
          title="Kursy"
          value={`${statistics.courses.completed}/${statistics.courses.total}`}
          progress={statistics.courses.completed / statistics.courses.total}
          onPress={() => onCardPress('Courses')}
          style={styles.card}
        />
      </View>

      <View style={styles.row}>
        <StatCard
          title="Samokształcenie"
          value={statistics.selfEducation.count.toString()}
          onPress={() => onCardPress('SelfEducation')}
          style={styles.card}
        />
        <StatCard
          title="Publikacje"
          value={statistics.publications.count.toString()}
          onPress={() => onCardPress('Publications')}
          style={styles.card}
        />
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: spacing.md,
    marginTop: spacing.md,
  },
  row: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: spacing.md,
  },
  card: {
    flex: 0.48,
  },
});
```

---

## Phase 6: Procedures Feature (Days 14-17)

### Step 6.1: Procedure Types

**src/features/procedures/types/procedure.types.ts:**
```typescript
export interface Procedure {
  id: string;
  internshipId: string;
  internshipName: string;
  date: string;
  year: number;
  code: string;
  name: string;
  location: string;
  operatorCode: 'A' | 'B'; // A - samostodzielnie, B - asysta
  performingPerson: string;
  patientInitials: string;
  patientGender: 'M' | 'K';
  moduleId: number;
  status: 'completed' | 'pending';
  syncStatus: 'notSynced' | 'synced' | 'modified';
}

export interface ProcedureTemplate {
  id: string;
  code: string;
  name: string;
  requiredCountA: number;
  requiredCountB: number;
  moduleId: number;
  specializationId: number;
}

export interface ProcedureFormData {
  date: Date;
  code: string;
  operatorCode: 'A' | 'B';
  performingPerson: string;
  location: string;
  internshipId: string;
  patientInitials: string;
  patientGender: 'M' | 'K';
}

export interface ProcedureStatistics {
  totalRequired: number;
  totalCompleted: number;
  requiredTypeA: number;
  completedTypeA: number;
  requiredTypeB: number;
  completedTypeB: number;
  byProcedure: Array<{
    code: string;
    name: string;
    requiredA: number;
    completedA: number;
    requiredB: number;
    completedB: number;
  }>;
}
```

### Step 6.2: Procedure List Screen

**src/features/procedures/screens/ProcedureListScreen.tsx:**
```typescript
import React, { useState, useMemo } from 'react';
import {
  FlatList,
  StyleSheet,
  View,
  SectionList,
  SectionListData,
} from 'react-native';
import {
  FAB,
  Searchbar,
  SegmentedButtons,
  Text,
  Divider,
} from 'react-native-paper';
import { useNavigation } from '@react-navigation/native';
import { ScreenWrapper } from '@shared/components/layout/ScreenWrapper';
import { ProcedureCard } from '../components/ProcedureCard';
import { ProcedureSummary } from '../components/ProcedureSummary';
import { ProcedureFilters } from '../components/ProcedureFilters';
import { useProcedures } from '../hooks/useProcedures';
import { useAuthStore } from '@features/auth/store/authStore';
import { colors, spacing } from '@shared/theme';
import { Procedure } from '../types/procedure.types';

export const ProcedureListScreen: React.FC = () => {
  const navigation = useNavigation();
  const user = useAuthStore((state) => state.user);
  const [searchQuery, setSearchQuery] = useState('');
  const [activeModule, setActiveModule] = useState<'basic' | 'specialist'>('basic');
  const [showFilters, setShowFilters] = useState(false);
  const [filters, setFilters] = useState({
    operatorCode: 'all',
    year: 'all',
    internshipId: 'all',
  });

  const { data: procedures = [], isLoading, refetch } = useProcedures({
    moduleType: activeModule,
    ...filters,
  });

  // Group procedures by template
  const groupedProcedures = useMemo(() => {
    const filtered = procedures.filter((proc) =>
      proc.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      proc.code.toLowerCase().includes(searchQuery.toLowerCase())
    );

    const groups = filtered.reduce((acc, proc) => {
      const key = `${proc.code} - ${proc.name}`;
      if (!acc[key]) {
        acc[key] = {
          title: key,
          requiredA: 3, // Get from template
          requiredB: 3,
          completedA: 0,
          completedB: 0,
          data: [],
        };
      }
      
      if (proc.operatorCode === 'A') {
        acc[key].completedA++;
      } else {
        acc[key].completedB++;
      }
      
      acc[key].data.push(proc);
      return acc;
    }, {} as Record<string, any>);

    return Object.values(groups);
  }, [procedures, searchQuery]);

  const renderSectionHeader = ({ section }: { section: any }) => (
    <View style={styles.sectionHeader}>
      <View style={styles.sectionTitleContainer}>
        <Text style={styles.sectionTitle}>{section.title}</Text>
        <View style={styles.sectionStats}>
          <Text style={styles.sectionStat}>
            {section.completedA}/{section.requiredA} (A)
          </Text>
          <Text style={styles.sectionStat}>
            {section.completedB}/{section.requiredB} (B)
          </Text>
        </View>
      </View>
    </View>
  );

  const renderProcedure = ({ item }: { item: Procedure }) => (
    <ProcedureCard
      procedure={item}
      onPress={() => navigation.navigate('ProcedureDetail', { id: item.id })}
    />
  );

  return (
    <ScreenWrapper>
      <View style={styles.header}>
        {/* Module Switch */}
        <SegmentedButtons
          value={activeModule}
          onValueChange={(value) => setActiveModule(value as any)}
          buttons={[
            { value: 'basic', label: 'Moduł podstawowy' },
            { value: 'specialist', label: 'Moduł specjalistyczny' },
          ]}
          style={styles.moduleSwitch}
        />

        {/* Search Bar */}
        <Searchbar
          placeholder="Szukaj procedury..."
          onChangeText={setSearchQuery}
          value={searchQuery}
          style={styles.searchbar}
          onIconPress={() => setShowFilters(!showFilters)}
          icon={showFilters ? 'filter-off' : 'filter'}
        />

        {/* Filters */}
        {showFilters && (
          <ProcedureFilters
            filters={filters}
            onFiltersChange={setFilters}
          />
        )}

        {/* Summary */}
        <ProcedureSummary
          procedures={procedures}
          moduleType={activeModule}
        />
      </View>

      {/* Procedures List */}
      <SectionList
        sections={groupedProcedures}
        keyExtractor={(item) => item.id}
        renderItem={renderProcedure}
        renderSectionHeader={renderSectionHeader}
        ItemSeparatorComponent={() => <Divider />}
        contentContainerStyle={styles.list}
        stickySectionHeadersEnabled={false}
      />

      {/* FAB */}
      <FAB
        icon="plus"
        style={styles.fab}
        onPress={() => navigation.navigate('AddProcedure')}
      />
    </ScreenWrapper>
  );
};

const styles = StyleSheet.create({
  header: {
    paddingHorizontal: spacing.md,
    paddingBottom: spacing.md,
    backgroundColor: colors.surface,
    elevation: 2,
  },
  moduleSwitch: {
    marginBottom: spacing.sm,
  },
  searchbar: {
    marginBottom: spacing.sm,
  },
  list: {
    paddingBottom: 100,
  },
  sectionHeader: {
    backgroundColor: colors.background,
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.sm,
  },
  sectionTitleContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  sectionTitle: {
    fontSize: 16,
    fontWeight: '600',
    color: colors.text.primary,
    flex: 1,
  },
  sectionStats: {
    flexDirection: 'row',
    gap: spacing.md,
  },
  sectionStat: {
    fontSize: 14,
    color: colors.text.secondary,
  },
  fab: {
    position: 'absolute',
    margin: spacing.lg,
    right: 0,
    bottom: 0,
    backgroundColor: colors.primary,
  },
});
```

### Step 6.3: Add Procedure Form

**src/features/procedures/screens/AddProcedureScreen.tsx:**
```typescript
import React, { useState } from 'react';
import {
  View,
  StyleSheet,
  ScrollView,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';
import { Text, TextInput, HelperText, RadioButton } from 'react-native-paper';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigation } from '@react-navigation/native';
import DatePicker from 'react-native-date-picker';
import { ScreenWrapper } from '@shared/components/layout/ScreenWrapper';
import { Button } from '@shared/components/ui/Button';
import { Select } from '@shared/components/ui/Select';
import { useInternships } from '@features/internships/hooks/useInternships';
import { useProcedureTemplates } from '../hooks/useProcedureTemplates';
import { useCreateProcedure } from '../hooks/useCreateProcedure';
import { colors, spacing } from '@shared/theme';
import { format } from 'date-fns';
import { pl } from 'date-fns/locale';

const procedureSchema = z.object({
  date: z.date(),
  internshipId: z.string().min(1, 'Wybierz staż'),
  procedureTemplateId: z.string().min(1, 'Wybierz procedurę'),
  operatorCode: z.enum(['A', 'B']),
  performingPerson: z.string().min(2, 'Podaj wykonującego'),
  location: z.string().min(2, 'Podaj miejsce wykonania'),
  patientInitials: z
    .string()
    .min(2, 'Minimum 2 znaki')
    .max(3, 'Maksimum 3 znaki')
    .regex(/^[A-Z]+$/, 'Tylko wielkie litery'),
  patientGender: z.enum(['M', 'K']),
});

type ProcedureFormData = z.infer<typeof procedureSchema>;

export const AddProcedureScreen: React.FC = () => {
  const navigation = useNavigation();
  const [showDatePicker, setShowDatePicker] = useState(false);
  const { data: internships = [] } = useInternships();
  const { data: templates = [] } = useProcedureTemplates();
  const { mutate: createProcedure, isLoading } = useCreateProcedure();

  const {
    control,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<ProcedureFormData>({
    resolver: zodResolver(procedureSchema),
    defaultValues: {
      date: new Date(),
      operatorCode: 'A',
      patientGender: 'M',
    },
  });

  const selectedDate = watch('date');

  const onSubmit = (data: ProcedureFormData) => {
    createProcedure(data, {
      onSuccess: () => {
        navigation.goBack();
      },
    });
  };

  return (
    <ScreenWrapper showBackButton title="Dodaj procedurę">
      <KeyboardAvoidingView
        style={styles.container}
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
      >
        <ScrollView
          contentContainerStyle={styles.scrollContent}
          keyboardShouldPersistTaps="handled"
        >
          {/* Date Selection */}
          <Controller
            control={control}
            name="date"
            render={({ field: { onChange, value } }) => (
              <View style={styles.inputContainer}>
                <Text style={styles.label}>Data wykonania *</Text>
                <Button
                  title={format(value, 'dd.MM.yyyy', { locale: pl })}
                  variant="outline"
                  onPress={() => setShowDatePicker(true)}
                  style={styles.dateButton}
                />
                <DatePicker
                  modal
                  open={showDatePicker}
                  date={value}
                  onConfirm={(date) => {
                    setShowDatePicker(false);
                    onChange(date);
                  }}
                  onCancel={() => setShowDatePicker(false)}
                  mode="date"
                  maximumDate={new Date()}
                  locale="pl"
                />
              </View>
            )}
          />

          {/* Internship Selection */}
          <Controller
            control={control}
            name="internshipId"
            render={({ field: { onChange, value } }) => (
              <View style={styles.inputContainer}>
                <Select
                  label="Staż *"
                  value={value}
                  onValueChange={onChange}
                  items={internships.map((i) => ({
                    label: i.name,
                    value: i.id,
                  }))}
                  error={!!errors.internshipId}
                  helperText={errors.internshipId?.message}
                />
              </View>
            )}
          />

          {/* Procedure Template Selection */}
          <Controller
            control={control}
            name="procedureTemplateId"
            render={({ field: { onChange, value } }) => (
              <View style={styles.inputContainer}>
                <Select
                  label="Procedura *"
                  value={value}
                  onValueChange={onChange}
                  items={templates.map((t) => ({
                    label: `${t.code} - ${t.name}`,
                    value: t.id,
                  }))}
                  error={!!errors.procedureTemplateId}
                  helperText={errors.procedureTemplateId?.message}
                />
              </View>
            )}
          />

          {/* Operator Code */}
          <Controller
            control={control}
            name="operatorCode"
            render={({ field: { onChange, value } }) => (
              <View style={styles.inputContainer}>
                <Text style={styles.label}>Kod zabiegu *</Text>
                <RadioButton.Group onValueChange={onChange} value={value}>
                  <View style={styles.radioRow}>
                    <RadioButton.Item
                      label="A - operator"
                      value="A"
                      position="leading"
                    />
                    <RadioButton.Item
                      label="B - asysta"
                      value="B"
                      position="leading"
                    />
                  </View>
                </RadioButton.Group>
              </View>
            )}
          />

          {/* Performing Person */}
          <Controller
            control={control}
            name="performingPerson"
            render={({ field: { onChange, onBlur, value } }) => (
              <View style={styles.inputContainer}>
                <TextInput
                  label="Osoba wykonująca *"
                  value={value}
                  onChangeText={onChange}
                  onBlur={onBlur}
                  error={!!errors.performingPerson}
                />
                <HelperText type="error" visible={!!errors.performingPerson}>
                  {errors.performingPerson?.message}
                </HelperText>
              </View>
            )}
          />

          {/* Location */}
          <Controller
            control={control}
            name="location"
            render={({ field: { onChange, onBlur, value } }) => (
              <View style={styles.inputContainer}>
                <TextInput
                  label="Miejsce wykonania *"
                  value={value}
                  onChangeText={onChange}
                  onBlur={onBlur}
                  error={!!errors.location}
                />
                <HelperText type="error" visible={!!errors.location}>
                  {errors.location?.message}
                </HelperText>
              </View>
            )}
          />

          {/* Patient Initials */}
          <Controller
            control={control}
            name="patientInitials"
            render={({ field: { onChange, onBlur, value } }) => (
              <View style={styles.inputContainer}>
                <TextInput
                  label="Inicjały pacjenta *"
                  value={value}
                  onChangeText={(text) => onChange(text.toUpperCase())}
                  onBlur={onBlur}
                  error={!!errors.patientInitials}
                  maxLength={3}
                  autoCapitalize="characters"
                />
                <HelperText type="error" visible={!!errors.patientInitials}>
                  {errors.patientInitials?.message}
                </HelperText>
              </View>
            )}
          />

          {/* Patient Gender */}
          <Controller
            control={control}
            name="patientGender"
            render={({ field: { onChange, value } }) => (
              <View style={styles.inputContainer}>
                <Text style={styles.label}>Płeć pacjenta *</Text>
                <RadioButton.Group onValueChange={onChange} value={value}>
                  <View style={styles.radioRow}>
                    <RadioButton.Item
                      label="K"
                      value="K"
                      position="leading"
                    />
                    <RadioButton.Item
                      label="M"
                      value="M"
                      position="leading"
                    />
                  </View>
                </RadioButton.Group>
              </View>
            )}
          />

          {/* Submit Buttons */}
          <View style={styles.buttonContainer}>
            <Button
              title="Anuluj"
              variant="outline"
              onPress={() => navigation.goBack()}
              style={styles.button}
            />
            <Button
              title="Zapisz"
              onPress={handleSubmit(onSubmit)}
              loading={isLoading}
              disabled={isLoading}
              style={styles.button}
            />
          </View>
        </ScrollView>
      </KeyboardAvoidingView>
    </ScreenWrapper>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  scrollContent: {
    padding: spacing.md,
    paddingBottom: spacing.xxl,
  },
  inputContainer: {
    marginBottom: spacing.md,
  },
  label: {
    fontSize: 16,
    fontWeight: '500',
    color: colors.text.primary,
    marginBottom: spacing.xs,
  },
  dateButton: {
    justifyContent: 'flex-start',
  },
  radioRow: {
    flexDirection: 'row',
    justifyContent: 'space-around',
  },
  buttonContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: spacing.lg,
    gap: spacing.md,
  },
  button: {
    flex: 1,
  },
});
```

---

## Phase 7: Medical Shifts Feature (Days 18-20)

### Step 7.1: Shift Types

**src/features/shifts/types/shift.types.ts:**
```typescript
export interface MedicalShift {
  id: string;
  internshipId: string;
  internshipName: string;
  date: string;
  hours: number;
  minutes: number;
  location: string;
  year: number;
  syncStatus: 'notSynced' | 'synced' | 'modified';
}

export interface ShiftStatistics {
  totalHours: number;
  requiredHours: number;
  byYear: Array<{
    year: number;
    hours: number;
    required: number;
  }>;
  byInternship: Array<{
    internshipId: string;
    internshipName: string;
    hours: number;
  }>;
}

export interface ShiftFormData {
  date: Date;
  hours: number;
  minutes: number;
  location: string;
  internshipId?: string;
}
```

### Step 7.2: Shift List Screen

**src/features/shifts/screens/ShiftListScreen.tsx:**
```typescript
import React, { useState } from 'react';
import { FlatList, StyleSheet, View } from 'react-native';
import { FAB, SegmentedButtons, Text, Chip