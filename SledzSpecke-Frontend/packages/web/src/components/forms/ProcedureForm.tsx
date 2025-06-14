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
import { Procedure } from '@shared/domain/entities';
import { OperatorLevel } from '@shared/domain/value-objects';
import { useCreateProcedure, useUpdateProcedure } from '@/hooks/useProcedures';

interface ProcedureFormProps {
  procedure?: Procedure | null;
  internshipId: number;
  onSuccess: () => void;
}

const procedureSchema = z.object({
  procedureCode: z.string()
    .min(1, 'Kod procedury jest wymagany')
    .max(20, 'Kod procedury nie może być dłuższy niż 20 znaków'),
  procedureName: z.string()
    .min(1, 'Nazwa procedury jest wymagana')
    .max(200, 'Nazwa procedury nie może być dłuższa niż 200 znaków'),
  performanceDate: z.date({
    required_error: 'Data wykonania jest wymagana',
  }),
  operatorLevel: z.nativeEnum(OperatorLevel, {
    required_error: 'Rola jest wymagana',
  }),
  location: z.string()
    .min(1, 'Miejsce wykonania jest wymagane')
    .max(200, 'Miejsce wykonania nie może być dłuższe niż 200 znaków'),
  patientAge: z.number()
    .min(0, 'Wiek pacjenta nie może być ujemny')
    .max(150, 'Wiek pacjenta nie może przekraczać 150 lat'),
  patientSex: z.enum(['M', 'K'], {
    required_error: 'Płeć pacjenta jest wymagana',
  }),
  icdCode: z.string()
    .max(10, 'Kod ICD nie może być dłuższy niż 10 znaków')
    .optional(),
  additionalInfo: z.string()
    .max(500, 'Dodatkowe informacje nie mogą być dłuższe niż 500 znaków')
    .optional(),
});

type ProcedureFormData = z.infer<typeof procedureSchema>;

