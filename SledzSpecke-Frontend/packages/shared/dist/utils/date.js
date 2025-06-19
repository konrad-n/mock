import { format, parseISO, isValid } from 'date-fns';
import { pl } from 'date-fns/locale';
export const formatDate = (date, formatStr = 'dd.MM.yyyy') => {
    const parsedDate = typeof date === 'string' ? parseISO(date) : date;
    return isValid(parsedDate) ? format(parsedDate, formatStr, { locale: pl }) : '';
};
export const formatDateTime = (date) => {
    return formatDate(date, 'dd.MM.yyyy HH:mm');
};
export const formatDateLong = (date) => {
    return formatDate(date, 'dd MMMM yyyy');
};
//# sourceMappingURL=date.js.map