import { Box, Typography, Paper, Button, Chip } from '@mui/material';
import { Add, EventBusy } from '@mui/icons-material';

export const AbsencesPage = () => {
  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Nieobecności</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
        >
          Zgłoś nieobecność
        </Button>
      </Box>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="subtitle1" gutterBottom>
          Podsumowanie
        </Typography>
        <Box display="flex" gap={2} flexWrap="wrap">
          <Chip label="Dni wykorzystane: 0" color="primary" />
          <Chip label="Dni pozostałe: 30" color="success" />
          <Chip label="Urlop naukowy: 0" />
          <Chip label="Zwolnienia lekarskie: 0" />
        </Box>
      </Paper>

      <Paper sx={{ p: 4, textAlign: 'center' }}>
        <EventBusy sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
        <Typography variant="h6" color="text.secondary" gutterBottom>
          Brak zgłoszonych nieobecności
        </Typography>
        <Typography variant="body2" color="text.secondary" mb={3}>
          Wszystkie Twoje nieobecności będą wyświetlane tutaj
        </Typography>
        <Button variant="contained" startIcon={<Add />}>
          Zgłoś nieobecność
        </Button>
      </Paper>
    </Box>
  );
};