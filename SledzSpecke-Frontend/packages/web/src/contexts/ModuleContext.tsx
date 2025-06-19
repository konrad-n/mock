import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { useQuery } from '@tanstack/react-query';
import { apiClient } from '@/services/api';
import { useAuthStore } from '@/stores/authStore';

interface Module {
  id: number;
  name: string;
  type: 'Basic' | 'Specialist';
  specializationId: number;
}

interface ModuleContextType {
  currentModule: Module | null;
  modules: Module[];
  setCurrentModule: (module: Module) => void;
  isLoading: boolean;
}

const ModuleContext = createContext<ModuleContextType | undefined>(undefined);

export const useModule = () => {
  const context = useContext(ModuleContext);
  if (!context) {
    throw new Error('useModule must be used within a ModuleProvider');
  }
  return context;
};

interface ModuleProviderProps {
  children: ReactNode;
}

export const ModuleProvider: React.FC<ModuleProviderProps> = ({ children }) => {
  const user = useAuthStore(state => state.user);
  const [currentModule, setCurrentModule] = useState<Module | null>(null);

  // Fetch user's specialization and modules
  const { data: userSpecialization, isLoading } = useQuery({
    queryKey: ['user-specialization', user?.id],
    queryFn: async () => {
      if (!user?.id) return null;
      // Get user's specialization
      const response = await apiClient.get(`/api/users/${user.id}/specializations`);
      return response;
    },
    enabled: !!user?.id,
  });

  // Extract modules from specialization
  const modules: Module[] = userSpecialization?.modules || [];

  // Set initial module to basic module when data loads
  useEffect(() => {
    if (modules.length > 0 && !currentModule) {
      const basicModule = modules.find(m => m.type === 'Basic') || modules[0];
      setCurrentModule(basicModule);
    }
  }, [modules, currentModule]);

  return (
    <ModuleContext.Provider value={{
      currentModule,
      modules,
      setCurrentModule,
      isLoading
    }}>
      {children}
    </ModuleContext.Provider>
  );
};