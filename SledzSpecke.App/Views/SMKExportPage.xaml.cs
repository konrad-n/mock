using SledzSpecke.App.Services;

namespace SledzSpecke.App.Views
{
    public partial class SMKExportPage : ContentPage
    {
        private SMKExportOptions _exportOptions;
        public SMKExportPage()
        {
            InitializeComponent();
            SetupInitialState();
        }

        private void SetupInitialState()
        {
            _exportOptions = new SMKExportOptions();

            StartDatePicker.Date = DateTime.Now.AddMonths(-3);
            EndDatePicker.Date = DateTime.Now;
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
            // Pobierz opcje eksportu z UI
            _exportOptions.IncludeCourses = CoursesCheckBox.IsChecked;
            _exportOptions.IncludeInternships = InternshipsCheckBox.IsChecked;
            _exportOptions.IncludeProcedures = ProceduresCheckBox.IsChecked;
            _exportOptions.Format = FormatPicker.SelectedIndex == 0 ? ExportFormat.Excel : ExportFormat.Csv;

            if (AllModulesRadioButton.IsChecked)
                _exportOptions.SelectedModule = SMKExportModuleFilter.All;
            else if (BasicModuleRadioButton.IsChecked)
                _exportOptions.SelectedModule = SMKExportModuleFilter.BasicOnly;
            else if (SpecialisticModuleRadioButton.IsChecked)
                _exportOptions.SelectedModule = SMKExportModuleFilter.SpecialisticOnly;

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