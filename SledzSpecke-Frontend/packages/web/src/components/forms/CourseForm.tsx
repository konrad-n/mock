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
  Grid,
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { pl } from 'date-fns/locale';
import { Course, CourseType } from '@shared/domain/entities';
import { useCreateCourse, useUpdateCourse } from '@/hooks/useCourses';

interface CourseFormProps {
  course?: Course | null;
  specializationId: number;
  onSuccess: () => void;
}

const courseSchema = z.object({
  courseName: z.string()
    .min(1, 'Nazwa kursu jest wymagana')
    .max(200, 'Nazwa kursu nie może być dłuższa niż 200 znaków'),
  courseType: z.nativeEnum(CourseType, {
    required_error: 'Typ kursu jest wymagany',
  }),
  startDate: z.date({
    required_error: 'Data rozpoczęcia jest wymagana',
  }),
  endDate: z.date({
    required_error: 'Data zakończenia jest wymagana',
  }),
  organizer: z.string()
    .min(1, 'Organizator jest wymagany')
    .max(200, 'Nazwa organizatora nie może być dłuższa niż 200 znaków'),
  location: z.string()
    .min(1, 'Miejsce jest wymagane')
    .max(200, 'Miejsce nie może być dłuższe niż 200 znaków'),
  certificateNumber: z.string()
    .max(100, 'Numer certyfikatu nie może być dłuższy niż 100 znaków')
    .optional(),
  creditHours: z.number()
    .min(0, 'Liczba punktów nie może być ujemna')
    .max(100, 'Liczba punktów nie może przekraczać 100')
    .optional(),
  notes: z.string()
    .max(500, 'Notatki nie mogą być dłuższe niż 500 znaków')
    .optional(),
}).refine((data) => data.endDate >= data.startDate, {
  message: 'Data zakończenia musi być późniejsza niż data rozpoczęcia',
  path: ['endDate'],
});

type CourseFormData = z.infer<typeof courseSchema>;

const courseTypeOptions = [
  { value: CourseType.Conference, label: 'Konferencja' },
  { value: CourseType.Workshop, label: 'Warsztaty' },
  { value: CourseType.Seminar, label: 'Seminarium' },
  { value: CourseType.OnlineCourse, label: 'Kurs online' },
  { value: CourseType.Other, label: 'Inne' },
];

export const CourseForm = ({ course, specializationId, onSuccess }: CourseFormProps) => {
  const createMutation = useCreateCourse();
  const updateMutation = useUpdateCourse();
  
  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CourseFormData>({
    resolver: zodResolver(courseSchema),
    defaultValues: {
      courseName: course?.courseName || '',
      courseType: course?.courseType || CourseType.Conference,
      startDate: course ? new Date(course.startDate) : new Date(),
      endDate: course ? new Date(course.endDate) : new Date(),
      organizer: course?.organizer || '',
      location: course?.location || '',
      certificateNumber: course?.certificateNumber || '',
      creditHours: course?.creditHours || 0,
      notes: course?.notes || '',
    },
  });

  const onSubmit = async (data: CourseFormData) => {
    try {
      const requestData = {
        specializationId,
        courseName: data.courseName,
        courseType: data.courseType,
        startDate: data.startDate.toISOString(),
        endDate: data.endDate.toISOString(),
        organizer: data.organizer,
        location: data.location,
        certificateNumber: data.certificateNumber,
        creditHours: data.creditHours,
        notes: data.notes,
      };
      
      if (course) {
        await updateMutation.mutateAsync({
          id: course.id,
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

          <TextField
            {...register('courseName')}
            label="Nazwa kursu"
            fullWidth
            error={!!errors.courseName}
            helperText={errors.courseName?.message}
            placeholder="np. Konferencja Kardiologiczna ESC 2024"
          />

          <Controller
            name="courseType"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                select
                label="Typ kursu"
                fullWidth
                error={!!errors.courseType}
                helperText={errors.courseType?.message}
              >
                {courseTypeOptions.map((option) => (
                  <MenuItem key={option.value} value={option.value}>
                    {option.label}
                  </MenuItem>
                ))}
              </TextField>
            )}
          />

          <Grid container spacing={2}>
            <Grid item xs={12} md={6}>
              <Controller
                name="startDate"
                control={control}
                render={({ field }) => (
                  <DatePicker
                    label="Data rozpoczęcia"
                    value={field.value}
                    onChange={field.onChange}
                    format="dd.MM.yyyy"
                    slotProps={{
                      textField: {
                        fullWidth: true,
                        error: !!errors.startDate,
                        helperText: errors.startDate?.message,
                      },
                    }}
                  />
                )}
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <Controller
                name="endDate"
                control={control}
                render={({ field }) => (
                  <DatePicker
                    label="Data zakończenia"
                    value={field.value}
                    onChange={field.onChange}
                    format="dd.MM.yyyy"
                    slotProps={{
                      textField: {
                        fullWidth: true,
                        error: !!errors.endDate,
                        helperText: errors.endDate?.message,
                      },
                    }}
                  />
                )}
              />
            </Grid>
          </Grid>

          <TextField
            {...register('organizer')}
            label="Organizator"
            fullWidth
            error={!!errors.organizer}
            helperText={errors.organizer?.message}
            placeholder="np. Polskie Towarzystwo Kardiologiczne"
          />

          <TextField
            {...register('location')}
            label="Miejsce"
            fullWidth
            error={!!errors.location}
            helperText={errors.location?.message}
            placeholder="np. Warszawa, Centrum Konferencyjne"
          />

          <Grid container spacing={2}>
            <Grid item xs={12} md={6}>
              <TextField
                {...register('certificateNumber')}
                label="Numer certyfikatu (opcjonalnie)"
                fullWidth
                error={!!errors.certificateNumber}
                helperText={errors.certificateNumber?.message}
                placeholder="np. PTK/2024/12345"
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <TextField
                {...register('creditHours', { valueAsNumber: true })}
                label="Punkty edukacyjne (opcjonalnie)"
                type="number"
                fullWidth
                error={!!errors.creditHours}
                helperText={errors.creditHours?.message}
                inputProps={{ min: 0, max: 100 }}
              />
            </Grid>
          </Grid>

          <TextField
            {...register('notes')}
            label="Notatki (opcjonalnie)"
            fullWidth
            multiline
            rows={3}
            error={!!errors.notes}
            helperText={errors.notes?.message}
            placeholder="np. główne tematy, wykładowcy, wrażenia"
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
              {course ? 'Zapisz zmiany' : 'Dodaj kurs'}
            </Button>
          </Stack>
        </Stack>
      </Box>
    </LocalizationProvider>
  );
};