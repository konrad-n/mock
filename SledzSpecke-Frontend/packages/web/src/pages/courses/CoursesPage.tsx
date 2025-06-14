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
  Tabs,
  Tab,
  Grid,
  Card,
  CardContent,
  CardActions
} from '@mui/material';
import { 
  Add, 
  Edit, 
  Delete, 
  School,
  LocationOn,
  Business,
  Assignment,
  CalendarToday
} from '@mui/icons-material';
import { useCourses, useDeleteCourse } from '@/hooks/useCourses';
import { CourseForm } from '@/components/forms/CourseForm';
import { formatDate } from '@shared/utils';
import { Course, CourseType } from '@shared/domain/entities';
import { SyncStatus } from '@shared/domain/value-objects';

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`courses-tabpanel-${index}`}
      aria-labelledby={`courses-tab-${index}`}
      {...other}
    >
      {value === index && (
        <Box sx={{ pt: 3 }}>
          {children}
        </Box>
      )}
    </div>
  );
}

export const CoursesPage = () => {
  const [tabValue, setTabValue] = useState(0);
  const [formOpen, setFormOpen] = useState(false);
  const [selectedCourse, setSelectedCourse] = useState<Course | null>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [courseToDelete, setCourseToDelete] = useState<number | null>(null);

  // For now, using mock specializationId - in real app, get from context or route
  const specializationId = 1;
  
  const { data: courses = [], isLoading, error } = useCourses(specializationId);
  const deleteMutation = useDeleteCourse();

  const handleAdd = () => {
    setSelectedCourse(null);
    setFormOpen(true);
  };

  const handleEdit = (course: Course) => {
    setSelectedCourse(course);
    setFormOpen(true);
  };

  const handleDelete = (id: number) => {
    setCourseToDelete(id);
    setDeleteDialogOpen(true);
  };

  const confirmDelete = async () => {
    if (courseToDelete) {
      await deleteMutation.mutateAsync(courseToDelete);
      setDeleteDialogOpen(false);
      setCourseToDelete(null);
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

  const getCourseTypeLabel = (type: CourseType) => {
    switch (type) {
      case CourseType.Conference:
        return 'Konferencja';
      case CourseType.Workshop:
        return 'Warsztaty';
      case CourseType.Seminar:
        return 'Seminarium';
      case CourseType.OnlineCourse:
        return 'Kurs online';
      case CourseType.Other:
        return 'Inne';
      default:
        return type;
    }
  };

  const getCourseTypeColor = (type: CourseType) => {
    switch (type) {
      case CourseType.Conference:
        return 'primary';
      case CourseType.Workshop:
        return 'secondary';
      case CourseType.Seminar:
        return 'info';
      case CourseType.OnlineCourse:
        return 'warning';
      default:
        return 'default';
    }
  };

  // Calculate statistics
  const totalCourses = courses.length;
  const totalCreditHours = courses.reduce((sum, course) => sum + (course.creditHours || 0), 0);
  const coursesByType = courses.reduce((acc, course) => {
    acc[course.courseType] = (acc[course.courseType] || 0) + 1;
    return acc;
  }, {} as Record<CourseType, number>);

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
        Wystąpił błąd podczas wczytywania kursów.
      </Alert>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Kursy i szkolenia</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={handleAdd}
        >
          Dodaj kurs
        </Button>
      </Box>

      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 2 }}>
            <Stack spacing={1}>
              <Typography variant="h4" color="primary" fontWeight="600">
                {totalCourses}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Ukończonych kursów
              </Typography>
            </Stack>
          </Paper>
        </Grid>
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 2 }}>
            <Stack spacing={1}>
              <Typography variant="h4" color="secondary" fontWeight="600">
                {totalCreditHours}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Punktów edukacyjnych
              </Typography>
            </Stack>
          </Paper>
        </Grid>
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 2 }}>
            <Stack spacing={1}>
              <Typography variant="h6" fontWeight="600">
                Typy kursów
              </Typography>
              <Stack direction="row" spacing={1} flexWrap="wrap">
                {Object.entries(coursesByType).map(([type, count]) => (
                  <Chip
                    key={type}
                    label={`${getCourseTypeLabel(type as CourseType)} (${count})`}
                    size="small"
                    color={getCourseTypeColor(type as CourseType)}
                  />
                ))}
              </Stack>
            </Stack>
          </Paper>
        </Grid>
      </Grid>

      <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
        <Tabs value={tabValue} onChange={(_, newValue) => setTabValue(newValue)}>
          <Tab label="Lista kursów" />
          <Tab label="Widok kartek" />
        </Tabs>
      </Box>

      <TabPanel value={tabValue} index={0}>
        {courses.length === 0 ? (
          <Paper sx={{ p: 4, textAlign: 'center' }}>
            <School sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
            <Typography variant="h6" color="text.secondary">
              Brak zarejestrowanych kursów
            </Typography>
            <Button
              variant="contained"
              sx={{ mt: 2 }}
              onClick={handleAdd}
            >
              Dodaj pierwszy kurs
            </Button>
          </Paper>
        ) : (
          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Nazwa kursu</TableCell>
                  <TableCell>Typ</TableCell>
                  <TableCell>Data</TableCell>
                  <TableCell>Organizator</TableCell>
                  <TableCell>Miejsce</TableCell>
                  <TableCell align="center">Punkty</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell align="right">Akcje</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {courses.map((course) => (
                  <TableRow key={course.id}>
                    <TableCell>
                      <Typography variant="body2" fontWeight="medium">
                        {course.courseName}
                      </Typography>
                      {course.certificateNumber && (
                        <Typography variant="caption" color="text.secondary">
                          Certyfikat: {course.certificateNumber}
                        </Typography>
                      )}
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={getCourseTypeLabel(course.courseType)}
                        color={getCourseTypeColor(course.courseType)}
                        size="small"
                      />
                    </TableCell>
                    <TableCell>
                      {formatDate(course.startDate)}
                      {course.endDate !== course.startDate && ` - ${formatDate(course.endDate)}`}
                    </TableCell>
                    <TableCell>{course.organizer}</TableCell>
                    <TableCell>{course.location}</TableCell>
                    <TableCell align="center">
                      {course.creditHours || '-'}
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={getSyncStatusLabel(course.syncStatus)}
                        color={getSyncStatusColor(course.syncStatus)}
                        size="small"
                      />
                    </TableCell>
                    <TableCell align="right">
                      <IconButton
                        onClick={() => handleEdit(course)}
                        disabled={course.syncStatus === SyncStatus.Approved}
                        size="small"
                      >
                        <Edit />
                      </IconButton>
                      <IconButton
                        onClick={() => handleDelete(course.id)}
                        disabled={course.syncStatus === SyncStatus.Approved}
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
      </TabPanel>

      <TabPanel value={tabValue} index={1}>
        {courses.length === 0 ? (
          <Paper sx={{ p: 4, textAlign: 'center' }}>
            <School sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
            <Typography variant="h6" color="text.secondary">
              Brak zarejestrowanych kursów
            </Typography>
            <Button
              variant="contained"
              sx={{ mt: 2 }}
              onClick={handleAdd}
            >
              Dodaj pierwszy kurs
            </Button>
          </Paper>
        ) : (
          <Grid container spacing={3}>
            {courses.map((course) => (
              <Grid item xs={12} md={6} lg={4} key={course.id}>
                <Card>
                  <CardContent>
                    <Stack spacing={2}>
                      <Box display="flex" justifyContent="space-between" alignItems="start">
                        <Typography variant="h6" component="div" sx={{ fontWeight: 600 }}>
                          {course.courseName}
                        </Typography>
                        <Chip
                          label={getCourseTypeLabel(course.courseType)}
                          color={getCourseTypeColor(course.courseType)}
                          size="small"
                        />
                      </Box>

                      <Stack spacing={1}>
                        <Box display="flex" alignItems="center" gap={1}>
                          <CalendarToday fontSize="small" color="action" />
                          <Typography variant="body2" color="text.secondary">
                            {formatDate(course.startDate)}
                            {course.endDate !== course.startDate && ` - ${formatDate(course.endDate)}`}
                          </Typography>
                        </Box>
                        
                        <Box display="flex" alignItems="center" gap={1}>
                          <Business fontSize="small" color="action" />
                          <Typography variant="body2" color="text.secondary">
                            {course.organizer}
                          </Typography>
                        </Box>

                        <Box display="flex" alignItems="center" gap={1}>
                          <LocationOn fontSize="small" color="action" />
                          <Typography variant="body2" color="text.secondary">
                            {course.location}
                          </Typography>
                        </Box>

                        {course.creditHours && (
                          <Box display="flex" alignItems="center" gap={1}>
                            <School fontSize="small" color="action" />
                            <Typography variant="body2" color="text.secondary">
                              {course.creditHours} punktów edukacyjnych
                            </Typography>
                          </Box>
                        )}

                        {course.certificateNumber && (
                          <Box display="flex" alignItems="center" gap={1}>
                            <Assignment fontSize="small" color="action" />
                            <Typography variant="body2" color="text.secondary">
                              Certyfikat: {course.certificateNumber}
                            </Typography>
                          </Box>
                        )}
                      </Stack>

                      {course.notes && (
                        <Typography variant="body2" color="text.secondary" sx={{ fontStyle: 'italic' }}>
                          {course.notes}
                        </Typography>
                      )}

                      <Box display="flex" justifyContent="space-between" alignItems="center">
                        <Chip
                          label={getSyncStatusLabel(course.syncStatus)}
                          color={getSyncStatusColor(course.syncStatus)}
                          size="small"
                        />
                      </Box>
                    </Stack>
                  </CardContent>
                  
                  <CardActions sx={{ justifyContent: 'flex-end' }}>
                    <IconButton
                      onClick={() => handleEdit(course)}
                      disabled={course.syncStatus === SyncStatus.Approved}
                      size="small"
                    >
                      <Edit />
                    </IconButton>
                    <IconButton
                      onClick={() => handleDelete(course.id)}
                      disabled={course.syncStatus === SyncStatus.Approved}
                      color="error"
                      size="small"
                    >
                      <Delete />
                    </IconButton>
                  </CardActions>
                </Card>
              </Grid>
            ))}
          </Grid>
        )}
      </TabPanel>

      {/* Add/Edit Dialog */}
      <Dialog open={formOpen} onClose={() => setFormOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>
          {selectedCourse ? 'Edytuj kurs' : 'Dodaj nowy kurs'}
        </DialogTitle>
        <DialogContent>
          <CourseForm
            course={selectedCourse}
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
            Czy na pewno chcesz usunąć ten kurs? Tej operacji nie można cofnąć.
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