import { useEffect } from 'react';
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
import { Internship } from '@shared/domain/entities';
import { useCreateInternship, useUpdateInternship, useInternshipModules } from '@/hooks/useInternships';
import { addDays } from 'date-fns';

interface InternshipFormProps {
  internship?: Internship | null;
  specializationId: number;
  onSuccess: () => void;
}

const internshipSchema = z.object({
  moduleId: z.number({
    required_error: 'Moduł jest wymagany',
  }),
  institutionName: z.string()
    .min(1, 'Nazwa instytucji jest wymagana')
    .max(200, 'Nazwa instytucji nie może być dłuższa niż 200 znaków'),
  departmentName: z.string()
    .min(1, 'Nazwa oddziału jest wymagana')
    .max(200, 'Nazwa oddziału nie może być dłuższa niż 200 znaków'),
  supervisorName: z.string()
    .min(1, 'Nazwa kierownika jest wymagana')
    .max(100, 'Nazwa kierownika nie może być dłuższa niż 100 znaków'),
  startDate: z.date({
    required_error: 'Data rozpoczęcia jest wymagana',
  }),
  endDate: z.date({
    required_error: 'Data zakończenia jest wymagana',
  }),
  completedDays: z.number()
    .min(0, 'Liczba dni nie może być ujemna')
    .max(365, 'Liczba dni nie może przekraczać 365'),
}).refine((data) => data.endDate >= data.startDate, {
  message: 'Data zakończenia musi być późniejsza niż data rozpoczęcia',
  path: ['endDate'],
});

type InternshipFormData = z.infer<typeof internshipSchema>;

export const InternshipForm = ({ internship, specializationId, onSuccess }: InternshipFormProps) => {
  const createMutation = useCreateInternship();
  const updateMutation = useUpdateInternship();
  const { data: modules = [] } = useInternshipModules(specializationId);
  
  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    watch,
    setValue,
  } = useForm<InternshipFormData>({
    resolver: zodResolver(internshipSchema),
    defaultValues: {
      moduleId: internship?.moduleId || 0,
      institutionName: internship?.institutionName || '',
      departmentName: internship?.departmentName || '',
      supervisorName: internship?.supervisorName || '',
      startDate: internship ? new Date(internship.startDate) : new Date(),
      endDate: internship ? new Date(internship.endDate) : addDays(new Date(), 180),
      completedDays: internship?.completedDays || 0,
    },
  });

  const selectedModuleId = watch('moduleId');

  // Auto-fill department name when module is selected
  useEffect(() => {
    if (selectedModuleId && !internship) {
      const module = modules.find(m => m.id === selectedModuleId);
      if (module) {
        setValue('departmentName', module.name);
      }
    }
  }, [selectedModuleId, modules, setValue, internship]);

  const onSubmit = async (data: InternshipFormData) => {
    try {
      const requestData = {
        specializationId,
        moduleId: data.moduleId,
        institutionName: data.institutionName,
        departmentName: data.departmentName,
        supervisorName: data.supervisorName,
        startDate: data.startDate.toISOString(),
        endDate: data.endDate.toISOString(),
      };
      
      if (internship) {
        await updateMutation.mutateAsync({
          id: internship.id,
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
            name="moduleId"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                select
                label="Moduł stażu"
                fullWidth
                error={!!errors.moduleId}
                helperText={errors.moduleId?.message}
                disabled={!!internship} // Can't change module after creation
              >
                <MenuItem value={0} disabled>
                  Wybierz moduł
                </MenuItem>
                {modules.map((module) => (
                  <MenuItem key={module.id} value={module.id}>
                    {module.name} ({module.requiredDays} dni)
                  </MenuItem>
                ))}
              </TextField>
            )}
          />

          <TextField
            {...register('institutionName')}
            label="Nazwa instytucji"
            fullWidth
            error={!!errors.institutionName}
            helperText={errors.institutionName?.message}
            placeholder="np. Szpital Uniwersytecki"
          />

          <TextField
            {...register('departmentName')}
            label="Nazwa oddziału"
            fullWidth
            error={!!errors.departmentName}
            helperText={errors.departmentName?.message}
            placeholder="np. Oddział Kardiologii Inwazyjnej"
          />

          <TextField
            {...register('supervisorName')}
            label="Kierownik stażu"
            fullWidth
            error={!!errors.supervisorName}
            helperText={errors.supervisorName?.message}
            placeholder="np. dr hab. n. med. Jan Kowalski"
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

          {internship && (
            <TextField
              {...register('completedDays', { valueAsNumber: true })}
              label="Zrealizowane dni"
              type="number"
              fullWidth
              error={!!errors.completedDays}
              helperText={errors.completedDays?.message}
              inputProps={{ min: 0, max: 365 }}
            />
          )}

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
              {internship ? 'Zapisz zmiany' : 'Dodaj staż'}
            </Button>
          </Stack>
        </Stack>
      </Box>
    </LocalizationProvider>
  );
};