using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Settings.ViewModels;

namespace SledzSpecke.App.Features.Settings.Views
{
    public partial class SettingsPage : BaseContentPage
    {
        private SettingsViewModel _viewModel;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this._viewModel = this.GetRequiredService<SettingsViewModel>();
                this.BindingContext = this._viewModel;
                await this._viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd", "Nie udało się załadować ustawień.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in SettingsPage: {ex}");
            }
        }

        private async void OnConfigureSMKClicked(object sender, EventArgs e)
        {
            await this.DisplayAlert("Informacja",
                "Funkcja konfiguracji integracji z SMK zostanie zaimplementowana w przyszłej wersji aplikacji.",
                "OK");
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                await this._viewModel.SaveChangesAsync();
                await this.DisplayAlert("Sukces", "Ustawienia zostały zapisane pomyślnie.", "OK");
            }
            catch (InvalidOperationException ex)
            {
                await this.DisplayAlert("Błąd", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Błąd",
                    $"Wystąpił problem podczas zapisywania ustawień: {ex.Message}",
                    "OK");
            }
        }

        private async void OnClearDataClicked(object sender, EventArgs e)
        {
            bool answer = await this.DisplayAlert("Potwierdzenie",
                "Czy na pewno chcesz wyczyścić wszystkie dane? Ta operacja jest nieodwracalna.",
                "Tak",
                "Nie");
            if (answer)
            {
                try
                {
                    await this._viewModel.ClearDataAsync();
                    await this.DisplayAlert("Sukces", "Wszystkie dane zostały wyczyszczone pomyślnie.", "OK");
                    await this.Navigation.PopToRootAsync();
                }
                catch (Exception ex)
                {
                    await this.DisplayAlert("Błąd",
                        $"Wystąpił problem podczas czyszczenia danych: {ex.Message}",
                        "OK");
                }
            }
        }
    }
}