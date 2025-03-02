using SledzSpecke.App.Common.Views;
using SledzSpecke.App.Features.Settings.ViewModels;

namespace SledzSpecke.App.Features.Settings.Views
{
    public partial class SettingsPage : BaseContentPage
    {
        private SettingsViewModel viewModel;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override async Task InitializePageAsync()
        {
            try
            {
                this.viewModel = this.GetRequiredService<SettingsViewModel>();
                this.BindingContext = this.viewModel;
                await this.viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Blad", "Nie udalo sie zaladowac ustawien.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error in SettingsPage: {ex}");
            }
        }

        private async void OnConfigureSMKClicked(object sender, EventArgs e)
        {
            await this.DisplayAlert("Informacja",
                "Funkcja konfiguracji integracji z SMK zostanie zaimplementowana w przyszlej wersji aplikacji.",
                "OK");
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                await this.viewModel.SaveChangesAsync();
                await this.DisplayAlert("Sukces", "Ustawienia zostaly zapisane pomyslnie.", "OK");
            }
            catch (InvalidOperationException ex)
            {
                await this.DisplayAlert("Blad", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await this.DisplayAlert("Blad",
                    $"Wystapil problem podczas zapisywania ustawien: {ex.Message}",
                    "OK");
            }
        }

        private async void OnClearDataClicked(object sender, EventArgs e)
        {
            bool answer = await this.DisplayAlert("Potwierdzenie",
                "Czy na pewno chcesz wyczyscic wszystkie dane? Ta operacja jest nieodwracalna.",
                "Tak",
                "Nie");
            if (answer)
            {
                try
                {
                    await this.viewModel.ClearDataAsync();
                    await this.DisplayAlert("Sukces", "Wszystkie dane zostaly wyczyszczone pomyslnie.", "OK");
                    await this.Navigation.PopToRootAsync();
                }
                catch (Exception ex)
                {
                    await this.DisplayAlert("Blad",
                        $"Wystapil problem podczas czyszczenia danych: {ex.Message}",
                        "OK");
                }
            }
        }
    }
}
