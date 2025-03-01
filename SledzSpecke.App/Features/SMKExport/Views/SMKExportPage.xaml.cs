using DocumentFormat.OpenXml.Vml.Spreadsheet;
using SledzSpecke.App.Services;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.SMKExport.Views
{
    public partial class SMKExportPage : ContentPage
    {
        private SMKExportOptions _exportOptions;
        private IExportService _exportService;

        public bool IsGeneralExportSelected => GeneralExportRadioButton.IsChecked;

        public SMKExportPage(
            IExportService exportService)
        {
            _exportService = exportService;

            InitializeComponent();
            SetupInitialState();

            // Bind properties for visibility
            BindingContext = this;

            // Set up radio button event handlers
            GeneralExportRadioButton.CheckedChanged += OnExportTypeChanged;
            ProcedureExportRadioButton.CheckedChanged += OnExportTypeChanged;
            DutyShiftExportRadioButton.CheckedChanged += OnExportTypeChanged;
        }

        private void SetupInitialState()
        {
            _exportOptions = new SMKExportOptions();

            StartDatePicker.Date = DateTime.Now.AddMonths(-3);
            EndDatePicker.Date = DateTime.Now;

            // Ensure LoadingOverlay is behind everything but invisible
            LoadingOverlay.IsVisible = false;
            LoadingOverlay.ZIndex = 1000;
        }

        private void OnExportTypeChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return; // Only handle when checked, not unchecked

            OnPropertyChanged(nameof(IsGeneralExportSelected));

            // Update SMK format options based on export type
            UseSmkExactFormatCheckBox.IsChecked = ProcedureExportRadioButton.IsChecked || DutyShiftExportRadioButton.IsChecked;
        }

        private void OnCustomDatesCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            DateRangeGrid.IsVisible = e.Value;
            _exportOptions.UseCustomDateRange = e.Value;

            if (e.Value)
            {
                _exportOptions.StartDate = StartDatePicker.Date;
                _exportOptions.EndDate = EndDatePicker.Date;
            }
            else
            {
                _exportOptions.StartDate = null;
                _exportOptions.EndDate = null;
            }
        }

        private async void OnGenerateReportClicked(object sender, EventArgs e)
        {
            // Validate selections
            if (CustomDatesRadioButton.IsChecked && StartDatePicker.Date > EndDatePicker.Date)
            {
                await DisplayAlert("Błąd zakresu dat", "Data początkowa nie może być późniejsza niż data końcowa.", "OK");
                return;
            }

            // If general export selected, ensure at least one category is selected
            if (GeneralExportRadioButton.IsChecked &&
                !CoursesCheckBox.IsChecked && !InternshipsCheckBox.IsChecked && !ProceduresCheckBox.IsChecked)
            {
                await DisplayAlert("Błąd wyboru zakresu", "Wybierz przynajmniej jedną kategorię danych do eksportu.", "OK");
                return;
            }

            // Determine export type
            if (GeneralExportRadioButton.IsChecked)
                _exportOptions.ExportType = SMKExportType.General;
            else if (ProcedureExportRadioButton.IsChecked)
                _exportOptions.ExportType = SMKExportType.Procedures;
            else if (DutyShiftExportRadioButton.IsChecked)
                _exportOptions.ExportType = SMKExportType.DutyShifts;

            // Only set these options if general export is selected
            if (_exportOptions.ExportType == SMKExportType.General)
            {
                _exportOptions.IncludeCourses = CoursesCheckBox.IsChecked;
                _exportOptions.IncludeInternships = InternshipsCheckBox.IsChecked;
                _exportOptions.IncludeProcedures = ProceduresCheckBox.IsChecked;
            }

            _exportOptions.Format = FormatPicker.SelectedIndex == 0 ? ExportFormat.Excel : ExportFormat.CSV;

            if (AllModulesRadioButton.IsChecked)
                _exportOptions.ModuleFilter = SMKExportModuleFilter.All;
            else if (BasicModuleRadioButton.IsChecked)
                _exportOptions.ModuleFilter = SMKExportModuleFilter.BasicOnly;
            else if (SpecialisticModuleRadioButton.IsChecked)
                _exportOptions.ModuleFilter = SMKExportModuleFilter.SpecialisticOnly;

            // Date range settings
            if (CustomDatesRadioButton.IsChecked)
            {
                _exportOptions.UseCustomDateRange = true;
                _exportOptions.StartDate = StartDatePicker.Date;
                _exportOptions.EndDate = EndDatePicker.Date;
            }
            else
            {
                _exportOptions.UseCustomDateRange = false;
                _exportOptions.StartDate = null;
                _exportOptions.EndDate = null;
            }

            // SMK format settings
            _exportOptions.UseSmkExactFormat = UseSmkExactFormatCheckBox.IsChecked;
            _exportOptions.SplitDutyHoursAndMinutes = true; // Always true for SMK format

            // Show loading indicator
            await ShowLoadingIndicator(true);

            try
            {
                // Generate report
                string filePath = await _exportService.ExportToSMKAsync(_exportOptions);

                // Display file information
                FilePathLabel.Text = $"Ścieżka pliku: {filePath}";
                ResultFrame.IsVisible = true;

                await ScrollToResultFrame();

                await DisplayAlert("Sukces", $"Raport został wygenerowany pomyślnie w formacie zgodnym z SMK.\n\nFormat: {(_exportOptions.Format == ExportFormat.Excel ? "Excel" : "CSV")}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił problem podczas generowania raportu: {ex.Message}", "OK");
            }
            finally
            {
                // Hide loading indicator
                await ShowLoadingIndicator(false);
            }
        }

        private async Task ShowLoadingIndicator(bool isVisible)
        {
            LoadingOverlay.IsVisible = isVisible;

            if (isVisible)
            {
                // Ensure UI is updated before continuing with long operation
                await Task.Delay(50);
            }
        }

        private async void OnOpenFileClicked(object sender, EventArgs e)
        {
            try
            {
                string filePath = FilePathLabel.Text.Replace("Ścieżka pliku: ", "");

                // Check if file exists
                if (!File.Exists(filePath))
                {
                    await DisplayAlert("Błąd", "Plik nie istnieje lub został usunięty.", "OK");
                    return;
                }

                // Try to open the file in the default application
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się otworzyć pliku: {ex.Message}", "OK");
            }
        }

        private async void OnOpenFolderClicked(object sender, EventArgs e)
        {
            try
            {
                // Try to open the folder containing the file
                string filePath = FilePathLabel.Text.Replace("Ścieżka pliku: ", "");
                string folderPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(folderPath))
                {
                    await DisplayAlert("Błąd", "Folder nie istnieje lub został usunięty.", "OK");
                    return;
                }

                await Launcher.OpenAsync(folderPath);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się otworzyć folderu: {ex.Message}", "OK");
            }
        }

        private async Task ScrollToResultFrame()
        {
            if (scrollView != null)
            {
                await scrollView.ScrollToAsync(ResultFrame, ScrollToPosition.Start, true);
            }
        }
    }
}