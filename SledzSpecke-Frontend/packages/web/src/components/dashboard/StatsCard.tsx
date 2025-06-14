import { Card, CardContent, Box, Typography } from '@mui/material';
import { useNavigate } from 'react-router-dom';

interface StatsCardProps {
  title: string;
  value: number | string;
  subValue?: string;
  icon: React.ReactNode;
  color: 'primary' | 'secondary' | 'success' | 'error' | 'warning' | 'info';
  link: string;
}

export const StatsCard = ({
  title,
  value,
  subValue,
  icon,
  color,
  link
}: StatsCardProps) => {
  const navigate = useNavigate();

  return (
    <Card
      sx={{
        height: '100%',
        cursor: 'pointer',
        transition: 'all 0.3s ease',
        '&:hover': {
          transform: 'translateY(-4px)',
          boxShadow: 3
        }
      }}
      onClick={() => navigate(link)}
    >
      <CardContent>
        <Box display="flex" alignItems="center" mb={2}>
          <Box
            sx={{
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              width: 48,
              height: 48,
              borderRadius: 2,
              bgcolor: `${color}.light`,
              color: `${color}.main`,
              mr: 2
            }}
          >
            {icon}
          </Box>
          <Box flexGrow={1}>
            <Typography variant="h6" gutterBottom>
              {title}
            </Typography>
          </Box>
        </Box>

        <Box>
          <Typography variant="h4" color={color} fontWeight="medium">
            {value}
          </Typography>
          {subValue && (
            <Typography variant="body2" color="text.secondary" mt={1}>
              {subValue}
            </Typography>
          )}
        </Box>
      </CardContent>
    </Card>
  );
};