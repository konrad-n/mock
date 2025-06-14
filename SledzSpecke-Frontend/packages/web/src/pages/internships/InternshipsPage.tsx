import { useState } from 'react';
import {
  Box,
  Typography,
  Button,
  Paper,
  Card,
  CardContent,
  CardActions,
  IconButton,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  CircularProgress,
  Alert,
  Stack,
  LinearProgress,
  Grid,
  Tooltip
} from '@mui/material';
import { 
  Add, 
  Edit, 
  Delete, 
  Business,
  Event,
  Person,
  CheckCircle,
  Warning
} from '@mui/icons-material';
import { useInternships, useDeleteInternship } from '@/hooks/useInternships';
import { InternshipForm } from '@/components/forms/InternshipForm';
import { formatDate } from '@shared/utils';
import { Internship } from '@shared/domain/entities';
import { SyncStatus } from '@shared/domain/value-objects';

export const InternshipsPage = () => {
  const [formOpen, setFormOpen] = useState(false);
  const [selectedInternship, setSelectedInternship] = useState<Internship | null>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [internshipToDelete, setInternshipToDelete] = useState<number | null>(null);

  // For now, using mock specializationId - in real app, get from context or route
  const specializationId = 1;
  
  const { data: internships = [], isLoading, error } = useInternships(specializationId);
  const deleteMutation = useDeleteInternship();

  const handleAdd = () => {
    setSelectedInternship(null);
    setFormOpen(true);
  };

  const handleEdit = (internship: Internship) => {
    setSelectedInternship(internship);
    setFormOpen(true);
  };

  const handleDelete = (id: number) => {
    setInternshipToDelete(id);
    setDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (internshipToDelete) {
      await deleteMutation.mutateAsync(internshipToDelete);
      setDeleteDialogOpen(false);
      setInternshipToDelete(null);
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

  const calculateProgress = (completed: number, required: number) => {
    return Math.min((completed / required) * 100, 100);
  };

  const calculateDaysRemaining = (endDate: string) => {
    const end = new Date(endDate);
    const today = new Date();
    const diffTime = end.getTime() - today.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays > 0 ? diffDays : 0;
  };

  // Calculate overall statistics
  const totalRequired = internships.reduce((sum, i) => sum + i.requiredDays, 0);
  const totalCompleted = internships.reduce((sum, i) => sum + i.completedDays, 0);
  const overallProgress = totalRequired > 0 ? (totalCompleted / totalRequired) * 100 : 0;

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
        Wystąpił błąd podczas wczytywania staży.
      </Alert>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Staże</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={handleAdd}
        >
          Dodaj staż
        </Button>
      </Box>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Stack spacing={2}>
          <Typography variant="h6">Postęp ogólny</Typography>
          <Box>
            <Stack direction="row" justifyContent="space-between" alignItems="baseline" mb={1}>
              <Box display="flex" alignItems="baseline" gap={1}>
                <Typography variant="h5" color="primary" fontWeight="600">
                  {totalCompleted} dni
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  / {totalRequired} dni (wymagane)
                </Typography>
              </Box>
              <Typography variant="body2" color="text.secondary">
                {overallProgress.toFixed(0)}%
              </Typography>
            </Stack>
            <LinearProgress 
              variant="determinate" 
              value={overallProgress} 
              sx={{ 
                height: 10, 
                borderRadius: 1,
                backgroundColor: 'grey.200',
                '& .MuiLinearProgress-bar': {
                  borderRadius: 1,
                  backgroundColor: overallProgress >= 100 ? 'success.main' : 'primary.main'
                }
              }}
            />
          </Box>
          <Typography variant="body2" color="text.secondary">
            Liczba staży: {internships.length}
          </Typography>
        </Stack>
      </Paper>

      {internships.length === 0 ? (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Business sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
          <Typography variant="h6" color="text.secondary">
            Brak zarejestrowanych staży
          </Typography>
          <Button
            variant="contained"
            sx={{ mt: 2 }}
            onClick={handleAdd}
          >
            Dodaj pierwszy staż
          </Button>
        </Paper>
      ) : (
        <Grid container spacing={3}>
          {internships.map((internship) => {
            const progress = calculateProgress(internship.completedDays, internship.requiredDays);
            const daysRemaining = calculateDaysRemaining(internship.endDate);
            const isComplete = internship.completedDays >= internship.requiredDays;

            return (
              <Grid item xs={12} md={6} lg={4} key={internship.id}>
                <Card>
                  <CardContent>
                    <Stack spacing={2}>
                      <Box display="flex" justifyContent="space-between" alignItems="start">
                        <Typography variant="h6" component="div" sx={{ fontWeight: 600 }}>
                          {internship.departmentName}
                        </Typography>
                        <Chip
                          label={getSyncStatusLabel(internship.syncStatus)}
                          color={getSyncStatusColor(internship.syncStatus)}
                          size="small"
                        />
                      </Box>

                      <Stack spacing={1}>
                        <Box display="flex" alignItems="center" gap={1}>
                          <Business fontSize="small" color="action" />
                          <Typography variant="body2" color="text.secondary">
                            {internship.institutionName}
                          </Typography>
                        </Box>
                        
                        <Box display="flex" alignItems="center" gap={1}>
                          <Person fontSize="small" color="action" />
                          <Typography variant="body2" color="text.secondary">
                            {internship.supervisorName}
                          </Typography>
                        </Box>

                        <Box display="flex" alignItems="center" gap={1}>
                          <Event fontSize="small" color="action" />
                          <Typography variant="body2" color="text.secondary">
                            {formatDate(internship.startDate)} - {formatDate(internship.endDate)}
                          </Typography>
                        </Box>
                      </Stack>

                      <Box>
                        <Stack direction="row" justifyContent="space-between" alignItems="center" mb={1}>
                          <Typography variant="body2">
                            Zrealizowano {internship.completedDays} z {internship.requiredDays} dni
                          </Typography>
                          {isComplete ? (
                            <CheckCircle color="success" fontSize="small" />
                          ) : daysRemaining < 30 ? (
                            <Tooltip title={`Pozostało ${daysRemaining} dni`}>
                              <Warning color="warning" fontSize="small" />
                            </Tooltip>
                          ) : null}
                        </Stack>
                        <LinearProgress 
                          variant="determinate" 
                          value={progress} 
                          sx={{ 
                            height: 8, 
                            borderRadius: 1,
                            backgroundColor: 'grey.200',
                            '& .MuiLinearProgress-bar': {
                              borderRadius: 1,
                              backgroundColor: isComplete ? 'success.main' : 'primary.main'
                            }
                          }}
                        />
                      </Box>
                    </Stack>
                  </CardContent>
                  
                  <CardActions sx={{ justifyContent: 'flex-end' }}>
                    <IconButton
                      onClick={() => handleEdit(internship)}
                      disabled={internship.syncStatus === SyncStatus.Approved}
                      size="small"
                    >
                      <Edit />
                    </IconButton>
                    <IconButton
                      onClick={() => handleDelete(internship.id)}
                      disabled={internship.syncStatus === SyncStatus.Approved}
                      color="error"
                      size="small"
                    >
                      <Delete />
                    </IconButton>
                  </CardActions>
                </Card>
              </Grid>
            );
          })}
        </Grid>
      )}

      {/* Add/Edit Dialog */}
      <Dialog open={formOpen} onClose={() => setFormOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>
          {selectedInternship ? 'Edytuj staż' : 'Dodaj nowy staż'}
        </DialogTitle>
        <DialogContent>
          <InternshipForm
            internship={selectedInternship}
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
            Czy na pewno chcesz usunąć ten staż? Tej operacji nie można cofnąć.
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