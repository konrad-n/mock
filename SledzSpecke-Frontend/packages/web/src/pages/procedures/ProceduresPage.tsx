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
  TextField,
  InputAdornment,
  CircularProgress,
  Alert,
  Stack,
  Tooltip
} from '@mui/material';
import { 
  Add, 
  Edit, 
  Delete, 
  MedicalServices, 
  Search,
  PersonOutline,
  SupervisorAccount,
  Groups
} from '@mui/icons-material';
import { useProcedures, useDeleteProcedure } from '@/hooks/useProcedures';
import { ProcedureForm } from '@/components/forms/ProcedureForm';
import { formatDate } from '@shared/utils';
import { Procedure } from '@shared/domain/entities';
import { SyncStatus, OperatorLevel } from '@shared/domain/value-objects';

export const ProceduresPage = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [formOpen, setFormOpen] = useState(false);
  const [selectedProcedure, setSelectedProcedure] = useState<Procedure | null>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [procedureToDelete, setProcedureToDelete] = useState<number | null>(null);

  // For now, using mock internshipId - in real app, get from context or route
  const internshipId = 1;
  
  const { data: procedures = [], isLoading, error } = useProcedures(internshipId);
  const deleteMutation = useDeleteProcedure();

  const handleAdd = () => {
    setSelectedProcedure(null);
    setFormOpen(true);
  };

  const handleEdit = (procedure: Procedure) => {
    setSelectedProcedure(procedure);
    setFormOpen(true);
  };

  const handleDelete = (id: number) => {
    setProcedureToDelete(id);
    setDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (procedureToDelete) {
      await deleteMutation.mutateAsync(procedureToDelete);
      setDeleteDialogOpen(false);
      setProcedureToDelete(null);
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

  const getOperatorIcon = (level: OperatorLevel) => {
    switch (level) {
      case OperatorLevel.Operator:
        return <PersonOutline fontSize="small" />;
      case OperatorLevel.FirstAssistant:
        return <SupervisorAccount fontSize="small" />;
      case OperatorLevel.SecondAssistant:
        return <Groups fontSize="small" />;
      default:
        return <PersonOutline fontSize="small" />;
    }
  };

  const getOperatorLabel = (level: OperatorLevel) => {
    switch (level) {
      case OperatorLevel.Operator:
        return 'Operator';
      case OperatorLevel.FirstAssistant:
        return 'Pierwsza asysta';
      case OperatorLevel.SecondAssistant:
        return 'Druga asysta';
      default:
        return 'Operator';
    }
  };

  // Filter procedures by search term
  const filteredProcedures = procedures.filter(procedure => 
    procedure.procedureName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    procedure.procedureCode.toLowerCase().includes(searchTerm.toLowerCase()) ||
    procedure.location.toLowerCase().includes(searchTerm.toLowerCase())
  );

  // Calculate statistics
  const stats = {
    total: filteredProcedures.length,
    asOperator: filteredProcedures.filter(p => p.operatorLevel === OperatorLevel.Operator).length,
    asAssistant: filteredProcedures.filter(p => p.operatorLevel !== OperatorLevel.Operator).length,
  };

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
        Wystąpił błąd podczas wczytywania procedur.
      </Alert>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Procedury medyczne</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={handleAdd}
        >
          Dodaj procedurę
        </Button>
      </Box>

      <Paper sx={{ p: 2, mb: 3 }}>
        <Stack spacing={2}>
          <TextField
            placeholder="Szukaj po nazwie, kodzie lub lokalizacji..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <Search />
                </InputAdornment>
              ),
            }}
            fullWidth
          />
          
          <Stack direction="row" spacing={3}>
            <Box>
              <Typography variant="h6" color="primary">
                {stats.total}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Wszystkich procedur
              </Typography>
            </Box>
            <Box>
              <Typography variant="h6" color="success.main">
                {stats.asOperator}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Jako operator
              </Typography>
            </Box>
            <Box>
              <Typography variant="h6" color="info.main">
                {stats.asAssistant}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Jako asysta
              </Typography>
            </Box>
          </Stack>
        </Stack>
      </Paper>

      {filteredProcedures.length === 0 ? (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <MedicalServices sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
          <Typography variant="h6" color="text.secondary">
            {searchTerm ? 'Nie znaleziono procedur spełniających kryteria' : 'Brak zarejestrowanych procedur'}
          </Typography>
          {!searchTerm && (
            <Button
              variant="contained"
              sx={{ mt: 2 }}
              onClick={handleAdd}
            >
              Dodaj pierwszą procedurę
            </Button>
          )}
        </Paper>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Data</TableCell>
                <TableCell>Kod</TableCell>
                <TableCell>Nazwa procedury</TableCell>
                <TableCell>Rola</TableCell>
                <TableCell>Miejsce</TableCell>
                <TableCell>Pacjent</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="right">Akcje</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredProcedures.map((procedure) => (
                <TableRow key={procedure.id}>
                  <TableCell>{formatDate(procedure.performanceDate)}</TableCell>
                  <TableCell>
                    <Typography variant="body2" fontWeight="medium">
                      {procedure.procedureCode}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Typography variant="body2">
                      {procedure.procedureName}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Tooltip title={getOperatorLabel(procedure.operatorLevel)}>
                      <Chip
                        icon={getOperatorIcon(procedure.operatorLevel)}
                        label={getOperatorLabel(procedure.operatorLevel)}
                        size="small"
                        color={procedure.operatorLevel === OperatorLevel.Operator ? 'primary' : 'default'}
                      />
                    </Tooltip>
                  </TableCell>
                  <TableCell>{procedure.location}</TableCell>
                  <TableCell>
                    <Typography variant="body2">
                      {procedure.patientAge} lat, {procedure.patientSex}
                    </Typography>
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={getSyncStatusLabel(procedure.syncStatus)}
                      color={getSyncStatusColor(procedure.syncStatus)}
                      size="small"
                    />
                  </TableCell>
                  <TableCell align="right">
                    <IconButton
                      onClick={() => handleEdit(procedure)}
                      disabled={procedure.syncStatus === SyncStatus.Approved}
                      size="small"
                    >
                      <Edit />
                    </IconButton>
                    <IconButton
                      onClick={() => handleDelete(procedure.id)}
                      disabled={procedure.syncStatus === SyncStatus.Approved}
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
          {selectedProcedure ? 'Edytuj procedurę' : 'Dodaj nową procedurę'}
        </DialogTitle>
        <DialogContent>
          <ProcedureForm
            procedure={selectedProcedure}
            internshipId={internshipId}
            onSuccess={() => setFormOpen(false)}
          />
        </DialogContent>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Potwierdzenie usunięcia</DialogTitle>
        <DialogContent>
          <Typography>
            Czy na pewno chcesz usunąć tę procedurę? Tej operacji nie można cofnąć.
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