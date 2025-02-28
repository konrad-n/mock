using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Views
{
    public partial class SMKExportPage : ContentPage
    {
        private SMKExportOptions _exportOptions;

        public bool IsGeneralExportSelected => GeneralExportRadioButton.IsChecked;

        public SMKExportPage()
        {
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

            // SMK format settings
            _exportOptions.UseSmkExactFormat = UseSmkExactFormatCheckBox.IsChecked;
            _exportOptions.SplitDutyHoursAndMinutes = true; // Always true for SMK format

            // Show loading indicator
            await ShowLoadingIndicator(true);

            try
            {
                // Generate report
                string filePath = await App.ExportService.ExportToSMKAsync(_exportOptions);

                // Display file information
                FilePathLabel.Text = $"Ścieżka pliku: {filePath}";
                ResultFrame.IsVisible = true;

                await DisplayAlert("Sukces", "Raport został wygenerowany pomyślnie w formacie zgodnym z SMK.", "OK");
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
            await Task.CompletedTask;
            // In a real application, this would implement showing/hiding a loading indicator
        }

        private async void OnOpenFileClicked(object sender, EventArgs e)
        {
            try
            {
                // Try to open the file in the default application
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(FilePathLabel.Text.Replace("Ścieżka pliku: ", ""))
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
                await Launcher.OpenAsync(folderPath);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się otworzyć folderu: {ex.Message}", "OK");
            }
        }
    }
}