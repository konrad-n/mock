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
  FormControl,
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { pl } from 'date-fns/locale';
import { MedicalShift } from '@shared/domain/entities';
import { Duration } from '@shared/domain/value-objects';
import { useCreateMedicalShift, useUpdateMedicalShift } from '@/hooks/useMedicalShifts';

interface MedicalShiftFormProps {
  shift?: MedicalShift | null;
  internshipId: number;
  onSuccess: () => void;
}

const medicalShiftSchema = z.object({
  date: z.date({
    required_error: 'Data jest wymagana',
  }),
  durationHours: z.number()
    .min(0, 'Godziny nie mogą być ujemne')
    .max(24, 'Godziny nie mogą przekraczać 24'),
  durationMinutes: z.number()
    .min(0, 'Minuty nie mogą być ujemne')
    .max(59, 'Minuty muszą być między 0 a 59'),
  location: z.string()
    .min(1, 'Miejsce jest wymagane')
    .max(200, 'Miejsce nie może być dłuższe niż 200 znaków'),
  year: z.number()
    .min(1, 'Rok szkolenia jest wymagany')
    .max(6, 'Rok szkolenia musi być między 1 a 6'),
});

type MedicalShiftFormData = z.infer<typeof medicalShiftSchema>;

export const MedicalShiftForm = ({ shift, internshipId, onSuccess }: MedicalShiftFormProps) => {
  const createMutation = useCreateMedicalShift();
  const updateMutation = useUpdateMedicalShift();
  
  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<MedicalShiftFormData>({
    resolver: zodResolver(medicalShiftSchema),
    defaultValues: {
      date: shift ? new Date(shift.date) : new Date(),
      durationHours: shift ? Math.floor(shift.duration.toTotalMinutes() / 60) : 0,
      durationMinutes: shift ? shift.duration.toTotalMinutes() % 60 : 0,
      location: shift?.location || '',
      year: shift?.year || 1,
    },
  });

  const onSubmit = async (data: MedicalShiftFormData) => {
    try {
      const duration = Duration.fromMinutes(data.durationHours * 60 + data.durationMinutes);
      
      if (shift) {
        await updateMutation.mutateAsync({
          id: shift.id,
          data: {
            date: data.date.toISOString(),
            hours: duration.hours,
            minutes: duration.minutes,
            location: data.location,
            year: data.year,
          },
        });
      } else {
        await createMutation.mutateAsync({
          internshipId,
          date: data.date.toISOString(),
          hours: duration.hours,
          minutes: duration.minutes,
          location: data.location,
          year: data.year,
        });
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
            name="date"
            control={control}
            render={({ field }) => (
              <DatePicker
                label="Data dyżuru"
                value={field.value}
                onChange={field.onChange}
                format="dd.MM.yyyy"
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

          <Stack direction="row" spacing={2}>
            <TextField
              {...register('durationHours', { valueAsNumber: true })}
              label="Godziny"
              type="number"
              fullWidth
              error={!!errors.durationHours}
              helperText={errors.durationHours?.message}
              inputProps={{ min: 0, max: 24 }}
            />
            
            <TextField
              {...register('durationMinutes', { valueAsNumber: true })}
              label="Minuty"
              type="number"
              fullWidth
              error={!!errors.durationMinutes}
              helperText={errors.durationMinutes?.message}
              inputProps={{ min: 0, max: 59 }}
            />
          </Stack>

          <TextField
            {...register('location')}
            label="Miejsce"
            fullWidth
            error={!!errors.location}
            helperText={errors.location?.message}
            placeholder="np. Szpital Uniwersytecki, Oddział Intensywnej Terapii"
          />

          <FormControl fullWidth error={!!errors.year}>
            <TextField
              {...register('year', { valueAsNumber: true })}
              label="Rok szkolenia"
              select
              fullWidth
              error={!!errors.year}
              helperText={errors.year?.message}
            >
              {[1, 2, 3, 4, 5, 6].map((year) => (
                <MenuItem key={year} value={year}>
                  {year} rok
                </MenuItem>
              ))}
            </TextField>
          </FormControl>

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
              {shift ? 'Zapisz zmiany' : 'Dodaj dyżur'}
            </Button>
          </Stack>
        </Stack>
      </Box>
    </LocalizationProvider>
  );
};