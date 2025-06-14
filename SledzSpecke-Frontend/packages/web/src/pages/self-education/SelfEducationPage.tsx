import { useState } from 'react';
import {
  Box,
  Typography,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  CircularProgress,
  Alert,
  Stack,
  Grid,
  LinearProgress
} from '@mui/material';
import { 
  Add, 
  Edit, 
  Delete, 
  Book,
  MenuBook,
  Article,
  OndemandVideo,
  Science,
  Description
} from '@mui/icons-material';
import { useSelfEducation, useDeleteSelfEducation } from '@/hooks/useCourses';
import { SelfEducationForm } from '@/components/forms/SelfEducationForm';
import { formatDate } from '@shared/utils';
import { SelfEducation, SelfEducationType } from '@shared/domain/entities';
import { SyncStatus } from '@shared/domain/value-objects';

export const SelfEducationPage = () => {
  const [formOpen, setFormOpen] = useState(false);
  const [selectedActivity, setSelectedActivity] = useState<SelfEducation | null>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [activityToDelete, setActivityToDelete] = useState<number | null>(null);

  // For now, using mock specializationId - in real app, get from context or route
  const specializationId = 1;
  
  const { data: activities = [], isLoading, error } = useSelfEducation(specializationId);
  const deleteMutation = useDeleteSelfEducation();

  const handleAdd = () => {
    setSelectedActivity(null);
    setFormOpen(true);
  };

  const handleEdit = (activity: SelfEducation) => {
    setSelectedActivity(activity);
    setFormOpen(true);
  };

  const handleDelete = (id: number) => {
    setActivityToDelete(id);
    setDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (activityToDelete) {
      await deleteMutation.mutateAsync(activityToDelete);
      setDeleteDialogOpen(false);
      setActivityToDelete(null);
    }
  };

  const getSyncStatusColor = (status: SyncStatus) => {
    switch (status) {
      case SyncStatus.Synced:
        return 'success';
      case SyncStatus.Modified:
        return 'warning';
      case SyncStatus.Approved:
        return 'info';
      default:
        return 'default';
    }
  };

  const getSyncStatusLabel = (status: SyncStatus) => {
    switch (status) {
      case SyncStatus.Synced:
        return 'Zsynchronizowany';
      case SyncStatus.Modified:
        return 'Zmodyfikowany';
      case SyncStatus.Approved:
        return 'Zatwierdzony';
      default:
        return 'Niezsynchonizowany';
    }
  };

  const getActivityTypeLabel = (type: SelfEducationType) => {
    switch (type) {
      case SelfEducationType.BookReading:
        return 'Lektura książki';
      case SelfEducationType.ArticleReview:
        return 'Przegląd artykułów';
      case SelfEducationType.OnlineLecture:
        return 'Wykład online';
      case SelfEducationType.CaseStudy:
        return 'Analiza przypadku';
      case SelfEducationType.Research:
        return 'Badania własne';
      case SelfEducationType.Other:
        return 'Inne';
      default:
        return type;
    }
  };

  const getActivityTypeIcon = (type: SelfEducationType) => {
    switch (type) {
      case SelfEducationType.BookReading:
        return <MenuBook fontSize="small" />;
      case SelfEducationType.ArticleReview:
        return <Article fontSize="small" />;
      case SelfEducationType.OnlineLecture:
        return <OndemandVideo fontSize="small" />;
      case SelfEducationType.CaseStudy:
        return <Description fontSize="small" />;
      case SelfEducationType.Research:
        return <Science fontSize="small" />;
      default:
        return <Book fontSize="small" />;
    }
  };

  // Calculate statistics
  const totalActivities = activities.length;
  const totalCreditHours = activities.reduce((sum, activity) => sum + activity.creditHours, 0);
  const requiredHours = 100; // Should come from specialization requirements
  const progressPercentage = Math.min((totalCreditHours / requiredHours) * 100, 100);
  
  const activitiesByType = activities.reduce((acc, activity) => {
    acc[activity.activityType] = (acc[activity.activityType] || 0) + 1;
    return acc;
  }, {} as Record<SelfEducationType, number>);

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" p={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error" sx={{ m: 2 }}>
        Wystąpił błąd podczas wczytywania samokształcenia.
      </Alert>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Samokształcenie</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={handleAdd}
        >
          Dodaj aktywność
        </Button>
      </Box>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Stack spacing={3}>
          <Typography variant="h6">Postęp samokształcenia</Typography>
          <Box>
            <Stack direction="row" justifyContent="space-between" alignItems="baseline" mb={1}>
              <Box display="flex" alignItems="baseline" gap={1}>
                <Typography variant="h5" color="primary" fontWeight="600">
                  {totalCreditHours} punktów
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  / {requiredHours} punktów (wymagane)
                </Typography>
              </Box>
              <Typography variant="body2" color="text.secondary">
                {progressPercentage.toFixed(0)}%
              </Typography>
            </Stack>
            <LinearProgress 
              variant="determinate" 
              value={progressPercentage} 
              sx={{ 
                height: 10, 
                borderRadius: 1,
                backgroundColor: 'grey.200',
                '& .MuiLinearProgress-bar': {
                  borderRadius: 1,
                  backgroundColor: progressPercentage >= 100 ? 'success.main' : 'primary.main'
                }
              }}
            />
          </Box>
          
          <Grid container spacing={2}>
            <Grid item xs={12} md={6}>
              <Typography variant="body2" color="text.secondary">
                Liczba aktywności: {totalActivities}
              </Typography>
            </Grid>
            <Grid item xs={12} md={6}>
              <Stack direction="row" spacing={1} flexWrap="wrap">
                {Object.entries(activitiesByType).map(([type, count]) => (
                  <Chip
                    key={type}
                    icon={getActivityTypeIcon(type as SelfEducationType)}
                    label={`${getActivityTypeLabel(type as SelfEducationType)} (${count})`}
                    size="small"
                    variant="outlined"
                  />
                ))}
              </Stack>
            </Grid>
          </Grid>
        </Stack>
      </Paper>

      {activities.length === 0 ? (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Book sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
          <Typography variant="h6" color="text.secondary">
            Brak zarejestrowanych aktywności samokształcenia
          </Typography>
          <Button
            variant="contained"
            sx={{ mt: 2 }}
            onClick={handleAdd}
          >
            Dodaj pierwszą aktywność
          </Button>
        </Paper>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Data</TableCell>
                <TableCell>Typ aktywności</TableCell>
                <TableCell>Tytuł</TableCell>
                <TableCell>Opis</TableCell>
                <TableCell align="center">Punkty</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="right">Akcje</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {activities.map((activity) => (
                <TableRow key={activity.id}>
                  <TableCell>{formatDate(activity.date)}</TableCell>
                  <TableCell>
                    <Stack direction="row" spacing={1} alignItems="center">
                      {getActivityTypeIcon(activity.activityType)}
                      <Typography variant="body2">
                        {getActivityTypeLabel(activity.activityType)}
                      </Typography>
                    </Stack>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" fontWeight="medium">
                      {activity.title}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2" sx={{ 
                      maxWidth: 300, 
                      overflow: 'hidden', 
                      textOverflow: 'ellipsis',
                      whiteSpace: 'nowrap' 
                    }}>
                      {activity.description}
                    </Typography>
                  </TableCell>
                  <TableCell align="center">
                    <Typography variant="body2" fontWeight="medium" color="primary">
                      {activity.creditHours}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={getSyncStatusLabel(activity.syncStatus)}
                      color={getSyncStatusColor(activity.syncStatus)}
                      size="small"
                    />
                  </TableCell>
                  <TableCell align="right">
                    <IconButton
                      onClick={() => handleEdit(activity)}
                      disabled={activity.syncStatus === SyncStatus.Approved}
                      size="small"
                    >
                      <Edit />
                    </IconButton>
                    <IconButton
                      onClick={() => handleDelete(activity.id)}
                      disabled={activity.syncStatus === SyncStatus.Approved}
                      color="error"
                      size="small"
                    >
                      <Delete />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* Add/Edit Dialog */}
      <Dialog open={formOpen} onClose={() => setFormOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>
          {selectedActivity ? 'Edytuj aktywność' : 'Dodaj nową aktywność'}
        </DialogTitle>
        <DialogContent>
          <SelfEducationForm
            activity={selectedActivity}
            specializationId={specializationId}
            onSuccess={() => setFormOpen(false)}
          />
        </DialogContent>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Potwierdzenie usunięcia</DialogTitle>
        <DialogContent>
          <Typography>
            Czy na pewno chcesz usunąć tę aktywność? Tej operacji nie można cofnąć.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>Anuluj</Button>
          <Button
            onClick={confirmDelete}
            color="error"
            variant="contained"
            disabled={deleteMutation.isPending}
          >
            {deleteMutation.isPending ? <CircularProgress size={20} /> : 'Usuń'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};