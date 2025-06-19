import { Box, Typography, Paper, List, ListItem, ListItemText, Switch, Button, TextField } from '@mui/material';
import { Save } from '@mui/icons-material';
import { useState } from 'react';

export const SettingsPage = () => {
  const [emailNotifications, setEmailNotifications] = useState(true);
  const [pushNotifications, setPushNotifications] = useState(false);

  return (
    <Box>
      <Typography variant="h4" mb={3}>Ustawienia</Typography>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Powiadomienia
        </Typography>
        <List>
          <ListItem>
            <ListItemText
              primary="Powiadomienia email"
              secondary="Otrzymuj powiadomienia o ważnych terminach na email"
            />
            <Switch
              checked={emailNotifications}
              onChange={(e) => setEmailNotifications(e.target.checked)}
            />
          </ListItem>
          <ListItem>
            <ListItemText
              primary="Powiadomienia push"
              secondary="Otrzymuj powiadomienia w przeglądarce"
            />
            <Switch
              checked={pushNotifications}
              onChange={(e) => setPushNotifications(e.target.checked)}
            />
          </ListItem>
        </List>
      </Paper>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Zmiana hasła
        </Typography>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 2 }}>
          <TextField
            label="Obecne hasło"
            type="password"
            fullWidth
          />
          <TextField
            label="Nowe hasło"
            type="password"
            fullWidth
          />
          <TextField
            label="Potwierdź nowe hasło"
            type="password"
            fullWidth
          />
          <Button variant="contained" sx={{ alignSelf: 'flex-start' }}>
            Zmień hasło
          </Button>
        </Box>
      </Paper>

      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>
          Preferencje eksportu
        </Typography>
        <Typography variant="body2" color="text.secondary" gutterBottom>
          Wybierz domyślny format eksportu danych
        </Typography>
        <Box sx={{ mt: 2 }}>
          <TextField
            select
            label="Format eksportu"
            fullWidth
            defaultValue="excel"
            SelectProps={{
              native: true,
            }}
          >
            <option value="excel">Excel (SMK)</option>
            <option value="pdf">PDF</option>
            <option value="csv">CSV</option>
          </TextField>
        </Box>
      </Paper>

      <Box sx={{ mt: 3, display: 'flex', justifyContent: 'flex-end' }}>
        <Button variant="contained" startIcon={<Save />}>
          Zapisz ustawienia
        </Button>
      </Box>
    </Box>
  );
};