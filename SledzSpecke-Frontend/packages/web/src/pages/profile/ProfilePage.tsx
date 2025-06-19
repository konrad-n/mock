import { Box, Typography, Paper, TextField, Button, Avatar, Grid } from '@mui/material';
import { Save, Edit } from '@mui/icons-material';
import { useState } from 'react';
import { useAuthStore } from '@/stores/authStore';

export const ProfilePage = () => {
  const user = useAuthStore(state => state.user);
  const [isEditing, setIsEditing] = useState(false);

  return (
    <Box>
      <Typography variant="h4" mb={3}>Mój profil</Typography>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Box display="flex" alignItems="center" gap={3} mb={3}>
          <Avatar sx={{ width: 80, height: 80, bgcolor: 'primary.main', fontSize: '2rem' }}>
            {user?.fullName.charAt(0).toUpperCase()}
          </Avatar>
          <Box>
            <Typography variant="h5">{user?.fullName}</Typography>
            <Typography variant="body2" color="text.secondary">{user?.email}</Typography>
          </Box>
        </Box>
        
        <Divider sx={{ my: 3 }} />

        <Typography variant="h6" gutterBottom>
          Dane osobowe
        </Typography>
        
        <Grid container spacing={2}>
          <Grid item xs={12} md={6}>
            <TextField
              label="Imię i nazwisko"
              value={user?.fullName || ''}
              fullWidth
              disabled={!isEditing}
              margin="normal"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              label="Email"
              value={user?.email || ''}
              fullWidth
              disabled
              margin="normal"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              label="Telefon"
              value="000000000"
              fullWidth
              disabled={!isEditing}
              margin="normal"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              label="Data urodzenia"
              value="1990-01-01"
              type="date"
              fullWidth
              disabled={!isEditing}
              margin="normal"
              InputLabelProps={{
                shrink: true,
              }}
            />
          </Grid>
        </Grid>
      </Paper>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Adres korespondencyjny
        </Typography>
        
        <Grid container spacing={2}>
          <Grid item xs={12} md={6}>
            <TextField
              label="Ulica"
              value="Default Street"
              fullWidth
              disabled={!isEditing}
              margin="normal"
            />
          </Grid>
          <Grid item xs={12} md={3}>
            <TextField
              label="Numer domu"
              value="1"
              fullWidth
              disabled={!isEditing}
              margin="normal"
            />
          </Grid>
          <Grid item xs={12} md={3}>
            <TextField
              label="Numer mieszkania"
              value=""
              fullWidth
              disabled={!isEditing}
              margin="normal"
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <TextField
              label="Kod pocztowy"
              value="00-000"
              fullWidth
              disabled={!isEditing}
              margin="normal"
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <TextField
              label="Miasto"
              value="Warsaw"
              fullWidth
              disabled={!isEditing}
              margin="normal"
            />
          </Grid>
          <Grid item xs={12} md={4}>
            <TextField
              label="Województwo"
              value="Mazowieckie"
              fullWidth
              disabled={!isEditing}
              margin="normal"
            />
          </Grid>
        </Grid>
      </Paper>

      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>
          Informacje o specjalizacji
        </Typography>
        
        <Grid container spacing={2}>
          <Grid item xs={12} md={6}>
            <TextField
              label="Specjalizacja"
              value="Kardiologia"
              fullWidth
              disabled
              margin="normal"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              label="Wersja SMK"
              value="Nowy SMK"
              fullWidth
              disabled
              margin="normal"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              label="Data rozpoczęcia"
              value={new Date().toISOString().split('T')[0]}
              type="date"
              fullWidth
              disabled
              margin="normal"
              InputLabelProps={{
                shrink: true,
              }}
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              label="Planowana data zakończenia"
              value={new Date(new Date().setFullYear(new Date().getFullYear() + 5)).toISOString().split('T')[0]}
              type="date"
              fullWidth
              disabled
              margin="normal"
              InputLabelProps={{
                shrink: true,
              }}
            />
          </Grid>
        </Grid>
      </Paper>

      <Box sx={{ mt: 3, display: 'flex', justifyContent: 'flex-end', gap: 2 }}>
        {isEditing ? (
          <>
            <Button variant="outlined" onClick={() => setIsEditing(false)}>
              Anuluj
            </Button>
            <Button variant="contained" startIcon={<Save />} onClick={() => setIsEditing(false)}>
              Zapisz zmiany
            </Button>
          </>
        ) : (
          <Button variant="contained" startIcon={<Edit />} onClick={() => setIsEditing(true)}>
            Edytuj profil
          </Button>
        )}
      </Box>
    </Box>
  );
};

// Add missing import
import { Divider } from '@mui/material';