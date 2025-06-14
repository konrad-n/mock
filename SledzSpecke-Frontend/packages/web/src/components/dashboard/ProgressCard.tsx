import { Card, CardContent, Box, Typography, LinearProgress } from '@mui/material';
import { useNavigate } from 'react-router-dom';

interface ProgressCardProps {
  title: string;
  completed: number;
  total: number;
  percentage: number;
  icon: React.ReactNode;
  color: 'primary' | 'secondary' | 'success' | 'error' | 'warning' | 'info';
  link: string;
  unit?: string;
}

export const ProgressCard = ({
  title,
  completed,
  total,
  percentage,
  icon,
  color,
  link,
  unit = ''
}: ProgressCardProps) => {
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
            <Typography variant="body2" color="text.secondary">
              {completed}{unit} z {total}{unit}
            </Typography>
          </Box>
        </Box>

        <Box>
          <Box display="flex" justifyContent="space-between" mb={1}>
            <Typography variant="body2" color="text.secondary">
              Postęp
            </Typography>
            <Typography variant="body2" fontWeight="medium" color={color}>
              {percentage}%
            </Typography>
          </Box>
          <LinearProgress
            variant="determinate"
            value={percentage}
            color={color}
            sx={{
              height: 8,
              borderRadius: 4,
              bgcolor: `${color}.50`
            }}
          />
        </Box>
      </CardContent>
    </Card>
  );
};