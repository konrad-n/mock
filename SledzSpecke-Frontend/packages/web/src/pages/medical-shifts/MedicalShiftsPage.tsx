import { useState, useEffect } from 'react';
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
  LinearProgress
} from '@mui/material';
import { Add, Edit, Delete, CalendarMonth } from '@mui/icons-material';
import { useMedicalShifts, useDeleteMedicalShift } from '@/hooks/useMedicalShifts';
import { MedicalShiftForm } from '@/components/forms/MedicalShiftForm';
import { formatDate } from '@shared/utils';
import { MedicalShift } from '@shared/domain/entities';
import { SyncStatus } from '@shared/domain/value-objects';
import { useModule } from '@/contexts/ModuleContext';
import { useQuery } from '@tanstack/react-query';
import { apiClient } from '@/services/api';

export const MedicalShiftsPage = () => {
  const { currentModule } = useModule();
  const [selectedTrainingYear, setSelectedTrainingYear] = useState<number>(1);
  const [formOpen, setFormOpen] = useState(false);
  const [selectedShift, setSelectedShift] = useState<MedicalShift | null>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [shiftToDelete, setShiftToDelete] = useState<number | null>(null);
  const [currentInternshipId, setCurrentInternshipId] = useState<number | null>(null);

  // Fetch internships for current module
  const { data: internships = [] } = useQuery({
    queryKey: ['internships', currentModule?.id],
    queryFn: async () => {
      if (!currentModule) return [];
      try {
        const response = await apiClient.get(`/api/modules/${currentModule.id}/internships`);
        return response;
      } catch {
        return [];
      }
    },
    enabled: !!currentModule
  });

  // Set current internship when internships load
  useEffect(() => {
    if (internships.length > 0 && !currentInternshipId) {
      setCurrentInternshipId(internships[0].id);
    }
  }, [internships, currentInternshipId]);
  
  const { data: shifts = [], isLoading, error } = useMedicalShifts(currentInternshipId || 0);
  const deleteMutation = useDeleteMedicalShift();

  const handleAdd = () => {
    setSelectedShift(null);
    setFormOpen(true);
  };

  const handleEdit = (shift: MedicalShift) => {
    setSelectedShift(shift);
    setFormOpen(true);
  };

  const handleDelete = (id: number) => {
    setShiftToDelete(id);
    setDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (shiftToDelete) {
      await deleteMutation.mutateAsync(shiftToDelete);
      setDeleteDialogOpen(false);
      setShiftToDelete(null);
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

  // Filter shifts by training year
  const filteredShifts = shifts.filter(shift => shift.year === selectedTrainingYear);

  // Calculate total hours and minutes
  const totalMinutes = filteredShifts.reduce((sum, shift) => 
    sum + shift.duration.toTotalMinutes(), 0
  );
  const totalHours = Math.floor(totalMinutes / 60);
  const remainingMinutes = totalMinutes % 60;
  
  // Required hours per year (example - should come from specialization template)
  const requiredHours = 1048;
  const progressPercentage = Math.min((totalMinutes / (requiredHours * 60)) * 100, 100);

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
        Wystąpił błąd podczas wczytywania dyżurów medycznych.
      </Alert>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Dyżury medyczne</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={handleAdd}
        >
          Dodaj dyżur
        </Button>
      </Box>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Stack spacing={3}>
          <Box>
            <Typography variant="subtitle1" gutterBottom>Rok szkolenia:</Typography>
            <Stack direction="row" spacing={1}>
              {[1, 2, 3, 4, 5].map((year) => (
                <Chip
                  key={year}
                  label={`Rok ${year}`}
                  onClick={() => setSelectedTrainingYear(year)}
                  color={selectedTrainingYear === year ? 'primary' : 'default'}
                  clickable
                  sx={{ fontWeight: selectedTrainingYear === year ? 600 : 400 }}
                />
              ))}
            </Stack>
          </Box>
          
          <Box>
            <Stack direction="row" justifyContent="space-between" alignItems="baseline" mb={1}>
              <Box display="flex" alignItems="baseline" gap={1}>
                <Typography variant="h5" color="primary" fontWeight="600">
                  {totalHours}h {remainingMinutes}min
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  / {requiredHours}h (wymagane)
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
            <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, display: 'block' }}>
              Liczba dyżurów: {filteredShifts.length}
            </Typography>
          </Box>
        </Stack>
      </Paper>

      {filteredShifts.length === 0 ? (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <CalendarMonth sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
          <Typography variant="h6" color="text.secondary">
            Brak dyżurów w roku {selectedTrainingYear}
          </Typography>
          <Button
            variant="contained"
            sx={{ mt: 2 }}
            onClick={handleAdd}
          >
            Dodaj pierwszy dyżur
          </Button>
        </Paper>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Data</TableCell>
                <TableCell>Czas trwania</TableCell>
                <TableCell>Miejsce</TableCell>
                <TableCell>Rok szkolenia</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="right">Akcje</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredShifts.map((shift) => (
                <TableRow key={shift.id}>
                  <TableCell>{formatDate(shift.date)}</TableCell>
                  <TableCell>{shift.duration.toString()}</TableCell>
                  <TableCell>{shift.location}</TableCell>
                  <TableCell>{shift.year}</TableCell>
                  <TableCell>
                    <Chip
                      label={getSyncStatusLabel(shift.syncStatus)}
                      color={getSyncStatusColor(shift.syncStatus)}
                      size="small"
                    />
                  </TableCell>
                  <TableCell align="right">
                    <IconButton
                      onClick={() => handleEdit(shift)}
                      disabled={shift.syncStatus === SyncStatus.Approved}
                      size="small"
                    >
                      <Edit />
                    </IconButton>
                    <IconButton
                      onClick={() => handleDelete(shift.id)}
                      disabled={shift.syncStatus === SyncStatus.Approved}
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
      <Dialog open={formOpen} onClose={() => setFormOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>
          {selectedShift ? 'Edytuj dyżur' : 'Dodaj nowy dyżur'}
        </DialogTitle>
        <DialogContent>
          <MedicalShiftForm
            shift={selectedShift}
            internshipId={currentInternshipId || 1}
            onSuccess={() => setFormOpen(false)}
          />
        </DialogContent>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Potwierdzenie usunięcia</DialogTitle>
        <DialogContent>
          <Typography>
            Czy na pewno chcesz usunąć ten dyżur? Tej operacji nie można cofnąć.
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