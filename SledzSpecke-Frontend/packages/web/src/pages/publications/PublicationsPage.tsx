import { Box, Typography, Paper, Button } from '@mui/material';
import { Add } from '@mui/icons-material';

export const PublicationsPage = () => {
  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Publikacje</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
        >
          Dodaj publikację
        </Button>
      </Box>

      <Paper sx={{ p: 4, textAlign: 'center' }}>
        <Typography variant="h6" color="text.secondary" gutterBottom>
          Brak publikacji
        </Typography>
        <Typography variant="body2" color="text.secondary" mb={3}>
          Dodaj swoją pierwszą publikację naukową
        </Typography>
        <Button variant="contained" startIcon={<Add />}>
          Dodaj publikację
        </Button>
      </Paper>
    </Box>
  );
};