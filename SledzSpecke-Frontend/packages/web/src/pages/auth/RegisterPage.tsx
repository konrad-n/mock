import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Box,
  TextField,
  Button,
  Typography,
  Alert,
  CircularProgress,
  InputAdornment,
  IconButton,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  FormHelperText
} from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useMutation, useQuery } from '@tanstack/react-query';
import { signUpSchema } from '@shared/utils/validation';
import { SignUpRequest } from '@shared/types';
import { SmkVersion } from '@shared/domain/value-objects';
import { apiClient } from '@/services/api';
import { useAuthStore } from '@/stores/authStore';

interface FormData extends SignUpRequest {
  confirmPassword: string;
  specializationTemplateId: number;
}

export const RegisterPage = () => {
  const navigate = useNavigate();
  const setAuth = useAuthStore((state) => state.setAuth);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  
  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<FormData>({
    resolver: zodResolver(signUpSchema),
    defaultValues: {
      smkVersion: 'new',
      specializationTemplateId: 1
    }
  });

  // Fetch available specializations
  const { data: specializations = [] } = useQuery({
    queryKey: ['specialization-templates'],
    queryFn: async () => {
      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/specialization-templates/public`);
      if (!response.ok) throw new Error('Failed to fetch specializations');
      return response.json();
    }
  });

  const registerMutation = useMutation({
    mutationFn: (data: SignUpRequest) => apiClient.signUp(data),
    onSuccess: (response) => {
      // Extract user data from JWT claims
      const email = response.Claims?.email?.[0] || '';
      const fullName = response.Claims?.full_name?.[0] || response.Claims?.name?.[0] || '';
      
      // Create user object from response
      const user = {
        id: response.UserId,
        email: email,
        fullName: fullName,
        smkVersion: SmkVersion.New,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      };
      
      setAuth(user, response.AccessToken);
      navigate('/dashboard');
    }
  });

  const onSubmit = (data: FormData) => {
    const { confirmPassword, specializationTemplateId, ...signUpData } = data;
    registerMutation.mutate({
      ...signUpData,
      specializationTemplateId
    } as any);
  };

  return (
    <Box>
      <Box sx={{ mb: 4, textAlign: 'center' }}>
        <Box sx={{ mb: 2 }}>
          <img 
            src="/logo.png" 
            alt="SledzSpecke Logo" 
            style={{ 
              width: 100, 
              height: 100,
              objectFit: 'contain'
            }}
          />
        </Box>
        <Typography variant="h4" component="h1" gutterBottom color="primary">
          Rejestracja
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Załóż konto w systemie śledzenia specjalizacji
        </Typography>
      </Box>

      {registerMutation.isError && (
        <Alert severity="error" sx={{ mb: 3 }}>
          Wystąpił błąd podczas rejestracji. Spróbuj ponownie.
        </Alert>
      )}

      <form onSubmit={handleSubmit(onSubmit)}>
        <TextField
          {...register('fullName')}
          label="Imię i nazwisko"
          fullWidth
          margin="normal"
          error={!!errors.fullName}
          helperText={errors.fullName?.message}
          autoComplete="name"
          autoFocus
        />

        <TextField
          {...register('email')}
          label="Adres email"
          type="email"
          fullWidth
          margin="normal"
          error={!!errors.email}
          helperText={errors.email?.message}
          autoComplete="email"
        />

        <FormControl fullWidth margin="normal" error={!!errors.smkVersion}>
          <InputLabel>Wersja SMK</InputLabel>
          <Select
            {...register('smkVersion')}
            label="Wersja SMK"
            defaultValue="new"
          >
            <MenuItem value="new">Nowy SMK</MenuItem>
            <MenuItem value="old">Stary SMK</MenuItem>
          </Select>
          {errors.smkVersion && (
            <FormHelperText>{errors.smkVersion.message}</FormHelperText>
          )}
        </FormControl>

        <FormControl fullWidth margin="normal" error={!!errors.specializationTemplateId}>
          <InputLabel>Specjalizacja</InputLabel>
          <Select
            {...register('specializationTemplateId', { valueAsNumber: true })}
            label="Specjalizacja"
            defaultValue={1}
          >
            {specializations.length === 0 ? (
              <MenuItem value={1}>Kardiologia</MenuItem>
            ) : (
              specializations.map((spec: any) => (
                <MenuItem key={spec.id} value={spec.id}>
                  {spec.name}
                </MenuItem>
              ))
            )}
          </Select>
          {errors.specializationTemplateId && (
            <FormHelperText>{errors.specializationTemplateId?.message}</FormHelperText>
          )}
        </FormControl>

        <TextField
          {...register('password')}
          label="Hasło"
          type={showPassword ? 'text' : 'password'}
          fullWidth
          margin="normal"
          error={!!errors.password}
          helperText={errors.password?.message}
          autoComplete="new-password"
          InputProps={{
            endAdornment: (
              <InputAdornment position="end">
                <IconButton
                  onClick={() => setShowPassword(!showPassword)}
                  edge="end"
                >
                  {showPassword ? <VisibilityOff /> : <Visibility />}
                </IconButton>
              </InputAdornment>
            )
          }}
        />

        <TextField
          {...register('confirmPassword')}
          label="Potwierdź hasło"
          type={showConfirmPassword ? 'text' : 'password'}
          fullWidth
          margin="normal"
          error={!!errors.confirmPassword}
          helperText={errors.confirmPassword?.message}
          autoComplete="new-password"
          InputProps={{
            endAdornment: (
              <InputAdornment position="end">
                <IconButton
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  edge="end"
                >
                  {showConfirmPassword ? <VisibilityOff /> : <Visibility />}
                </IconButton>
              </InputAdornment>
            )
          }}
        />

        <Button
          type="submit"
          fullWidth
          variant="contained"
          size="large"
          sx={{ mt: 3, mb: 2 }}
          disabled={registerMutation.isPending}
        >
          {registerMutation.isPending ? (
            <CircularProgress size={24} color="inherit" />
          ) : (
            'Zarejestruj się'
          )}
        </Button>

        <Box sx={{ textAlign: 'center' }}>
          <Typography variant="body2" color="text.secondary">
            Masz już konto?{' '}
            <Link
              to="/login"
              style={{
                color: 'inherit',
                textDecoration: 'underline'
              }}
            >
              Zaloguj się
            </Link>
          </Typography>
        </Box>
      </form>
    </Box>
  );
};