export enum SmkVersion {
  Old = 'old',
  New = 'new'
}

export const getSmkVersionLabel = (version: SmkVersion): string => {
  return version === SmkVersion.Old ? 'Stary SMK' : 'Nowy SMK';
};