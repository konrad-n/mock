namespace SledzSpecke.App.Views.Reports
{
    public partial class SMKExportPage : ContentPage
    {
        public SMKExportPage()
        {
            InitializeComponent();
            SetupInitialState();
        }

        private void SetupInitialState()
        {
            StartDatePicker.Date = DateTime.Now.AddMonths(-3);
            EndDatePicker.Date = DateTime.Now;
        }

        private void OnCustomDatesCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            DateRangeGrid.IsVisible = e.Value;
        }

        private async void OnGenerateReportClicked(object sender, EventArgs e)
        {
            // Pokazanie indykatora ładowania
            await ShowLoadingIndicator(true);

            try
            {
                // Symulacja generowania raportu
                await Task.Delay(2000);

                // Uzyskanie ścieżki do folderu Pobrane
                string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string fileName = $"SMK_Export_{DateTime.Now.ToString("yyyyMMdd")}.xlsx";
                string filePath = Path.Combine(downloadsPath, fileName);

                // W rzeczywistej aplikacji tutaj byłoby generowanie pliku Excel/CSV

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