export const ProcedureForm = ({ procedure, internshipId, onSuccess }: ProcedureFormProps) => {
  const createMutation = useCreateProcedure();
  const updateMutation = useUpdateProcedure();
  
  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    setValue,
  } = useForm<ProcedureFormData>({
    resolver: zodResolver(procedureSchema),
    defaultValues: {
      procedureCode: procedure?.procedureCode || '',
      procedureName: procedure?.procedureName || '',
      performanceDate: procedure ? new Date(procedure.performanceDate) : new Date(),
      operatorLevel: procedure?.operatorLevel || OperatorLevel.Operator,
      location: procedure?.location || '',
      patientAge: procedure?.patientAge || 0,
      patientSex: procedure?.patientSex || 'M',
      icdCode: procedure?.icdCode || '',
      additionalInfo: procedure?.additionalInfo || '',
    },
  });

  const onSubmit = async (data: ProcedureFormData) => {
    try {
      const requestData = {
        procedureCode: data.procedureCode,
        procedureName: data.procedureName,
        performanceDate: data.performanceDate.toISOString(),
        operatorLevel: data.operatorLevel,
        location: data.location,
        patientAge: data.patientAge,
        patientSex: data.patientSex,
        icdCode: data.icdCode,
        additionalInfo: data.additionalInfo,
      };
      
      if (procedure) {
        await updateMutation.mutateAsync({
          id: procedure.id,
          data: requestData,
        });
      } else {
        await createMutation.mutateAsync({
          internshipId,
          ...requestData,
        });
      }
      
      onSuccess();
    } catch (error) {
      // Error is handled by mutations
    }
  };

  const isLoading = createMutation.isPending || updateMutation.isPending;
  const error = createMutation.error || updateMutation.error;

  // Common procedure codes for cardiology (example)
  const commonProcedures = [
    { code: '89.52', name: 'Koronarografia' },
    { code: '36.01', name: 'PTCA (angioplastyka wieńcowa)' },
    { code: '37.22', name: 'Cewnikowanie serca' },
    { code: '37.21', name: 'Cewnikowanie prawego serca' },
    { code: '88.72', name: 'Echokardiografia' },
    { code: '89.43', name: 'Test wysiłkowy EKG' },
    { code: '99.62', name: 'Kardiowersja elektryczna' },
  ];

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={pl}>
      <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ mt: 2 }}>
        <Stack spacing={3}>
          {error && (
            <Alert severity="error">
              {error instanceof Error ? error.message : 'Wystąpił błąd'}
            </Alert>
          )}

          <Grid container spacing={2}>
            <Grid item xs={12} md={4}>
              <Controller
                name="procedureCode"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    select
                    label="Kod procedury"
                    fullWidth
                    error={!!errors.procedureCode}
                    helperText={errors.procedureCode?.message}
                    onChange={(e) => {
                      field.onChange(e);
                      const selected = commonProcedures.find(p => p.code === e.target.value);
                      if (selected) {
                        setValue('procedureName', selected.name);
                      }
                    }}
                  >
                    {commonProcedures.map((proc) => (
                      <MenuItem key={proc.code} value={proc.code}>
                        {proc.code} - {proc.name}
                      </MenuItem>
                    ))}
                    <MenuItem value="other">Inny...</MenuItem>
                  </TextField>
                )}
              />
            </Grid>

            <Grid item xs={12} md={8}>
              <TextField
                {...register('procedureName')}
                label="Nazwa procedury"
                fullWidth
                error={!!errors.procedureName}
                helperText={errors.procedureName?.message}
              />
            </Grid>
          </Grid>

          <Controller
            name="performanceDate"
            control={control}
            render={({ field }) => (
              <DatePicker
                label="Data wykonania"
                value={field.value}
                onChange={field.onChange}
                format="dd.MM.yyyy"
                slotProps={{
                  textField: {
                    fullWidth: true,
                    error: !!errors.performanceDate,
                    helperText: errors.performanceDate?.message,
                  },
                }}
              />
            )}
          />

          <Controller
            name="operatorLevel"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                select
                label="Rola w procedurze"
                fullWidth
                error={!!errors.operatorLevel}
                helperText={errors.operatorLevel?.message}
              >
                <MenuItem value={OperatorLevel.Operator}>Operator</MenuItem>
                <MenuItem value={OperatorLevel.FirstAssistant}>Pierwsza asysta</MenuItem>
                <MenuItem value={OperatorLevel.SecondAssistant}>Druga asysta</MenuItem>
              </TextField>
            )}
          />

          <TextField
            {...register('location')}
            label="Miejsce wykonania"
            fullWidth
            error={!!errors.location}
            helperText={errors.location?.message}
            placeholder="np. Pracownia Hemodynamiki, Oddział Kardiologii"
          />

          <Grid container spacing={2}>
            <Grid item xs={6}>
              <TextField
                {...register('patientAge', { valueAsNumber: true })}
                label="Wiek pacjenta"
                type="number"
                fullWidth
                error={!!errors.patientAge}
                helperText={errors.patientAge?.message}
                inputProps={{ min: 0, max: 150 }}
              />
            </Grid>

            <Grid item xs={6}>
              <Controller
                name="patientSex"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    select
                    label="Płeć pacjenta"
                    fullWidth
                    error={!!errors.patientSex}
                    helperText={errors.patientSex?.message}
                  >
                    <MenuItem value="M">Mężczyzna</MenuItem>
                    <MenuItem value="K">Kobieta</MenuItem>
                  </TextField>
                )}
              />
            </Grid>
          </Grid>

          <TextField
            {...register('icdCode')}
            label="Kod ICD-10 (opcjonalnie)"
            fullWidth
            error={!!errors.icdCode}
            helperText={errors.icdCode?.message}
            placeholder="np. I25.1"
          />

          <TextField
            {...register('additionalInfo')}
            label="Dodatkowe informacje (opcjonalnie)"
            fullWidth
            multiline
            rows={3}
            error={!!errors.additionalInfo}
            helperText={errors.additionalInfo?.message}
            placeholder="np. wskazania, powikłania, szczególne warunki"
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
              {procedure ? 'Zapisz zmiany' : 'Dodaj procedurę'}
            </Button>
          </Stack>
        </Stack>
      </Box>
    </LocalizationProvider>
  );
};