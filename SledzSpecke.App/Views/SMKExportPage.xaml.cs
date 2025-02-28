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

            // Pokazanie indykatora ładowania
            await ShowLoadingIndicator(true);

            try
            {
                // Generowanie raportu
                string filePath = await App.ExportService.ExportToSMKAsync(_exportOptions);

                // Wyświetlenie informacji o wygenerowanym pliku
                FilePathLabel.Text = $"Ścieżka pliku: {filePath}";
                ResultFrame.IsVisible = true;

                await DisplayAlert("Sukces", "Raport został wygenerowany pomyślnie.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił problem podczas generowania raportu: {ex.Message}", "OK");
            }
            finally
            {
                // Ukrycie indykatora ładowania
                await ShowLoadingIndicator(false);
            }
        }

        private async Task ShowLoadingIndicator(bool isVisible)
        {
            await Task.CompletedTask;
            // W rzeczywistej aplikacji tutaj byłaby implementacja pokazywania/ukrywania indykatora ładowania
        }

        private async void OnOpenFileClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Informacja", "Funkcja otwierania pliku zostanie zaimplementowana.", "OK");
            // W rzeczywistej aplikacji tutaj byłoby otwieranie pliku
        }

        private async void OnOpenFolderClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Informacja", "Funkcja otwierania folderu zostanie zaimplementowana.", "OK");
            // W rzeczywistej aplikacji tutaj byłoby otwieranie folderu
        }
    }
}