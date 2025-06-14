import { Outlet } from 'react-router-dom';
import { Box, Container, Paper } from '@mui/material';

export const AuthLayout = () => {
  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: 'background.default',
        padding: 2
      }}
    >
      <Container maxWidth="sm">
        <Paper
          elevation={3}
          sx={{
            p: 4,
            borderRadius: 2
          }}
        >
          <Outlet />
        </Paper>
      </Container>
    </Box>
  );
};