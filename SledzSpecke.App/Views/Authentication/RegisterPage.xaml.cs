using SledzSpecke.App.ViewModels.Authentication;

namespace SledzSpecke.App.Views.Authentication
{
    public partial class RegisterPage : ContentPage
    {
        private readonly RegisterViewModel viewModel;

        public RegisterPage(RegisterViewModel viewModel)
        {
            this.InitializeComponent();
            this.BindingContext = viewModel;
            this.viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Krytyczne - to zapewnia załadowanie danych przed pokazaniem ekranu
                await this.viewModel.InitializeAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas inicjalizacji RegisterViewModel: {ex.Message}");
                await this.DisplayAlert("Błąd", "Nie udało się załadować listy specjalizacji.", "OK");
            }
        }
    }
}