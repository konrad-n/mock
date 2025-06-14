export enum ModuleType {
  Basic = 'Basic',
  Specialist = 'Specialist'
}

export const getModuleLabel = (type: ModuleType): string => {
  return type === ModuleType.Basic ? 'Moduł Podstawowy' : 'Moduł Specjalistyczny';
};