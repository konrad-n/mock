import { format, parseISO, isValid } from 'date-fns';
import { pl } from 'date-fns/locale';

export const formatDate = (date: string | Date, formatStr: string = 'dd.MM.yyyy'): string => {
  const parsedDate = typeof date === 'string' ? parseISO(date) : date;
  return isValid(parsedDate) ? format(parsedDate, formatStr, { locale: pl }) : '';
};

export const formatDateTime = (date: string | Date): string => {
  return formatDate(date, 'dd.MM.yyyy HH:mm');
};

export const formatDateLong = (date: string | Date): string => {
  return formatDate(date, 'dd MMMM yyyy');
};