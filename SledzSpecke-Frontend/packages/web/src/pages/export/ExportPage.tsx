import { useState } from 'react';
import {
  Box,
  Typography,
  Button,
  Paper,
  FormControl,
  FormControlLabel,
  FormGroup,
  Checkbox,
  RadioGroup,
  Radio,
  FormLabel,
  Grid,
  Alert,
  CircularProgress,
  Stack,
  Divider,
  Chip,
} from '@mui/material';
import {
  GetApp,
  PictureAsPdf,
  TableChart,
  Code,
  CloudUpload,
  CheckCircle,
  Error as ErrorIcon,
} from '@mui/icons-material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { pl } from 'date-fns/locale';
import { exportService, ExportOptions } from '@/services/export';
// import { useAuthStore } from '@/stores/authStore'; // Will be used for getting user's specializationId

export const ExportPage = () => {
  // const user = useAuthStore((state) => state.user); // Will be used to get specializationId
  const [exportFormat, setExportFormat] = useState<'pdf' | 'excel' | 'json'>('pdf');
  const [smkVersion, setSmkVersion] = useState<'Old' | 'New'>('New');
  const [dateFrom, setDateFrom] = useState<Date | null>(null);
  const [dateTo, setDateTo] = useState<Date | null>(null);
  const [isExporting, setIsExporting] = useState(false);
  const [exportResult, setExportResult] = useState<{ success: boolean; filename?: string; error?: string } | null>(null);
  
  const [selectedData, setSelectedData] = useState({
    internships: true,
    medicalShifts: true,
    procedures: true,
    courses: true,
    selfEducation: true,
    publications: true,
    absences: true,
  });

  const handleDataChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSelectedData({
      ...selectedData,
      [event.target.name]: event.target.checked,
    });
  };

  const handleGeneralExport = async () => {
    setIsExporting(true);
    setExportResult(null);

    try {
      const options: ExportOptions = {
        specializationId: 1, // Should come from user context
        format: exportFormat,
        includeInternships: selectedData.internships,
        includeMedicalShifts: selectedData.medicalShifts,
        includeProcedures: selectedData.procedures,
        includeCourses: selectedData.courses,
        includeSelfEducation: selectedData.selfEducation,
        includePublications: selectedData.publications,
        includeAbsences: selectedData.absences,
        dateFrom: dateFrom?.toISOString(),
        dateTo: dateTo?.toISOString(),
      };

      const result = await exportService.exportData(options);
      setExportResult(result);
    } catch (error) {
      setExportResult({ 
        success: false, 
        error: 'Wystąpił błąd podczas eksportu danych' 
      });
    } finally {
      setIsExporting(false);
    }
  };

  const handleSMKExport = async () => {
    setIsExporting(true);
    setExportResult(null);

    try {
      const result = await exportService.exportToSMK(1, smkVersion);
      setExportResult(result);
    } catch (error) {
      setExportResult({ 
        success: false, 
        error: 'Wystąpił błąd podczas eksportu SMK' 
      });
    } finally {
      setIsExporting(false);
    }
  };

  const getFormatIcon = (format: string) => {
    switch (format) {
      case 'pdf':
        return <PictureAsPdf />;
      case 'excel':
        return <TableChart />;
      case 'json':
        return <Code />;
      default:
        return <GetApp />;
    }
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={pl}>
      <Box>
        <Typography variant="h4" gutterBottom>
          Eksport danych
        </Typography>
        <Typography variant="body1" color="text.secondary" gutterBottom>
          Eksportuj dane swojej specjalizacji do różnych formatów
        </Typography>

        {exportResult && (
          <Alert 
            severity={exportResult.success ? 'success' : 'error'}
            icon={exportResult.success ? <CheckCircle /> : <ErrorIcon />}
            sx={{ mb: 3 }}
            onClose={() => setExportResult(null)}
          >
            {exportResult.success 
              ? `Plik ${exportResult.filename} został pobrany pomyślnie`
              : exportResult.error
            }
          </Alert>
        )}

        <Grid container spacing={3}>
          {/* General Export */}
          <Grid item xs={12} lg={6}>
            <Paper sx={{ p: 3, height: '100%' }}>
              <Typography variant="h6" gutterBottom>
                Eksport ogólny
              </Typography>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Eksportuj wybrane dane w wybranym formacie
              </Typography>

              <Box sx={{ mt: 3 }}>
                <FormControl component="fieldset">
                  <FormLabel component="legend">Format eksportu</FormLabel>
                  <RadioGroup
                    row
                    value={exportFormat}
                    onChange={(e) => setExportFormat(e.target.value as 'pdf' | 'excel' | 'json')}
                  >
                    <FormControlLabel 
                      value="pdf" 
                      control={<Radio />} 
                      label={
                        <Stack direction="row" spacing={1} alignItems="center">
                          <PictureAsPdf fontSize="small" />
                          <span>PDF</span>
                        </Stack>
                      } 
                    />
                    <FormControlLabel 
                      value="excel" 
                      control={<Radio />} 
                      label={
                        <Stack direction="row" spacing={1} alignItems="center">
                          <TableChart fontSize="small" />
                          <span>Excel</span>
                        </Stack>
                      } 
                    />
                    <FormControlLabel 
                      value="json" 
                      control={<Radio />} 
                      label={
                        <Stack direction="row" spacing={1} alignItems="center">
                          <Code fontSize="small" />
                          <span>JSON</span>
                        </Stack>
                      } 
                    />
                  </RadioGroup>
                </FormControl>

                <FormControl component="fieldset" sx={{ mt: 3 }}>
                  <FormLabel component="legend">Dane do eksportu</FormLabel>
                  <FormGroup>
                    <FormControlLabel
                      control={
                        <Checkbox 
                          checked={selectedData.internships} 
                          onChange={handleDataChange} 
                          name="internships" 
                        />
                      }
                      label="Staże"
                    />
                    <FormControlLabel
                      control={
                        <Checkbox 
                          checked={selectedData.medicalShifts} 
                          onChange={handleDataChange} 
                          name="medicalShifts" 
                        />
                      }
                      label="Dyżury medyczne"
                    />
                    <FormControlLabel
                      control={
                        <Checkbox 
                          checked={selectedData.procedures} 
                          onChange={handleDataChange} 
                          name="procedures" 
                        />
                      }
                      label="Procedury"
                    />
                    <FormControlLabel
                      control={
                        <Checkbox 
                          checked={selectedData.courses} 
                          onChange={handleDataChange} 
                          name="courses" 
                        />
                      }
                      label="Kursy i szkolenia"
                    />
                    <FormControlLabel
                      control={
                        <Checkbox 
                          checked={selectedData.selfEducation} 
                          onChange={handleDataChange} 
                          name="selfEducation" 
                        />
                      }
                      label="Samokształcenie"
                    />
                    <FormControlLabel
                      control={
                        <Checkbox 
                          checked={selectedData.publications} 
                          onChange={handleDataChange} 
                          name="publications" 
                        />
                      }
                      label="Publikacje"
                    />
                    <FormControlLabel
                      control={
                        <Checkbox 
                          checked={selectedData.absences} 
                          onChange={handleDataChange} 
                          name="absences" 
                        />
                      }
                      label="Nieobecności"
                    />
                  </FormGroup>
                </FormControl>

                <Box sx={{ mt: 3 }}>
                  <Typography variant="subtitle2" gutterBottom>
                    Zakres dat (opcjonalnie)
                  </Typography>
                  <Stack direction="row" spacing={2}>
                    <DatePicker
                      label="Od"
                      value={dateFrom}
                      onChange={setDateFrom}
                      format="dd.MM.yyyy"
                      slotProps={{
                        textField: {
                          size: 'small',
                          fullWidth: true,
                        },
                      }}
                    />
                    <DatePicker
                      label="Do"
                      value={dateTo}
                      onChange={setDateTo}
                      format="dd.MM.yyyy"
                      minDate={dateFrom || undefined}
                      slotProps={{
                        textField: {
                          size: 'small',
                          fullWidth: true,
                        },
                      }}
                    />
                  </Stack>
                </Box>

                <Button
                  variant="contained"
                  size="large"
                  fullWidth
                  startIcon={isExporting ? <CircularProgress size={20} /> : getFormatIcon(exportFormat)}
                  onClick={handleGeneralExport}
                  disabled={isExporting || !Object.values(selectedData).some(v => v)}
                  sx={{ mt: 3 }}
                >
                  {isExporting ? 'Eksportowanie...' : 'Eksportuj dane'}
                </Button>
              </Box>
            </Paper>
          </Grid>

          {/* SMK Export */}
          <Grid item xs={12} lg={6}>
            <Paper sx={{ p: 3, height: '100%' }}>
              <Typography variant="h6" gutterBottom>
                Eksport do SMK
              </Typography>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Eksportuj dane zgodne z formatem Systemu Monitorowania Kształcenia
              </Typography>

              <Box sx={{ mt: 3 }}>
                <Alert severity="info" sx={{ mb: 3 }}>
                  Eksport SMK zawiera wszystkie dane wymagane przez system w odpowiednim formacie XML
                </Alert>

                <FormControl component="fieldset">
                  <FormLabel component="legend">Wersja SMK</FormLabel>
                  <RadioGroup
                    value={smkVersion}
                    onChange={(e) => setSmkVersion(e.target.value as 'Old' | 'New')}
                  >
                    <FormControlLabel 
                      value="New" 
                      control={<Radio />} 
                      label={
                        <Box>
                          <Typography variant="body2">Nowy SMK</Typography>
                          <Typography variant="caption" color="text.secondary">
                            Dla specjalizacji rozpoczętych po 2019 roku
                          </Typography>
                        </Box>
                      } 
                    />
                    <FormControlLabel 
                      value="Old" 
                      control={<Radio />} 
                      label={
                        <Box>
                          <Typography variant="body2">Stary SMK</Typography>
                          <Typography variant="caption" color="text.secondary">
                            Dla specjalizacji rozpoczętych przed 2019 rokiem
                          </Typography>
                        </Box>
                      } 
                    />
                  </RadioGroup>
                </FormControl>

                <Divider sx={{ my: 3 }} />

                <Box sx={{ mb: 3 }}>
                  <Typography variant="subtitle2" gutterBottom>
                    Dane, które zostaną wyeksportowane:
                  </Typography>
                  <Stack direction="row" spacing={1} flexWrap="wrap" sx={{ mt: 1 }}>
                    <Chip label="Staże" size="small" />
                    <Chip label="Dyżury" size="small" />
                    <Chip label="Procedury" size="small" />
                    <Chip label="Kursy" size="small" />
                    <Chip label="Samokształcenie" size="small" />
                    <Chip label="Publikacje" size="small" />
                  </Stack>
                </Box>

                <Button
                  variant="contained"
                  size="large"
                  fullWidth
                  color="secondary"
                  startIcon={isExporting ? <CircularProgress size={20} /> : <CloudUpload />}
                  onClick={handleSMKExport}
                  disabled={isExporting}
                >
                  {isExporting ? 'Eksportowanie...' : 'Eksportuj do SMK'}
                </Button>
              </Box>
            </Paper>
          </Grid>

          {/* Export History */}
          <Grid item xs={12}>
            <Paper sx={{ p: 3 }}>
              <Typography variant="h6" gutterBottom>
                Historia eksportów
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Ta funkcja będzie dostępna wkrótce
              </Typography>
            </Paper>
          </Grid>
        </Grid>
      </Box>
    </LocalizationProvider>
  );
};