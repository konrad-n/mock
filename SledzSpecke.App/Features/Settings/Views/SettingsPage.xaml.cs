using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Settings.ViewModels;

namespace SledzSpecke.App.Features.Settings.Views
{
    public partial class SettingsPage
    {
        private SettingsViewModel _viewModel;

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                _viewModel = GetRequiredService<SettingsViewModel>();
                BindingContext = _viewModel;
                await _viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", "Nie udało się załadować ustawień.", "OK");
            }
        }

        private async void OnConfigureSMKClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Informacja",
                "Funkcja konfiguracji integracji z SMK zostanie zaimplementowana w przyszłej wersji aplikacji.",
                "OK");
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                await _viewModel.SaveChangesAsync();
                await DisplayAlert("Sukces", "Ustawienia zostały zapisane pomyślnie.", "OK");
            }
            catch (InvalidOperationException ex)
            {
                await DisplayAlert("Błąd", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd",
                    $"Wystąpił problem podczas zapisywania ustawień: {ex.Message}",
                    "OK");
            }
        }

        private async void OnClearDataClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Potwierdzenie",
                "Czy na pewno chcesz wyczyścić wszystkie dane? Ta operacja jest nieodwracalna.",
                "Tak",
                "Nie");

            if (answer)
            {
                try
                {
                    await _viewModel.ClearDataAsync();
                    await DisplayAlert("Sukces", "Wszystkie dane zostały wyczyszczone pomyślnie.", "OK");
                    await Navigation.PopToRootAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Błąd",
                        $"Wystąpił problem podczas czyszczenia danych: {ex.Message}",
                        "OK");
                }
            }
        }
    }
}