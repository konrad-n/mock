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
  IconButton
} from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useMutation } from '@tanstack/react-query';
import { signInSchema } from '@shared/utils/validation';
import { SignInRequest } from '@shared/types';
import { SmkVersion } from '@shared/domain/value-objects';
import { apiClient } from '@/services/api';
import { useAuthStore } from '@/stores/authStore';

type FormData = SignInRequest;

export const LoginPage = () => {
  const navigate = useNavigate();
  const setAuth = useAuthStore((state) => state.setAuth);
  const [showPassword, setShowPassword] = useState(false);
  
  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<FormData>({
    resolver: zodResolver(signInSchema)
  });

  const loginMutation = useMutation({
    mutationFn: (data: SignInRequest) => apiClient.signIn(data),
    onSuccess: (response) => {
      // Extract user data from JWT claims
      const email = response.Claims?.email?.[0] || '';
      const fullName = response.Claims?.full_name?.[0] || response.Claims?.name?.[0] || '';
      
      // Create user object from response
      const user = {
        id: response.UserId,
        email: email,
        fullName: fullName,
        smkVersion: SmkVersion.New, // This should come from backend
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      };
      
      setAuth(user, response.AccessToken);
      navigate('/dashboard');
    }
  });

  const onSubmit = (data: FormData) => {
    loginMutation.mutate(data);
  };

  return (
    <Box>
      <Box sx={{ mb: 4, textAlign: 'center' }}>
        <Box sx={{ mb: 2 }}>
          <img 
            src="/logo.png" 
            alt="SledzSpecke Logo" 
            style={{ 
              width: 120, 
              height: 120,
              objectFit: 'contain'
            }}
          />
        </Box>
        <Typography variant="h4" component="h1" gutterBottom color="primary">
          SledzSpecke
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Zaloguj się do systemu śledzenia specjalizacji
        </Typography>
      </Box>

      {loginMutation.isError && (
        <Alert severity="error" sx={{ mb: 3 }}>
          Nieprawidłowy email lub hasło. Spróbuj ponownie.
        </Alert>
      )}

      <form onSubmit={handleSubmit(onSubmit)}>
        <TextField
          {...register('email')}
          label="Adres email"
          type="email"
          fullWidth
          margin="normal"
          error={!!errors.email}
          helperText={errors.email?.message || "Test: użyj email i hasło 'Test123!'"}
          autoComplete="email"
          autoFocus
        />

        <TextField
          {...register('password')}
          label="Hasło"
          type={showPassword ? 'text' : 'password'}
          fullWidth
          margin="normal"
          error={!!errors.password}
          helperText={errors.password?.message}
          autoComplete="current-password"
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

        <Button
          type="submit"
          fullWidth
          variant="contained"
          size="large"
          sx={{ mt: 3, mb: 2 }}
          disabled={loginMutation.isPending}
        >
          {loginMutation.isPending ? (
            <CircularProgress size={24} color="inherit" />
          ) : (
            'Zaloguj się'
          )}
        </Button>

        <Box sx={{ textAlign: 'center' }}>
          <Typography variant="body2" color="text.secondary">
            Nie masz konta?{' '}
            <Link
              to="/register"
              style={{
                color: 'inherit',
                textDecoration: 'underline'
              }}
            >
              Zarejestruj się
            </Link>
          </Typography>
        </Box>
      </form>
    </Box>
  );
};