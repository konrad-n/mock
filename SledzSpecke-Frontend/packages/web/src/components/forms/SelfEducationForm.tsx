import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import {
  Box,
  TextField,
  Button,
  Stack,
  CircularProgress,
  Alert,
  MenuItem,
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { pl } from 'date-fns/locale';
import { SelfEducation, SelfEducationType } from '@shared/domain/entities';
import { useCreateSelfEducation, useUpdateSelfEducation } from '@/hooks/useCourses';

interface SelfEducationFormProps {
  activity?: SelfEducation | null;
  specializationId: number;
  onSuccess: () => void;
}

const selfEducationSchema = z.object({
  activityType: z.nativeEnum(SelfEducationType, {
    required_error: 'Typ aktywności jest wymagany',
  }),
  title: z.string()
    .min(1, 'Tytuł jest wymagany')
    .max(200, 'Tytuł nie może być dłuższy niż 200 znaków'),
  description: z.string()
    .min(1, 'Opis jest wymagany')
    .max(500, 'Opis nie może być dłuższy niż 500 znaków'),
  date: z.date({
    required_error: 'Data jest wymagana',
  }),
  creditHours: z.number()
    .min(1, 'Liczba punktów musi być większa od 0')
    .max(20, 'Liczba punktów nie może przekraczać 20'),
  notes: z.string()
    .max(500, 'Notatki nie mogą być dłuższe niż 500 znaków')
    .optional(),
});

type SelfEducationFormData = z.infer<typeof selfEducationSchema>;

const activityTypeOptions = [
  { value: SelfEducationType.BookReading, label: 'Lektura książki' },
  { value: SelfEducationType.ArticleReview, label: 'Przegląd artykułów' },
  { value: SelfEducationType.OnlineLecture, label: 'Wykład online' },
  { value: SelfEducationType.CaseStudy, label: 'Analiza przypadku' },
  { value: SelfEducationType.Research, label: 'Badania własne' },
  { value: SelfEducationType.Other, label: 'Inne' },
];

// Suggested titles based on activity type
const suggestedTitles: Record<SelfEducationType, string[]> = {
  [SelfEducationType.BookReading]: [
    'Podręcznik kardiologii klinicznej',
    'Elektrokardiografia praktyczna',
    'Farmakoterapia w kardiologii',
  ],
  [SelfEducationType.ArticleReview]: [
    'Przegląd wytycznych ESC',
    'Analiza najnowszych badań klinicznych',
    'Przegląd literatury dot. niewydolności serca',
  ],
  [SelfEducationType.OnlineLecture]: [
    'Webinarium: Postępy w kardiologii interwencyjnej',
    'Kurs online: Echokardiografia',
    'Wykład: Nowe leki w kardiologii',
  ],
  [SelfEducationType.CaseStudy]: [
    'Analiza przypadku: Ostry zespół wieńcowy',
    'Przypadek kliniczny: Kardiomiopatia',
    'Analiza: Powikłania po zawale',
  ],
  [SelfEducationType.Research]: [
    'Badanie własne: Analiza wyników leczenia',
    'Projekt badawczy: Czynniki ryzyka',
    'Meta-analiza publikacji',
  ],
  [SelfEducationType.Other]: [],
};

export const SelfEducationForm = ({ activity, specializationId, onSuccess }: SelfEducationFormProps) => {
  const createMutation = useCreateSelfEducation();
  const updateMutation = useUpdateSelfEducation();
  
  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<SelfEducationFormData>({
    resolver: zodResolver(selfEducationSchema),
    defaultValues: {
      activityType: activity?.activityType || SelfEducationType.ArticleReview,
      title: activity?.title || '',
      description: activity?.description || '',
      date: activity ? new Date(activity.date) : new Date(),
      creditHours: activity?.creditHours || 2,
      notes: activity?.notes || '',
    },
  });

  const selectedActivityType = watch('activityType');

  const onSubmit = async (data: SelfEducationFormData) => {
    try {
      const requestData = {
        specializationId,
        activityType: data.activityType,
        title: data.title,
        description: data.description,
        date: data.date.toISOString(),
        creditHours: data.creditHours,
        notes: data.notes,
      };
      
      if (activity) {
        await updateMutation.mutateAsync({
          id: activity.id,
          data: requestData,
        });
      } else {
        await createMutation.mutateAsync(requestData);
      }
      
      onSuccess();
    } catch (error) {
      // Error is handled by mutations
    }
  };

  const isLoading = createMutation.isPending || updateMutation.isPending;
  const error = createMutation.error || updateMutation.error;

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={pl}>
      <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ mt: 2 }}>
        <Stack spacing={3}>
          {error && (
            <Alert severity="error">
              {error instanceof Error ? error.message : 'Wystąpił błąd'}
            </Alert>
          )}

          <Controller
            name="activityType"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                select
                label="Typ aktywności"
                fullWidth
                error={!!errors.activityType}
                helperText={errors.activityType?.message}
              >
                {activityTypeOptions.map((option) => (
                  <MenuItem key={option.value} value={option.value}>
                    {option.label}
                  </MenuItem>
                ))}
              </TextField>
            )}
          />

          <Controller
            name="title"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                label="Tytuł"
                fullWidth
                error={!!errors.title}
                helperText={errors.title?.message || 
                  (suggestedTitles[selectedActivityType]?.length > 0 && 
                    `Sugestie: ${suggestedTitles[selectedActivityType].join(', ')}`)}
                placeholder="np. Przegląd najnowszych wytycznych ESC"
              />
            )}
          />

          <TextField
            {...register('description')}
            label="Opis"
            fullWidth
            multiline
            rows={3}
            error={!!errors.description}
            helperText={errors.description?.message}
            placeholder="Opisz czego dotyczyła aktywność, jakie były główne wnioski"
          />

          <Controller
            name="date"
            control={control}
            render={({ field }) => (
              <DatePicker
                label="Data"
                value={field.value}
                onChange={field.onChange}
                format="dd.MM.yyyy"
                maxDate={new Date()}
                slotProps={{
                  textField: {
                    fullWidth: true,
                    error: !!errors.date,
                    helperText: errors.date?.message,
                  },
                }}
              />
            )}
          />

          <TextField
            {...register('creditHours', { valueAsNumber: true })}
            label="Liczba punktów edukacyjnych"
            type="number"
            fullWidth
            error={!!errors.creditHours}
            helperText={errors.creditHours?.message}
            inputProps={{ min: 1, max: 20, step: 0.5 }}
          />

          <TextField
            {...register('notes')}
            label="Notatki (opcjonalnie)"
            fullWidth
            multiline
            rows={2}
            error={!!errors.notes}
            helperText={errors.notes?.message}
            placeholder="Dodatkowe uwagi, źródła, linki"
          />

          <Stack direction="row" spacing={2} justifyContent="flex-end">
            <Button onClick={onSuccess} disabled={isLoading}>
              Anuluj
            </Button>
            <Button
              type="submit"
              variant="contained"
              disabled={isLoading}
              startIcon={isLoading && <CircularProgress size={20} />}
            >
              {activity ? 'Zapisz zmiany' : 'Dodaj aktywność'}
            </Button>
          </Stack>
        </Stack>
      </Box>
    </LocalizationProvider>
  );
};